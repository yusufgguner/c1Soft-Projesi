IF DB_ID('C1SoftB2B') IS NULL
BEGIN
    CREATE DATABASE C1SoftB2B;
END
GO

USE C1SoftB2B;
GO

IF OBJECT_ID('Roles', 'U') IS NULL
BEGIN
    CREATE TABLE Roles
    (
        RoleId INT IDENTITY(1,1) PRIMARY KEY,
        RoleName NVARCHAR(50) NOT NULL UNIQUE
    );
END
GO

IF OBJECT_ID('Users', 'U') IS NULL
BEGIN
    CREATE TABLE Users
    (
        UserId INT IDENTITY(1,1) PRIMARY KEY,
        RoleId INT NOT NULL,
        FirstName NVARCHAR(50) NOT NULL,
        LastName NVARCHAR(50) NOT NULL,
        Email NVARCHAR(150) NOT NULL UNIQUE,
        Username NVARCHAR(50) NOT NULL UNIQUE,
        PasswordHash NVARCHAR(200) NOT NULL,
        IsActive BIT NOT NULL DEFAULT 1,
        CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
        CONSTRAINT FK_Users_Roles FOREIGN KEY (RoleId) REFERENCES Roles(RoleId)
    );
END
GO

IF COL_LENGTH('Users', 'Phone') IS NULL
BEGIN
    ALTER TABLE Users ADD Phone NVARCHAR(30) NULL;
END
GO

IF NOT EXISTS (SELECT 1 FROM Roles WHERE RoleName = 'Admin')
BEGIN
    INSERT INTO Roles (RoleName) VALUES ('Admin');
END

IF NOT EXISTS (SELECT 1 FROM Roles WHERE RoleName = 'Customer')
BEGIN
    INSERT INTO Roles (RoleName) VALUES ('Customer');
END
GO

IF NOT EXISTS (SELECT 1 FROM Users WHERE Username = 'admin')
BEGIN
    INSERT INTO Users
    (RoleId, FirstName, LastName, Email, Username, PasswordHash)
    SELECT RoleId, 'System', 'Admin', 'admin@c1soft.local', 'admin',
           '3EB3FE66B31E3B4D10FA70B5CAD49C7112294AF6AE4E476A1C405155D45AA121'
    FROM Roles
    WHERE RoleName = 'Admin';
END

IF NOT EXISTS (SELECT 1 FROM Users WHERE Username = 'customer')
BEGIN
    INSERT INTO Users
    (RoleId, FirstName, LastName, Email, Username, PasswordHash)
    SELECT RoleId, 'Test', 'Customer', 'customer@c1soft.local', 'customer',
           '6F9AF26F62F7568BEE1E6F849594547A8CFDF904496FAB19CDEE7192A2C2B538'
    FROM Roles
    WHERE RoleName = 'Customer';
END
GO

IF OBJECT_ID('Categories', 'U') IS NULL
BEGIN
    CREATE TABLE Categories
    (
        CategoryId INT IDENTITY(1,1) PRIMARY KEY,
        CategoryName NVARCHAR(100) NOT NULL UNIQUE,
        Description NVARCHAR(250) NULL,
        IsActive BIT NOT NULL DEFAULT 1,
        CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE()
    );
END
GO

IF NOT EXISTS (SELECT 1 FROM Categories WHERE CategoryName = 'Ofis Malzemeleri')
BEGIN
    INSERT INTO Categories (CategoryName, Description)
    VALUES ('Ofis Malzemeleri', 'Ofiste kullanılan temel ürünler');
END

IF NOT EXISTS (SELECT 1 FROM Categories WHERE CategoryName = 'Temizlik Ürünleri')
BEGIN
    INSERT INTO Categories (CategoryName, Description)
    VALUES ('Temizlik Ürünleri', 'Temizlik ve hijyen ürünleri');
END
GO

