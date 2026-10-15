USE EsunLikeDb;
GO
CREATE PROCEDURE dbo.usp_User_Get @UserID NVARCHAR(20) AS
BEGIN
    SET NOCOUNT ON;
    SELECT UserID, UserName, Email, Account FROM dbo.Users WHERE UserID = @UserID;
END
GO
CREATE PROCEDURE dbo.usp_LikeList_GetByUser @UserID NVARCHAR(20) AS
BEGIN
    SET NOCOUNT ON;
    SELECT l.SN, l.ProductNo, p.ProductName, p.Price, p.FeeRate, l.PurchaseQuantity,
           l.Account, l.TotalFee, l.TotalAmount, u.Email
    FROM dbo.LikeList l
    JOIN dbo.Products p ON p.No = l.ProductNo
    JOIN dbo.Users u ON u.UserID = l.UserID
    WHERE l.UserID = @UserID
    ORDER BY l.SN;
END
GO
CREATE PROCEDURE dbo.usp_LikeList_GetBySN @UserID NVARCHAR(20), @SN INT AS
BEGIN
    SET NOCOUNT ON;
    SELECT l.SN, l.ProductNo, p.ProductName, p.Price, p.FeeRate, l.PurchaseQuantity,
           l.Account, l.TotalFee, l.TotalAmount, u.Email
    FROM dbo.LikeList l
    JOIN dbo.Products p ON p.No = l.ProductNo
    JOIN dbo.Users u ON u.UserID = l.UserID
    WHERE l.UserID = @UserID AND l.SN = @SN;
END
GO
-- 新增:同時異動 Products 與 LikeList,使用 Transaction
CREATE PROCEDURE dbo.usp_LikeList_Add
    @UserID NVARCHAR(20), @ProductName NVARCHAR(100), @Price DECIMAL(18,2), @FeeRate DECIMAL(5,4),
    @PurchaseQuantity INT, @Account NVARCHAR(20), @TotalFee DECIMAL(18,0), @TotalAmount DECIMAL(18,2)
AS
BEGIN
    SET NOCOUNT ON; SET XACT_ABORT ON;
    BEGIN TRY
        BEGIN TRAN;
        INSERT INTO dbo.Products (ProductName, Price, FeeRate) VALUES (@ProductName, @Price, @FeeRate);
        DECLARE @No INT = SCOPE_IDENTITY();
        INSERT INTO dbo.LikeList (UserID, ProductNo, PurchaseQuantity, Account, TotalFee, TotalAmount)
        VALUES (@UserID, @No, @PurchaseQuantity, @Account, @TotalFee, @TotalAmount);
        DECLARE @SN INT = SCOPE_IDENTITY();
        COMMIT TRAN;
        SELECT @SN;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRAN;
        THROW;
    END CATCH
END
GO
CREATE PROCEDURE dbo.usp_LikeList_Update
    @UserID NVARCHAR(20), @SN INT, @ProductName NVARCHAR(100), @Price DECIMAL(18,2), @FeeRate DECIMAL(5,4),
    @PurchaseQuantity INT, @Account NVARCHAR(20), @TotalFee DECIMAL(18,0), @TotalAmount DECIMAL(18,2)
AS
BEGIN
    SET NOCOUNT ON; SET XACT_ABORT ON;
    BEGIN TRY
        BEGIN TRAN;
        DECLARE @No INT = (SELECT ProductNo FROM dbo.LikeList WHERE SN = @SN AND UserID = @UserID);
        IF @No IS NULL BEGIN ROLLBACK TRAN; SELECT 0; RETURN; END
        UPDATE dbo.Products SET ProductName = @ProductName, Price = @Price, FeeRate = @FeeRate WHERE No = @No;
        UPDATE dbo.LikeList SET PurchaseQuantity = @PurchaseQuantity, Account = @Account,
               TotalFee = @TotalFee, TotalAmount = @TotalAmount WHERE SN = @SN AND UserID = @UserID;
        COMMIT TRAN;
        SELECT 1;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRAN;
        THROW;
    END CATCH
END
GO
CREATE PROCEDURE dbo.usp_LikeList_Delete @UserID NVARCHAR(20), @SN INT AS
BEGIN
    SET NOCOUNT ON; SET XACT_ABORT ON;
    BEGIN TRY
        BEGIN TRAN;
        DECLARE @No INT = (SELECT ProductNo FROM dbo.LikeList WHERE SN = @SN AND UserID = @UserID);
        IF @No IS NULL BEGIN ROLLBACK TRAN; SELECT 0; RETURN; END
        DELETE FROM dbo.LikeList WHERE SN = @SN AND UserID = @UserID;
        DELETE FROM dbo.Products WHERE No = @No AND NOT EXISTS (SELECT 1 FROM dbo.LikeList WHERE ProductNo = @No);
        COMMIT TRAN;
        SELECT 1;
    END TRY
    BEGIN CATCH
        IF @@TRANCOUNT > 0 ROLLBACK TRAN;
        THROW;
    END CATCH
END
GO
