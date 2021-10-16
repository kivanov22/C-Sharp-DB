CREATE DATABASE [CigarShop]
GO

USE [CigarShop]
GO

CREATE TABLE [Sizes](
[Id] INT PRIMARY KEY IDENTITY,
[Length] INT CHECK([Length] BETWEEN 10 AND 25) NOT NULL,
[RingRange] DECIMAL(18,2) CHECK([RingRange] BETWEEN 1.5 AND 7.5) NOT NULL
)

CREATE TABLE [Tastes](
[Id] INT PRIMARY KEY IDENTITY,
[TasteType] VARCHAR(20) NOT NULL,
[TasteStrength] VARCHAR(15) NOT NULL,
[ImageUrl] NVARCHAR(100) NOT NULL
)

CREATE TABLE [Brands](
[Id] INT PRIMARY KEY IDENTITY,
[BrandName] VARCHAR(30) NOT NULL UNIQUE,
[BrandDescription] VARCHAR(MAX)
)

CREATE TABLE [Cigars](
[Id] INT PRIMARY KEY IDENTITY,
[CigarName] VARCHAR(80) NOT NULL,
[BrandId] INT FOREIGN KEY REFERENCES [Brands]([Id]) NOT NULL,
[TastId] INT FOREIGN KEY REFERENCES [Tastes]([Id]) NOT NULL,
[SizeId] INT FOREIGN KEY REFERENCES [Sizes]([Id]) NOT NULL,
[PriceForSingleCigar] DECIMAL(18,2) NOT NULL,--?? less
[ImageURL] NVARCHAR(100) NOT NULL
)

CREATE TABLE [Addresses](
[Id] INT PRIMARY KEY IDENTITY,
[Town] VARCHAR(30) NOT NULL,
[Country] NVARCHAR(30) NOT NULL,
[Streat] NVARCHAR(100) NOT NULL,--??name
[ZIP] VARCHAR(20) NOT NULL
)

CREATE TABLE [Clients](
[Id] INT PRIMARY KEY IDENTITY,
[FirstName] NVARCHAR(30) NOT NULL,
[LastName] NVARCHAR(30) NOT NULL,
[Email] NVARCHAR(50) NOT NULL,
[AddressId] INT FOREIGN KEY REFERENCES [Addresses]([Id]) NOT NULL
)

CREATE TABLE [ClientsCigars](
[ClientId] INT FOREIGN KEY REFERENCES [Clients]([Id]) NOT NULL,
[CigarId] INT FOREIGN KEY REFERENCES [Cigars]([Id]) NOT NULL,
PRIMARY KEY([ClientId],[CigarId])
)


--2 INSERT 

INSERT INTO [Cigars](CigarName,BrandId,TastId,SizeId,PriceForSingleCigar,ImageURL)
VALUES
('COHIBA ROBUSTO',9,1,5,15.50,'cohiba-robusto-stick_18.jpg'),
('COHIBA SIGLO I',9,1,10,410.00,'cohiba-siglo-i-stick_12.jpg'),
('HOYO DE MONTERREY LE HOYO DU MAIRE',14,5,11,7.50,'hoyo-du-maire-stick_17.jpg'),
('HOYO DE MONTERREY LE HOYO DE SAN JUAN',14,4,15,32.00,'hoyo-de-san-juan-stick_20.jpg'),
('TRINIDAD COLONIALES',2,3,8,85.21,'trinidad-coloniales-stick_30.jpg')

INSERT INTO Addresses(Town,Country,Streat,ZIP)
VALUES
('Sofia','Bulgaria','18 Bul. Vasil levski','1000'),
('Athens','Greece','4342 McDonald Avenue','10435'),
('Zagreb','Croatia','4333 Lauren Drive','10000')

SELECT * FROM Tastes
--UPDATE
UPDATE Cigars
SET PriceForSingleCigar =PriceForSingleCigar * 1.20
WHERE TastId=1

UPDATE Brands
SET BrandDescription='New description'
WHERE BrandDescription IS NULL

--DELETE

DELETE FROM Clients
WHERE AddressId IN (7,8,10)

DELETE FROM Addresses
WHERE Country LIKE 'C%'

