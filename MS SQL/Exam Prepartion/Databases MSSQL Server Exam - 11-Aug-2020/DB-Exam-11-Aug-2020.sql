CREATE DATABASE [Bakery]
GO

USE [Bakery]
GO

CREATE TABLE [Countries](
[Id] INT PRIMARY KEY IDENTITY NOT NULL,
[Name] NVARCHAR(50) UNIQUE NOT NULL,--NOT NULL ?
)

CREATE TABLE [Customers](
[Id] INT PRIMARY KEY IDENTITY NOT NULL,
[FirstName] NVARCHAR(25) NOT NULL,
[LastName] NVARCHAR(25) NOT NULL,
[Gender] CHAR(1) CHECK([Gender] IN ('M','F')) NOT NULL,
[Age] INT,
[PhoneNumber] CHAR(10),--CHAR ?
[CountryId] INT FOREIGN KEY REFERENCES [Countries]([Id])
)

CREATE TABLE [Products](
[Id] INT PRIMARY KEY IDENTITY NOT NULL,
[Name] NVARCHAR(25) UNIQUE,
[Description] NVARCHAR(250),
[Recipe] NVARCHAR(MAX),
[Price] DECIMAL(18,2),
CHECK ([Price] > 0)
)

CREATE TABLE [Feedbacks](
[Id] INT PRIMARY KEY IDENTITY NOT NULL,
[Description] NVARCHAR(255),
[Rate] DECIMAL(18,2), --6??
[ProductId] INT FOREIGN KEY REFERENCES [Products]([Id]),
[CustomerId] INT FOREIGN KEY REFERENCES [Customers]([Id]),
CHECK ([Rate] BETWEEN 0 AND 10)--??
)

CREATE TABLE [Distributors](
[Id] INT PRIMARY KEY IDENTITY NOT NULL,
[Name] NVARCHAR(25) UNIQUE,
[AddressText] NVARCHAR(30),
[Summary] NVARCHAR(200),
[CountryId] INT FOREIGN KEY REFERENCES [Countries]([Id])
)

CREATE TABLE [Ingredients](
[Id] INT PRIMARY KEY IDENTITY NOT NULL,
[Name] NVARCHAR(30),
[Description] NVARCHAR(200),
[OriginCountryId] INT FOREIGN KEY REFERENCES [Countries]([Id]),
[DistributorId] INT FOREIGN KEY REFERENCES [Distributors]([Id])
)

CREATE TABLE [ProductsIngredients](
[ProductId] INT FOREIGN KEY REFERENCES [Products]([Id]) NOT NULL,
[IngredientId] INT FOREIGN KEY REFERENCES [Ingredients]([Id]) NOT NULL,
PRIMARY KEY([ProductId],[IngredientId])
)

--2 INSERT 
INSERT INTO Distributors([Name],CountryId,AddressText,Summary)
VALUES
('Deloitte & Touche',2,'6 Arch St #9757','Customizable neutral traveling'),
('Congress Title',13,'58 Hancock St','Customer loyalty'),
('Kitchen People',1,'3 E 31st St #77','Triple-buffered stable delivery'),
('General Color Co Inc',21,'6185 Bohn St #72','Focus group'),
('Beck Corporation',23,'21 E 64th Ave','Quality-focused 4th generation hardware')

INSERT INTO Customers(FirstName,LastName,Age,Gender,PhoneNumber,CountryId)
VALUES
('Francoise','Rautenstrauch',15,'M','0195698399',5),
('Kendra','Loud',22,'F','0063631526',11),
('Lourdes','Bauswell',50,'M','0139037043',8),
('Hannah','Edmison',18,'F','0043343686',1),
('Tom','Loeza',31,'M','0144876096',23),
('Queenie','Kramarczyk',30,'F','0064215793',29),
('Hiu','Portaro',25,'M','0068277755',16),
('Josefa','Opitz',43,'F','0197887645',17)

--3 UPDATE
SELECT * FROM Ingredients
WHERE [Name] IN('Bay Leaf','Paprika','Poppy')

UPDATE Ingredients
SET DistributorId = 35
WHERE [Name] IN('Bay Leaf','Paprika','Poppy')

UPDATE Ingredients
SET OriginCountryId = 14
WHERE OriginCountryId = 8

--4 DELETE
DELETE FROM Feedbacks
WHERE CustomerId=14 OR ProductId =5

--5.	Products by Price
