using System.Data;
using EsunLikeList.Common.Models;
using Microsoft.Data.SqlClient;
namespace EsunLikeList.Data;
public interface ILikeListRepository
{
    Task<UserInfo?> GetUserAsync(string userId);
    Task<List<LikeItem>> GetListAsync(string userId, string? keyword = null);
    Task<LikeItem?> GetAsync(string userId, int sn);
    Task<int> AddAsync(string userId, LikeItem item);
    Task<bool> UpdateAsync(string userId, LikeItem item);
    Task<bool> DeleteAsync(string userId, int sn);
}
public class LikeListRepository : ILikeListRepository
{
    private readonly string _cs;
    public LikeListRepository(string connectionString) => _cs = connectionString;
    private static SqlCommand Sp(SqlConnection conn, string name) =>
        new(name, conn) { CommandType = CommandType.StoredProcedure };
    private static void FillItem(SqlCommand cmd, string userId, LikeItem i)
    {
        cmd.Parameters.Add("@UserID", SqlDbType.NVarChar, 20).Value = userId;
        cmd.Parameters.Add("@ProductName", SqlDbType.NVarChar, 100).Value = i.ProductName;
        cmd.Parameters.Add("@Price", SqlDbType.Decimal).Value = i.Price;
        cmd.Parameters.Add("@FeeRate", SqlDbType.Decimal).Value = i.FeeRate;
        cmd.Parameters.Add("@PurchaseQuantity", SqlDbType.Int).Value = i.PurchaseQuantity;
        cmd.Parameters.Add("@Account", SqlDbType.NVarChar, 20).Value = i.Account;
        cmd.Parameters.Add("@TotalFee", SqlDbType.Decimal).Value = i.TotalFee;
        cmd.Parameters.Add("@TotalAmount", SqlDbType.Decimal).Value = i.TotalAmount;
    }
    private static LikeItem Map(SqlDataReader r) => new()
    {
        SN = r.GetInt32(r.GetOrdinal("SN")),
        ProductNo = r.GetInt32(r.GetOrdinal("ProductNo")),
        ProductName = r.GetString(r.GetOrdinal("ProductName")),
        Price = r.GetDecimal(r.GetOrdinal("Price")),
        FeeRate = r.GetDecimal(r.GetOrdinal("FeeRate")),
        PurchaseQuantity = r.GetInt32(r.GetOrdinal("PurchaseQuantity")),
        Account = r.GetString(r.GetOrdinal("Account")),
        TotalFee = r.GetDecimal(r.GetOrdinal("TotalFee")),
        TotalAmount = r.GetDecimal(r.GetOrdinal("TotalAmount")),
        Email = r.GetString(r.GetOrdinal("Email"))
    };
    public async Task<UserInfo?> GetUserAsync(string userId)
    {
        await using var conn = new SqlConnection(_cs);
        await conn.OpenAsync();
        await using var cmd = Sp(conn, "dbo.usp_User_Get");
        cmd.Parameters.Add("@UserID", SqlDbType.NVarChar, 20).Value = userId;
        await using var r = await cmd.ExecuteReaderAsync();
        if (!await r.ReadAsync()) return null;
        return new UserInfo
        {
            UserID = r.GetString(0), UserName = r.GetString(1),
            Email = r.GetString(2), Account = r.GetString(3)
        };
    }
    public async Task<List<LikeItem>> GetListAsync(string userId, string? keyword = null)
    {
        var list = new List<LikeItem>();
        await using var conn = new SqlConnection(_cs);
        await conn.OpenAsync();
        await using var cmd = Sp(conn, "dbo.usp_LikeList_GetByUser");
        cmd.Parameters.Add("@UserID", SqlDbType.NVarChar, 20).Value = userId;
        cmd.Parameters.Add("@Keyword", SqlDbType.NVarChar, 100).Value = (object?)keyword ?? DBNull.Value;
        await using var r = await cmd.ExecuteReaderAsync();
        while (await r.ReadAsync()) list.Add(Map(r));
        return list;
    }
    public async Task<LikeItem?> GetAsync(string userId, int sn)
    {
        await using var conn = new SqlConnection(_cs);
        await conn.OpenAsync();
        await using var cmd = Sp(conn, "dbo.usp_LikeList_GetBySN");
        cmd.Parameters.Add("@UserID", SqlDbType.NVarChar, 20).Value = userId;
        cmd.Parameters.Add("@SN", SqlDbType.Int).Value = sn;
        await using var r = await cmd.ExecuteReaderAsync();
        return await r.ReadAsync() ? Map(r) : null;
    }
    public async Task<int> AddAsync(string userId, LikeItem item)
    {
        await using var conn = new SqlConnection(_cs);
        await conn.OpenAsync();
        await using var cmd = Sp(conn, "dbo.usp_LikeList_Add");
        FillItem(cmd, userId, item);
        return Convert.ToInt32(await cmd.ExecuteScalarAsync());
    }
    public async Task<bool> UpdateAsync(string userId, LikeItem item)
    {
        await using var conn = new SqlConnection(_cs);
        await conn.OpenAsync();
        await using var cmd = Sp(conn, "dbo.usp_LikeList_Update");
        FillItem(cmd, userId, item);
        cmd.Parameters.Add("@SN", SqlDbType.Int).Value = item.SN;
        return Convert.ToInt32(await cmd.ExecuteScalarAsync()) > 0;
    }
    public async Task<bool> DeleteAsync(string userId, int sn)
    {
        await using var conn = new SqlConnection(_cs);
        await conn.OpenAsync();
        await using var cmd = Sp(conn, "dbo.usp_LikeList_Delete");
        cmd.Parameters.Add("@UserID", SqlDbType.NVarChar, 20).Value = userId;
        cmd.Parameters.Add("@SN", SqlDbType.Int).Value = sn;
        return Convert.ToInt32(await cmd.ExecuteScalarAsync()) > 0;
    }
}
