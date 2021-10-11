CREATE DATABASE [WMS]
GO

USE [WMS]
GO

CREATE TABLE [Clients](
[ClientId] INT PRIMARY KEY IDENTITY NOT NULL,
[FirstName] VARCHAR(50) NOT NULL,
[LastName] VARCHAR(50) NOT NULL,
[Phone] CHAR(12) NOT NULL,
CHECK (LEN([Phone]) =  12)
)

CREATE TABLE [Mechanics](
[MechanicId] INT PRIMARY KEY IDENTITY NOT NULL,
[FirstName] VARCHAR(50) NOT NULL,
[LastName] VARCHAR(50) NOT NULL,
[Address] VARCHAR(255) NOT NULL
)

CREATE TABLE [Models](
[ModelId] INT PRIMARY KEY IDENTITY NOT NULL,
[Name] VARCHAR(50)  NOT NULL UNIQUE
)

CREATE TABLE [Jobs](
[JobId] INT PRIMARY KEY IDENTITY NOT NULL,
[ModelId] INT FOREIGN KEY REFERENCES [Models]([ModelId]) NOT NULL,
[Status] VARCHAR(11) NOT NULL DEFAULT('Pending'),
[ClientId] INT FOREIGN KEY REFERENCES [Clients]([ClientId]) NOT NULL,
[MechanicId] INT FOREIGN KEY REFERENCES [Mechanics]([MechanicId]),
[IssueDate] DATE NOT NULL,
[FinishDate] DATE,
CHECK ([Status] IN ('Pending','In Progress', 'Finished'))
)

CREATE TABLE [Orders](
[OrderId] INT PRIMARY KEY IDENTITY NOT NULL,
[JobId] INT FOREIGN KEY REFERENCES [Jobs]([JobId]) NOT NULL,
[IssueDate] DATE,
[Delivered] BIT NOT NULL DEFAULT(0) 
)

CREATE TABLE [Vendors](
[VendorId] INT PRIMARY KEY IDENTITY NOT NULL,
[Name] VARCHAR(50)  NOT NULL UNIQUE
)

CREATE TABLE [Parts](
[PartId] INT PRIMARY KEY IDENTITY NOT NULL,
[SerialNumber] VARCHAR(50) NOT NULL UNIQUE,
[Description] VARCHAR(255),
[Price] DECIMAL(6,2) NOT NULL,
[VendorId] INT FOREIGN KEY REFERENCES [Vendors]([VendorId]) NOT NULL,
[StockQty] INT NOT NULL DEFAULT(0),
CHECK ([Price] > 0),
CHECK ([StockQty] >= 0)
)

CREATE TABLE [OrderParts](
[OrderId] INT FOREIGN KEY REFERENCES [Orders]([OrderId]) NOT NULL,
[PartId] INT FOREIGN KEY REFERENCES [Parts]([PartId]) NOT NULL ,
[Quantity] INT NOT NULL DEFAULT(1),
PRIMARY KEY([OrderId],[PartId]),
CHECK ([Quantity] > 0)
)

CREATE TABLE [PartsNeeded](
[JobId] INT FOREIGN KEY REFERENCES [Jobs] ([JobId]) NOT NULL,
[PartId] INT FOREIGN KEY REFERENCES [Parts] ([PartId]) NOT NULL,
[Quantity] INT NOT NULL DEFAULT(1),
PRIMARY KEY ([JobId],[PartId]),
CHECK ([Quantity] > 0)
)


--INSERT
INSERT INTO [Clients]([FirstName], [LastName], [Phone])
VALUES
('Teri', 'Ennaco', '570-889-5187'),
('Merlyn', 'Lawler', '201-588-7810'),
('Georgene' ,'Montezuma'    ,'925-615-5185'),
('Jettie'   ,'Mconnell' ,'908-802-3564'),
('Lemuel' ,'Latzke  ','631-748-6479'),
('Melodie'  ,'Knipp',   '805-690-1682'),
('Candida'  ,'Corbley'  ,'908-275-8357')
 
INSERT INTO [Parts]([SerialNumber], [Description], [Price], [VendorId])
VALUES
('WP8182119','Door Boot Seal',117.86,2),
('W10780048','Suspension Rod',42.81,1),
('W10841140','Silicone Adhesive',6.77,4),
('WPY055980','High Temperature Adhesive',13.94,3)