SELECT * FROM Addresses
WHERE Country LIKE 'C%'

--5.	Cigars by Price
SELECT [CigarName],
		[PriceForSingleCigar],
		[ImageURL]
		FROM Cigars 
		ORDER BY PriceForSingleCigar,CigarName DESC
		
--6.	Cigars by Taste
SELECT c.Id,
		c.CigarName,
		c.PriceForSingleCigar,
		t.TasteType,
		t.TasteStrength
		FROM Tastes AS t
		 JOIN Cigars AS c
		ON t.Id= c.TastId
		WHERE t.TasteType IN('Earthy','Woody')
		ORDER BY c.PriceForSingleCigar DESC
		
--7.	Clients without Cigars
SELECT c.Id,
		CONCAT(c.FirstName,' ',c.LastName) AS [ClientName],
		c.Email
		FROM ClientsCigars AS cc
		RIGHT JOIN Clients AS c
		ON cc.ClientId= c.Id
		WHERE cc.ClientId IS NULL
		ORDER BY [ClientName]

--8.	First 5 Cigars
SELECT TOP(5) c.CigarName,
		c.PriceForSingleCigar,
		c.ImageURL
		FROM Cigars AS c
		LEFT JOIN Sizes AS s
		ON c.SizeId = s.Id 
		WHERE s.[Length] >= 12 AND (c.CigarName LIKE '%ci%' OR c.[PriceForSingleCigar] > 50) AND s.[RingRange] >2.55
		ORDER BY c.CigarName,c.PriceForSingleCigar DESC


--9.	Clients with ZIP Codes

SELECT CONCAT(c.FirstName,' ',c.LastName) AS [FullName],
		a.Country,
		a.ZIP,
		CONCAT('$',MAX(ci.PriceForSingleCigar)) AS [CigarPrice]
		FROM Clients AS c
		 JOIN Addresses AS a ON c.AddressId = a.Id 
		 JOIN ClientsCigars AS cc ON c.Id = cc.ClientId 
		 JOIN Cigars AS ci ON  cc.CigarId= ci.Id
		WHERE ZIP NOT LIKE '%[^0-9]%' 
		GROUP BY a.ZIP,a.Country,c.FirstName,c.LastName
		ORDER BY [FullName]

--10.	Cigars by Size
SELECT c.LastName,
		AVG(sz.[Length]) AS [CigarLenght],
		CEILING(AVG(sz.RingRange)) AS [CigarRingRange]
		FROM Clients AS c
		 JOIN ClientsCigars AS cc ON c.Id= cc.ClientId
		 JOIN Cigars AS cig ON cig.Id = cc.CigarId
		 JOIN Sizes AS sz ON sz.Id = cig.SizeId
		GROUP BY c.LastName
		ORDER BY [CigarLenght] DESC
		GO
--11.	Client with Cigars
CREATE FUNCTION udf_ClientWithCigars(@name VARCHAR(20))
RETURNS INT
AS
BEGIN
	DECLARE @count INT=(SELECT	
							COUNT(ci.CigarId)
							FROM Clients AS c
							JOIN ClientsCigars AS ci ON c.Id = ci.ClientId
							WHERE c.FirstName = @name
							)

	RETURN @count
END
GO

SELECT dbo.udf_ClientWithCigars('Betty')
GO



--12.	Search for Cigar with Specific Taste
CREATE PROCEDURE usp_SearchByTaste(@taste VARCHAR(20))
AS
BEGIN
			SELECT c.CigarName,
							CONCAT('$', c.PriceForSingleCigar) AS [Price],
							t.TasteType,
							b.BrandName,
							CONCAT(sz.[Length],' cm') AS [CigarLength],
							CONCAT(sz.RingRange,' cm') AS [CigarRingRange]
							FROM Cigars AS c
							JOIN Brands AS b ON c.BrandId = b.Id
							JOIN Tastes AS t ON t.Id = c.TastId
							JOIN Sizes AS sz ON c.SizeId = sz.Id
							WHERE t.TasteType = @taste
							ORDER BY [CigarLength],[CigarRingRange] DESC
							
							
END

GO
EXEC usp_SearchByTaste 'Woody'