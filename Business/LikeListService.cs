using EsunLikeList.Common.Models;
using EsunLikeList.Data;
namespace EsunLikeList.Business;
public interface ILikeListService
{
    Task<UserInfo?> GetUserAsync(string userId);
    Task<List<LikeItem>> GetListAsync(string userId, string? keyword = null);
    Task<LikeItemInput?> GetInputAsync(string userId, int sn);
    Task<int> AddAsync(string userId, LikeItemInput input);
    Task<bool> UpdateAsync(string userId, LikeItemInput input);
    Task<bool> DeleteAsync(string userId, int sn);
}
public class LikeListService : ILikeListService
{
    private readonly ILikeListRepository _repo;
    public LikeListService(ILikeListRepository repo) => _repo = repo;
    private static LikeItem ToItem(LikeItemInput i)
    {
        var fee = Math.Round(i.Price * i.PurchaseQuantity * i.FeeRate, 0, MidpointRounding.AwayFromZero);
        return new LikeItem
        {
            SN = i.SN,
            ProductName = i.ProductName.Trim(),
            Price = i.Price,
            FeeRate = i.FeeRate,
            PurchaseQuantity = i.PurchaseQuantity,
            Account = i.Account.Trim(),
            TotalFee = fee,
            TotalAmount = i.Price * i.PurchaseQuantity + fee
        };
    }
    public Task<UserInfo?> GetUserAsync(string userId) => _repo.GetUserAsync(userId);
    public Task<List<LikeItem>> GetListAsync(string userId, string? keyword = null) => _repo.GetListAsync(userId, keyword?.Trim());
    public async Task<LikeItemInput?> GetInputAsync(string userId, int sn)
    {
        var i = await _repo.GetAsync(userId, sn);
        return i == null ? null : new LikeItemInput
        {
            SN = i.SN, ProductName = i.ProductName, Price = i.Price, FeeRate = i.FeeRate,
            PurchaseQuantity = i.PurchaseQuantity, Account = i.Account
        };
    }
    public Task<int> AddAsync(string userId, LikeItemInput input) => _repo.AddAsync(userId, ToItem(input));
    public Task<bool> UpdateAsync(string userId, LikeItemInput input) => _repo.UpdateAsync(userId, ToItem(input));
    public Task<bool> DeleteAsync(string userId, int sn) => _repo.DeleteAsync(userId, sn);
}