IF OBJECT_ID('Products', 'U') IS NULL
BEGIN
    CREATE TABLE Products
    (
        ProductId INT IDENTITY(1,1) PRIMARY KEY,
        CategoryId INT NOT NULL,
        ProductCode NVARCHAR(50) NOT NULL UNIQUE,
        ProductName NVARCHAR(150) NOT NULL,
        Description NVARCHAR(500) NULL,
        Brand NVARCHAR(100) NULL,
        ManufacturerCode NVARCHAR(50) NULL,
        CustomCode1 NVARCHAR(50) NULL,
        CustomCode2 NVARCHAR(50) NULL,
        ImageUrl NVARCHAR(300) NULL,
        StockQuantity INT NOT NULL DEFAULT 0,
        CriticalStockLevel INT NOT NULL DEFAULT 5,
        Price DECIMAL(18,2) NOT NULL,
        IsActive BIT NOT NULL DEFAULT 1,
        CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
        CONSTRAINT FK_Products_Categories FOREIGN KEY (CategoryId) REFERENCES Categories(CategoryId)
    );
END
GO

IF NOT EXISTS (SELECT 1 FROM Products WHERE ProductCode = 'P-1001')
BEGIN
    INSERT INTO Products
    (
        CategoryId, ProductCode, ProductName, Description, Brand,
        ManufacturerCode, CustomCode1, ImageUrl, StockQuantity,
        CriticalStockLevel, Price
    )
    SELECT CategoryId, 'P-1001', 'A4 Fotokopi Kağıdı', '80 gram A4 fotokopi kağıdı',
           'C1 Ofis', 'M-1001', 'KAGIT', NULL, 50, 10, 125.00
    FROM Categories
    WHERE CategoryName = 'Ofis Malzemeleri';
END

IF NOT EXISTS (SELECT 1 FROM Products WHERE ProductCode = 'P-1002')
BEGIN
    INSERT INTO Products
    (
        CategoryId, ProductCode, ProductName, Description, Brand,
        ManufacturerCode, CustomCode1, ImageUrl, StockQuantity,
        CriticalStockLevel, Price
    )
    SELECT CategoryId, 'P-1002', 'Tükenmez Kalem', 'Mavi tükenmez kalem',
           'C1 Ofis', 'M-1002', 'KALEM', NULL, 5, 10, 18.50
    FROM Categories
    WHERE CategoryName = 'Ofis Malzemeleri';
END
GO

IF OBJECT_ID('ProductGridColumns', 'U') IS NULL
BEGIN
    CREATE TABLE ProductGridColumns
    (
        ProductGridColumnId INT IDENTITY(1,1) PRIMARY KEY,
        FieldName NVARCHAR(50) NOT NULL,
        DisplayName NVARCHAR(100) NOT NULL,
        SortOrder INT NOT NULL,
        RenderType NVARCHAR(30) NOT NULL,
        ColumnWidth INT NOT NULL DEFAULT 120,
        IsVisible BIT NOT NULL DEFAULT 1,
        ShowOnMobile BIT NOT NULL DEFAULT 1,
        CONSTRAINT UQ_ProductGridColumns_FieldName UNIQUE (FieldName)
    );
END
GO

IF NOT EXISTS (SELECT 1 FROM ProductGridColumns WHERE FieldName = 'ImageUrl')
BEGIN
    INSERT INTO ProductGridColumns
    (FieldName, DisplayName, SortOrder, RenderType, ColumnWidth, IsVisible, ShowOnMobile)
    VALUES
    ('ImageUrl', 'Görsel', 1, 'Image', 80, 1, 0),
    ('ProductCode', 'Ürün Kodu', 2, 'Text', 120, 1, 1),
    ('ProductName', 'Ürün Adı', 3, 'Text', 180, 1, 1),
    ('Brand', 'Marka', 4, 'Text', 120, 1, 1),
    ('StockQuantity', 'Stok Durumu', 5, 'StockStatus', 110, 1, 1),
    ('Price', 'Fiyat', 6, 'Price', 100, 1, 1),
    ('Quantity', 'Adet', 7, 'QuantityInput', 100, 1, 1),
    ('AddToCart', 'İşlem', 8, 'AddToCart', 130, 1, 1);