SELECT * FROM Jobs

--UPDATE
UPDATE Jobs
SET MechanicId= 3 
WHERE [Status] = 'Pending'

UPDATE Jobs
SET [Status] = 'In Progress'
WHERE MechanicId = 3 AND [Status] = 'Pending'


--DELETE
DELETE FROM OrderParts
WHERE OrderId=19

DELETE FROM Orders
WHERE OrderId=19

--5.	Mechanic Assignments
SELECT CONCAT(m.[FirstName], ' ', m.LastName),
		j.[Status],
		j.[IssueDate]
		FROM [Jobs] AS j
		 JOIN Mechanics AS m
		ON(j.MechanicId = m.MechanicId)
		ORDER BY m.MechanicId, j.IssueDate,j.JobId

--6.	Current Clients
SELECT  CONCAT(c.FirstName, ' ', c.LastName) AS [Client],
		DATEDIFF(DAY,j.IssueDate,'04/24/2017') AS [Days going],
		j.Status 
		FROM Jobs AS j
		JOIN Clients AS c
		ON( j.ClientId = c.ClientId)
		WHERE j.Status <> 'Finished'

--7.	Mechanic Performance

	SELECT CONCAT(m.FirstName, ' ',m.LastName) AS [Mechanic],
		AVG(DATEDIFF(DAY,j.IssueDate,j.FinishDate)) AS [Average Days]
		FROM Jobs AS j
		JOIN Mechanics AS m
		ON(j.MechanicId = m.MechanicId)
		WHERE j.Status = 'Finished'
		GROUP BY m.MechanicId,m.FirstName,m.LastName,j.[Status]
		ORDER BY m.MechanicId

--8.	Available Mechanics
SELECT CONCAT([FirstName], ' ',[LastName]) AS [Available]
FROM Mechanics
WHERE MechanicId NOT IN (
						SELECT MechanicId FROM [Jobs]
						WHERE [Status] = 'In Progress'
						GROUP BY [MechanicId]
						)
--9.	Past Expenses
SELECT sq.JobId, SUM(sq.[TotalPrice]) AS [Total] 
FROM (
		SELECT j.[JobId],
		p.Price * op.Quantity AS [TotalPrice]
		FROM Parts AS p
	 JOIN OrderParts AS op
		ON(p.PartId = op.PartId)
	 JOIN Orders AS r
		ON(r.OrderId = op.OrderId)
	 JOIN Jobs AS j
		ON(j.JobId = r.JobId)
		WHERE j.[Status]='Finished'
) AS sq
GROUP BY sq.JobId
ORDER BY Total DESC,sq.JobId
	--mine missing records
SELECT sq.JobId, SUM(sq.[TotalPrice]) AS [Total] 
FROM (
		SELECT j.[JobId],
		p.Price * pn.Quantity AS [TotalPrice]
		FROM Jobs AS j
		JOIN PartsNeeded AS pn
		ON(j.JobId = pn.JobId)
		JOIN Parts AS p
		ON(p.PartId = pn.PartId)
		WHERE j.[Status]='Finished'
) AS sq
GROUP BY sq.JobId
ORDER BY Total DESC,sq.JobId
GO
--working

--10.	Missing Parts
--11.	Place Order


--12.	Cost Of Order
CREATE FUNCTION udf_GetCost (@jobId INT)
RETURNS DECIMAL(8,2)
AS
BEGIN
	DECLARE @totalCost DECIMAL(8,2)

	DECLARE @jobOrdersCount INT = (SELECT COUNT(OrderId) FROM Jobs AS j
									LEFT JOIN Orders AS o
									ON j.JobId=o.JobId
									WHERE j.JobId = @jobId
									)
	IF @jobOrdersCount = 0
	BEGIN
	RETURN 0
	END

	SET @totalCost = (SELECT SUM(p.Price * op.Quantity) FROM Jobs AS j
									LEFT JOIN Orders AS o
									ON j.JobId=o.JobId
									LEFT JOIN OrderParts op
									ON o.OrderId = op.OrderId
									LEFT JOIN Parts AS p
									ON op.PartId = p.PartId
									WHERE j.JobId = @jobId
									)
	RETURN @totalCost
END
GO

SELECT dbo.udf_GetCost (3)
GO