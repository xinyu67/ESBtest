IF DB_ID('EsunLikeDb') IS NULL CREATE DATABASE EsunLikeDb;
GO
USE EsunLikeDb;
GO
IF OBJECT_ID('dbo.LikeList') IS NOT NULL DROP TABLE dbo.LikeList;
IF OBJECT_ID('dbo.Products') IS NOT NULL DROP TABLE dbo.Products;
IF OBJECT_ID('dbo.Users') IS NOT NULL DROP TABLE dbo.Users;
GO
CREATE TABLE dbo.Users (
    UserID   NVARCHAR(20)  NOT NULL PRIMARY KEY,
    UserName NVARCHAR(50)  NOT NULL,
    Email    NVARCHAR(100) NOT NULL,
    Account  NVARCHAR(20)  NOT NULL
);
CREATE TABLE dbo.Products (
    No          INT IDENTITY(1,1) PRIMARY KEY,
    ProductName NVARCHAR(100) NOT NULL,
    Price       DECIMAL(18,2) NOT NULL CHECK (Price > 0),
    FeeRate     DECIMAL(5,4)  NOT NULL CHECK (FeeRate BETWEEN 0 AND 1)
);
CREATE TABLE dbo.LikeList (
    SN               INT IDENTITY(1,1) PRIMARY KEY,
    UserID           NVARCHAR(20) NOT NULL REFERENCES dbo.Users(UserID),
    ProductNo        INT NOT NULL REFERENCES dbo.Products(No),
    PurchaseQuantity INT NOT NULL CHECK (PurchaseQuantity > 0),
    Account          NVARCHAR(20) NOT NULL,
    TotalFee         DECIMAL(18,0) NOT NULL,
    TotalAmount      DECIMAL(18,2) NOT NULL
);
GO
