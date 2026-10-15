using System.ComponentModel.DataAnnotations;

namespace EsunLikeList.Common.Models;

public class UserInfo
{
    public string UserID { get; set; } = "";
    public string UserName { get; set; } = "";
    public string Email { get; set; } = "";
    public string Account { get; set; } = "";
}

/// <summary>喜好清單顯示資料(Product + LikeList + User 聯合結果)</summary>
public class LikeItem
{
    public int SN { get; set; }
    public int ProductNo { get; set; }
    public string ProductName { get; set; } = "";
    public decimal Price { get; set; }
    public decimal FeeRate { get; set; }
    public int PurchaseQuantity { get; set; }
    public string Account { get; set; } = "";
    public decimal TotalFee { get; set; }
    public decimal TotalAmount { get; set; }
    public string Email { get; set; } = "";
}

/// <summary>新增/修改的表單輸入</summary>
public class LikeItemInput
{
    public int SN { get; set; }

    [Required(ErrorMessage = "請輸入產品名稱"), StringLength(100)]
    [Display(Name = "產品名稱")]
    public string ProductName { get; set; } = "";

    [Range(typeof(decimal), "0.01", "100000000", ErrorMessage = "價格需大於 0")]
    [Display(Name = "產品價格")]
    public decimal Price { get; set; }

    [Range(typeof(decimal), "0", "1", ErrorMessage = "費率需介於 0 ~ 1 (例如 0.01 = 1%)")]
    [Display(Name = "手續費率 (0.01 = 1%)")]
    public decimal FeeRate { get; set; }

    [Range(1, 1000000, ErrorMessage = "數量需大於 0")]
    [Display(Name = "購買數量")]
    public int PurchaseQuantity { get; set; } = 1;

    [Required(ErrorMessage = "請輸入扣款帳號"), RegularExpression(@"^\d{10,16}$", ErrorMessage = "帳號需為 10~16 位數字")]
    [Display(Name = "扣款帳號")]
    public string Account { get; set; } = "";
}
