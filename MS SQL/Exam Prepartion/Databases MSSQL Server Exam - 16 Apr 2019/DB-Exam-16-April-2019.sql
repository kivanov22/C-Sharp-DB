CREATE DATABASE [Airport]
GO

USE [Airport]
GO

CREATE TABLE [Planes](
[Id] INT PRIMARY KEY IDENTITY NOT NULL,
[Name] VARCHAR(30) NOT NULL,--? NVARCHAR
[Seats] INT NOT NULL,
[Range] INT NOT NULL
)

CREATE TABLE [Flights](
[Id] INT PRIMARY KEY IDENTITY NOT NULL,
[DepartureTime] DATETIME,
[ArrivalTime] DATETIME,
[Origin] VARCHAR(50) NOT NULL,
[Destination] VARCHAR(50) NOT NULL,
[PlaneId] INT FOREIGN KEY REFERENCES [Planes]([Id]) NOT NULL
)

CREATE TABLE [Passengers](
[Id] INT PRIMARY KEY IDENTITY NOT NULL,
[FirstName] VARCHAR(50) NOT NULL,
[LastName] VARCHAR(50) NOT NULL,
[Age] INT NOT NULL,
[Address] VARCHAR(50) NOT NULL,
[PassportId] CHAR(11) NOT NULL
)

CREATE TABLE [LuggageTypes](
[Id] INT PRIMARY KEY IDENTITY NOT NULL,
[Type] VARCHAR(50) NOT NULL
)

CREATE TABLE [Luggages](
[Id] INT PRIMARY KEY IDENTITY NOT NULL,
[LuggageTypeId] INT FOREIGN KEY REFERENCES [LuggageTypes]([Id]) NOT NULL,
[PassengerId] INT FOREIGN KEY REFERENCES [Passengers]([Id]) NOT NULL
)

CREATE TABLE [Tickets](
[Id] INT PRIMARY KEY IDENTITY NOT NULL,
[PassengerId] INT FOREIGN KEY REFERENCES [Passengers]([Id]) NOT NULL,
[FlightId] INT FOREIGN KEY REFERENCES [Flights]([Id]) NOT NULL,
[LuggageId] INT FOREIGN KEY REFERENCES [Luggages]([Id]) NOT NULL,
[Price] DECIMAL(18,2) NOT NULL
)

SELECT * FROM [Planes]

--2.INSERT
INSERT INTO [Planes]([Name],[Seats],[Range])
VALUES
('Airbus 336',112,5132),
('Airbus 330',432,5325),
('Boeing 369',231,2355),
('Stelt 297',254,2143),
('Boeing 338',165,5111),
('Airbus 558',387,1342),
('Boeing 128',345,5541)



INSERT INTO [LuggageTypes]([Type])
VALUES
('Crossbody Bag'),
('School Backpack'),
('Shoulder Bag')


--3.UPDATE
UPDATE Tickets
SET Price *= 1.13
WHERE FlightId IN  (SELECT [Id] 
						FROM [Flights]
						WHERE Destination= 'Carlsbad'
						)

--4.DELETE
--first delete from table connected to the one said in the document , then the given
DELETE FROM Tickets WHERE FlightId IN (SELECT [Id] 
										FROM Flights 
										WHERE Destination ='Ayn Halagim')

DELETE FROM Flights WHERE Destination = 'Ayn Halagim'

--5.	The "Tr" Planes

SELECT * FROM Planes
WHERE [Name] LIKE '%tr%'
ORDER BY [Id],[Name],[Seats],[Range]

--6.	Flight Profits
SELECT [FlightId],
		SUM(Price) AS [TotalPrice]
		FROM [Tickets] 
		GROUP BY FlightId
		ORDER BY [TotalPrice] DESC, [FlightId]

--7.	Passenger Trips
SELECT CONCAT([FirstName],' ', [LastName]) AS [Full Name],
		f.Origin,
		f.Destination
		FROM [Passengers] AS p
		JOIN Tickets AS t
		ON(p.Id = t.PassengerId)
		JOIN Flights AS f
		ON(t.FlightId = f.Id)
		ORDER BY [Full Name] , f.Origin, f.Destination

--8.	Non Adventures People
SELECT p.FirstName,
		p.LastName,
		p.Age
		FROM [Passengers] AS p
		LEFT JOIN Tickets AS t
		ON(p.Id=t.PassengerId)
		WHERE t.Id IS NULL
		ORDER BY p.Age DESC ,p.FirstName,p.LastName

--9.	Full Info
SELECT CONCAT(p.[FirstName],' ',p.LastName) AS [Full Name],
			pl.[Name] AS [Plane Name],
			CONCAT(f.Origin,' - ',f.Destination) AS [Trip],
			lgt.[Type] AS [Luggage Type]
		FROM Passengers AS p
		 JOIN  Tickets AS t
		 ON( p.Id = t.PassengerId)
		 JOIN Flights AS f
		 ON( t.FlightId = f.Id)
		 JOIN Planes AS pl
		 ON(f.PlaneId = pl.Id)
		 JOIN Luggages AS l
		 ON(t.LuggageId = l.Id)
		 JOIN LuggageTypes AS lgt
		 ON(l.LuggageTypeId = lgt.id)
		 ORDER BY [Full Name],[Plane Name],f.Origin,f.Destination,lgt.[Type]

--10.	PSP

SELECT p.[Name],
		p.Seats,
		COUNT(t.Id) AS [Passenger Count]
		FROM [Planes] AS p
		LEFT JOIN Flights AS f
		ON(p.Id = f.PlaneId)
		LEFT JOIN Tickets AS t
		ON(f.Id= t.FlightId)
		GROUP BY p.Id,p.[Name],p.Seats
		ORDER BY [Passenger Count] DESC, p.[Name],p.Seats
		GO
--11.	Vacation
CREATE FUNCTION udf_CalculateTickets(@origin VARCHAR(50), @destination VARCHAR(50), @peopleCount INT) 
RETURNS VARCHAR(50)
BEGIN
	DECLARE @totalPrice DECIMAL(6,2)

	IF @peopleCount <=0
	BEGIN
	RETURN 'Invalid people count!'
	END

	IF (NOT EXISTS (SELECT 1 FROM Flights WHERE Origin = @origin AND Destination = @destination))
        RETURN 'Invalid flight!'
    RETURN CONCAT('Total price ', 
    (SELECT TOP(1) ts.Price FROM Tickets AS ts 
    JOIN Flights AS f ON ts.FlightId = f.Id
    WHERE f.Origin = @origin AND f.Destination = @destination) * @peopleCount)
END
GO
--12.	Wrong Data
CREATE PROCEDURE usp_CancelFlights
AS 
BEGIN
			UPDATE Flights
			SET DepartureTime=NULL, ArrivalTime= NULL
			WHERE ArrivalTime > DepartureTime
END