END
GO

IF OBJECT_ID('Carts', 'U') IS NULL
BEGIN
    CREATE TABLE Carts
    (
        CartId INT IDENTITY(1,1) PRIMARY KEY,
        UserId INT NOT NULL UNIQUE,
        CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
        UpdatedAt DATETIME2 NULL,
        CONSTRAINT FK_Carts_Users FOREIGN KEY (UserId) REFERENCES Users(UserId)
    );
END
GO

IF OBJECT_ID('CartItems', 'U') IS NULL
BEGIN
    CREATE TABLE CartItems
    (
        CartItemId INT IDENTITY(1,1) PRIMARY KEY,
        CartId INT NOT NULL,
        ProductId INT NOT NULL,
        Quantity INT NOT NULL,
        AddedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
        CONSTRAINT CK_CartItems_Quantity CHECK (Quantity > 0),
        CONSTRAINT UQ_CartItems_Cart_Product UNIQUE (CartId, ProductId),
        CONSTRAINT FK_CartItems_Carts FOREIGN KEY (CartId) REFERENCES Carts(CartId),
        CONSTRAINT FK_CartItems_Products FOREIGN KEY (ProductId) REFERENCES Products(ProductId)
    );
END
GO

IF OBJECT_ID('Orders', 'U') IS NULL
BEGIN
    CREATE TABLE Orders
    (
        OrderId INT IDENTITY(1,1) PRIMARY KEY,
        UserId INT NOT NULL,
        OrderNumber NVARCHAR(30) NOT NULL UNIQUE,
        OrderStatus NVARCHAR(30) NOT NULL DEFAULT 'Pending',
        TotalAmount DECIMAL(18,2) NOT NULL DEFAULT 0,
        ShippingAddress NVARCHAR(300) NULL,
        CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
        UpdatedAt DATETIME2 NULL,
        CONSTRAINT FK_Orders_Users FOREIGN KEY (UserId) REFERENCES Users(UserId)
    );
END
GO

IF OBJECT_ID('OrderItems', 'U') IS NULL
BEGIN
    CREATE TABLE OrderItems
    (
        OrderItemId INT IDENTITY(1,1) PRIMARY KEY,
        OrderId INT NOT NULL,
        ProductId INT NOT NULL,
        ProductCode NVARCHAR(50) NOT NULL,
        ProductName NVARCHAR(150) NOT NULL,
        UnitPrice DECIMAL(18,2) NOT NULL,
        Quantity INT NOT NULL,
        LineTotal DECIMAL(18,2) NOT NULL,
        CONSTRAINT CK_OrderItems_Quantity CHECK (Quantity > 0),
        CONSTRAINT UQ_OrderItems_Order_Product UNIQUE (OrderId, ProductId),
        CONSTRAINT FK_OrderItems_Orders FOREIGN KEY (OrderId) REFERENCES Orders(OrderId),
        CONSTRAINT FK_OrderItems_Products FOREIGN KEY (ProductId) REFERENCES Products(ProductId)
    );
END
GO

IF OBJECT_ID('StockMovements', 'U') IS NULL
BEGIN
    CREATE TABLE StockMovements
    (
        StockMovementId INT IDENTITY(1,1) PRIMARY KEY,
        ProductId INT NOT NULL,
        UserId INT NULL,
        MovementType NVARCHAR(30) NOT NULL,
        QuantityChange INT NOT NULL,
        OldQuantity INT NOT NULL,
        NewQuantity INT NOT NULL,
        Note NVARCHAR(250) NULL,
        CreatedAt DATETIME2 NOT NULL DEFAULT GETDATE(),
        CONSTRAINT FK_StockMovements_Products FOREIGN KEY (ProductId) REFERENCES Products(ProductId),
        CONSTRAINT FK_StockMovements_Users FOREIGN KEY (UserId) REFERENCES Users(UserId)
    );
END
GO
