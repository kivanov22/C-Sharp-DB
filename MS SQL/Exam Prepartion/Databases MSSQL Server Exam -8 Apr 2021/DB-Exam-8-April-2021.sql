CREATE DATABASE [Service]
GO

USE [Service]
GO

CREATE TABLE [Users](
[Id] INT PRIMARY KEY IDENTITY NOT NULL,
[Username] VARCHAR(30) UNIQUE NOT NULL,
[Password] VARCHAR(50) NOT NULL,
[Name] VARCHAR(50),
[Birthdate] DATETIME,
[Age] INT,
[Email] VARCHAR(50) NOT NULL,
CHECK ([Age] BETWEEN 14 AND 110)
)

CREATE TABLE [Departments](
[Id] INT PRIMARY KEY IDENTITY NOT NULL,
[Name] VARCHAR(50) NOT NULL
)

CREATE TABLE [Employees](
[Id] INT PRIMARY KEY IDENTITY NOT NULL,
[FirstName] VARCHAR(25),
[LastName] VARCHAR(25),
[Birthdate] DATETIME,
[Age] INT,
[DepartmentId] INT FOREIGN KEY REFERENCES [Departments]([Id]),
CHECK ([Age] BETWEEN 18 AND 110)
)

CREATE TABLE [Categories](
[Id] INT PRIMARY KEY IDENTITY NOT NULL,
[Name] VARCHAR(50) NOT NULL,
[DepartmentId] INT FOREIGN KEY REFERENCES [Departments]([Id]) NOT NULL
)

CREATE TABLE [Status](
[Id] INT PRIMARY KEY IDENTITY NOT NULL,
[Label] VARCHAR(30) NOT NULL
)

CREATE TABLE [Reports](
[Id] INT PRIMARY KEY IDENTITY NOT NULL,
[CategoryId] INT FOREIGN KEY REFERENCES [Categories]([Id]) NOT NULL,
[StatusId] INT FOREIGN KEY REFERENCES [Status]([Id]) NOT NULL,
[OpenDate] DATETIME NOT NULL,
[CloseDate] DATETIME,
[Description] VARCHAR(200) NOT NULL,
[UserId] INT FOREIGN KEY REFERENCES [Users]([Id]),
[EmployeeId] INT FOREIGN KEY REFERENCES [Employees]([Id])
)

--2 INSERT
INSERT INTO [Employees](FirstName,LastName,Birthdate,DepartmentId)
VALUES
('Marlo','O''Malley','1958-9-21',1),
('Niki','Stanaghan','1969-11-26',4),
('Ayrton','Senna','1960-03-21',9),
('Ronnie','Peterson','1944-02-14',9),
('Giovanna','Amati','1959-07-20',5)

INSERT INTO [Reports](CategoryId,StatusId,OpenDate,CloseDate,[Description],UserId,EmployeeId)
VALUES 
(1,1,'2017-04-13',NULL,'Stuck Road on Str.133',6,2),
(6,3,'2015-09-05','2015-12-06','Charity trail running',3,5),
(14,2,'2015-09-07',NULL,'Falling bricks on Str.58',5,2),
(4,3,'2017-07-03','2017-07-06','Cut off streetlight on Str.11',1,1)

--3 UPDATE
UPDATE Reports
SET CloseDate = GETDATE()
WHERE CloseDate  IS NULL

--4 DELETE
DELETE FROM Reports
WHERE [StatusId]=4

--5.	Unassigned Reports
SELECT [Description],
		FORMAT([OpenDate],'dd-MM-yyyy')
		FROM Reports
		WHERE EmployeeId IS NULL
		ORDER BY OpenDate,[Description]

--6.	Reports & Categories
SELECT r.[Description],
		c.[Name] AS [CategoryName]
		FROM Reports AS r
		LEFT JOIN Categories AS c
		ON (r.CategoryId = c.Id)
		ORDER BY r.[Description],[CategoryName]

--7.	Most Reported Category
SELECT TOP(5) c.[Name] AS [CategoryName],
		COUNT(r.[Id]) AS [ReportNumber]
		FROM Reports AS r
		JOIN Categories AS c
		ON(c.Id = r.CategoryId)
		GROUP BY c.[Name]
		ORDER BY ReportNumber DESC,CategoryName

--8.	Birthday Report
SELECT u.[Username] AS [Username],
		c.[Name] AS [CategoryName]
		FROM Reports AS r
		JOIN Users AS u
		ON( r.UserId=u.Id )
		 JOIN Categories AS c
		ON(r.CategoryId = c.Id)
		WHERE DAY(u.Birthdate) = DAY(r.OpenDate)
		ORDER BY Username,CategoryName

--9.	Users per Employee 
SELECT  CONCAT(e.FirstName, ' ',e.LastName) AS [FullName],
		COUNT(u.Id) AS [UsersCount]
		FROM Employees AS e
		LEFT JOIN Reports As r
		ON e.Id = r.EmployeeId
		LEFT JOIN Users AS u
		ON u.Id = r.UserId
		GROUP BY e.FirstName,e.LastName
		ORDER BY UsersCount DESC, FullName

--10.	Full Info
SELECT CASE
		WHEN	COALESCE(e.FirstName,e.LastName) IS NOT NULL
		THEN CONCAT(e.FirstName, ' ',e.LastName) 
		ELSE 'None'
		END AS [Employee],
		ISNULL(d.[Name],'None') AS [Department],
		ISNULL (c.[Name],'None') AS [Category],
		ISNULL (r.[Description],'None'),
		ISNULL (FORMAT(r.OpenDate,'dd.MM.yyyy'),'None') AS [OpenDate],
		ISNULL (s.[Label],'None') AS [Status],
		ISNULL (u.[Name],'None') AS [User]
		FROM Reports AS r
		LEFT JOIN Employees AS e ON e.Id = r.EmployeeId
		LEFT JOIN Departments AS d ON d.Id = e.DepartmentId
		LEFT JOIN Categories AS c ON c.Id = r.CategoryId
		LEFT JOIN [Status] AS s ON s.Id = r.StatusId
		LEFT JOIN Users AS u ON u.Id = r.UserId
		ORDER BY e.[FirstName] DESC,e.[LastName] DESC,[Department],[Category],r.[Description],r.OpenDate,[Status],[User]

--11.	Hours to Complete


--12.	Assign Employee