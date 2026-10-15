USE EsunLikeDb;
GO
ALTER PROCEDURE dbo.usp_LikeList_GetByUser @UserID NVARCHAR(20), @Keyword NVARCHAR(100) = NULL AS
BEGIN
    SET NOCOUNT ON;
    SELECT l.SN, l.ProductNo, p.ProductName, p.Price, p.FeeRate, l.PurchaseQuantity,
           l.Account, l.TotalFee, l.TotalAmount, u.Email
    FROM dbo.LikeList l
    JOIN dbo.Products p ON p.No = l.ProductNo
    JOIN dbo.Users u ON u.UserID = l.UserID
    WHERE l.UserID = @UserID
      AND (@Keyword IS NULL OR @Keyword = N''
           OR CHARINDEX(@Keyword, p.ProductName) > 0
           OR CHARINDEX(@Keyword, l.Account) > 0)
    ORDER BY l.SN;
END
GO
