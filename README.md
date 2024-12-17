CREATE DATABASE BPN;
GO

USE BPN;
GO

CREATE TABLE Roles (
    RoleID INT PRIMARY KEY IDENTITY,
    RoleName NVARCHAR(50) NOT NULL UNIQUE
);

CREATE TABLE Client (
    UserID INT PRIMARY KEY IDENTITY,
    Login NVARCHAR(50) NOT NULL UNIQUE,
    PasswordHash NVARCHAR(255) NOT NULL,
    Email NVARCHAR(100) NOT NULL UNIQUE,
    RoleID INT FOREIGN KEY REFERENCES Roles(RoleID),
    DateJoined DATETIME DEFAULT GETDATE()
);

CREATE TABLE Categories (
    CategoryID INT PRIMARY KEY IDENTITY,
    CategoryName NVARCHAR(100) NOT NULL,
    Description NVARCHAR(255)
);


CREATE TABLE Products (
    ProductID INT PRIMARY KEY IDENTITY,
    CategoryID INT FOREIGN KEY REFERENCES Categories(CategoryID),
    ProductName NVARCHAR(100) NOT NULL,
    Description NVARCHAR(255),
    Price DECIMAL(10, 2) NOT NULL,
    StockQuantity INT DEFAULT 0
);

CREATE TABLE Orders (
    OrderID INT PRIMARY KEY IDENTITY,
    UserID INT FOREIGN KEY REFERENCES Client(UserID),
    OrderDate DATETIME DEFAULT GETDATE(),
    TotalAmount DECIMAL(10, 2) NOT NULL,
    Status NVARCHAR(20) DEFAULT 'Processing'
);

CREATE TABLE OrderItems (
    OrderItemID INT PRIMARY KEY IDENTITY,
    OrderID INT FOREIGN KEY REFERENCES Orders(OrderID),
    ProductID INT FOREIGN KEY REFERENCES Products(ProductID),
    Quantity INT NOT NULL,
    ItemPrice DECIMAL(10, 2) NOT NULL
);

CREATE TABLE CartItems (
    CartItemID INT PRIMARY KEY IDENTITY, -- Уникальный идентификатор элемента корзины
    ProductID INT NOT NULL, -- Идентификатор продукта
    ProductName NVARCHAR(100) NOT NULL, -- Название продукта
    Price DECIMAL(10, 2) NOT NULL, -- Цена продукта
    Quantity INT DEFAULT 1, -- Количество продукта
    UserID INT NOT NULL, -- Идентификатор пользователя, которому принадлежит элемент корзины
    FOREIGN KEY (UserID) REFERENCES Client(UserID) ON DELETE CASCADE, -- Связь с таблицей Client
    FOREIGN KEY (ProductID) REFERENCES Products(ProductID) ON DELETE CASCADE -- Связь с таблицей Products
);


INSERT INTO Roles (RoleName) VALUES 
('Customer'), 
('Admin');

INSERT INTO Client (Login, PasswordHash, Email, RoleID) VALUES
('vaska','pmWkWSBCL51Bfkhn79xPuKBKHz//H6B+mY6G9/eieuM=','v@s',2),
('catlover1', 'hashed_password1', 'catlover1@example.com', 1),
('admin', 'hashed_password2', 'admin@example.com', 2);

INSERT INTO Categories (CategoryName, Description) VALUES 
('Котики', 'Все о кошках и котятах'),
('Аксессуары', 'Аксессуары для питомцев'),
('Корм', 'Корма для кошек различных брендов');

INSERT INTO Products (CategoryID, ProductName, Description, Price, StockQuantity) VALUES 
(1, 'Британский котенок', 'Котенок британской породы', 15000.00, 5),
(3, 'Корм для котов Whiskas', 'Корм для котов, 1.5 кг', 400.00, 50);

INSERT INTO Orders (UserID, TotalAmount) VALUES 
(1, 15400.00);  

INSERT INTO OrderItems (OrderID, ProductID, Quantity, ItemPrice) VALUES 
(1, 1, 1, 15000.00),  
(1, 2, 5, 400.00);    



SELECT * FROM Roles;
SELECT * FROM Client;
SELECT * FROM Categories;
SELECT * FROM Products;
SELECT * FROM Orders;
SELECT * FROM OrderItems;
SELECT * FROM CartItems;


SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_NAME = 'CartItems';
