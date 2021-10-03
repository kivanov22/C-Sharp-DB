USE [SoftUni]

--1) Problem 1.Find Names of All Employees by First Name
SELECT [FirstName],[LastName] FROM [Employees]
WHERE LEFT([FirstName] ,2) = 'Sa'

--2)Problem 2.Find Names of All employees by Last Name 
SELECT [FirstName],[LastName] FROM [Employees]
WHERE [LastName] LIKE '%ei%'

--3) Problem 3.	Find First Names of All Employees
SELECT [FirstName] FROM [Employees]
WHERE [DepartmentID] IN (3,10) AND YEAR([HireDate]) BETWEEN 1995 AND 2005

--4)Problem 4.	Find All Employees Except Engineers 
SELECT [FirstName],[LastName] FROM [Employees]
WHERE NOT [JobTitle] LIKE '%engineer%'

--5)Problem 5.	Find Towns with Name Length
SELECT [Name]
FROM [Towns]
WHERE LEN([Name]) IN (5,6)
ORDER BY [Name]

--6)Problem 6.	 Find Towns Starting With
SELECT * FROM [Towns]
WHERE LEFT ([Name],1) IN ('M','K','B','E')
ORDER BY [Name] 

--7)Problem 7.	 Find Towns Not Starting With
SELECT * FROM [Towns]
WHERE NOT LEFT ([Name],1) IN ('R','B','D')
ORDER BY [Name] 

--08) Problem 8.	Create View Employees Hired After 2000 Year
CREATE VIEW [V_EmployeesHiredAfter2000] AS
SELECT [FirstName],[LastName]
FROM [Employees]
WHERE YEAR([HireDate]) > 2000

--09) Problem 9.	Length of Last Name
SELECT [FirstName],[LastName] FROM [Employees]
WHERE LEN([LastName]) IN (5)

--10)Problem 10.Rank Employees by Salary
SELECT [EmployeeID],[FirstName],[LastName],[Salary],
DENSE_RANK() OVER(PARTITION BY [Salary] ORDER BY [EmployeeID]) AS [Rank]
FROM [Employees]
WHERE [Salary] BETWEEN 10000 AND 50000
ORDER BY [Salary] DESC

--11)Problem 11.Find All Employees with Rank 2 *
SELECT * FROM (
        SELECT [EmployeeID],[FirstName],[LastName],[Salary],
        DENSE_RANK() OVER(PARTITION BY [Salary] ORDER BY [EmployeeID]) AS [Rank]
        FROM [Employees]
		WHERE [Salary] BETWEEN 10000 AND 50000
)AS [RankingTable]
WHERE [Rank] = 2
ORDER BY [Salary] DESC

--12)Problem 12.Countries Holding ‘A’ 3 or More Times
USE [Geography]

SELECT [CountryName] AS [Country Name],
		[IsoCode] AS [ISO Code]
FROM [Countries]
WHERE [CountryName] LIKE '%a%a%a%'
ORDER BY [ISO Code]

--OR
SELECT [CountryName] , [IsoCode] FROM [Countries]
WHERE LEN ([CountryName]) - LEN(REPLACE([CountryName] , 'a','')) >=3
ORDER BY [IsoCode]

--13)Problem 13. Mix of Peak and River Names
SELECT p.[PeakName],
		r.[RiverName],
				LOWER(CONCAT(LEFT(p.[PeakName], LEN(p.[PeakName])-1),r.[RiverName])) AS [Mix]
		FROM [Peaks] AS p,
	     [Rivers] AS r
		 WHERE LOWER(RIGHT(p.[PeakName],1)) = LOWER(LEFT(r.[Rivername],1))
		 ORDER BY [Mix]


--14)Problem 14.Games from 2011 and 2012 year

USE [Diablo]

SELECT TOP(50) [Name],FORMAT([Start],'yyyy-MM-dd') AS [Start]
FROM [Games]
WHERE DATEPART(YEAR,[Start]) BETWEEN 2011 AND 2012
ORDER BY [Start]

--15) Problem 15.User Email Providers
SELECT [Username],SUBSTRING([Email],CHARINDEX('@',[Email]) +1 , LEN([Email])) AS [EmailProvider]
FROM [Users]
ORDER BY [EmailProvider],[Username]

--16)Problem 16. Get Users with IPAdress Like Pattern
SELECT [Username],[IpAddress]
FROM [Users]
WHERE [IpAddress] LIKE '___.1%.%.___' 
ORDER BY [Username]

--17)Problem 17. Show All Games with Duration and Part of the Day

SELECT [Name],
	CASE
		WHEN DATEPART(HOUR,[Start]) BETWEEN 0 AND 11 THEN 'Morning'
		WHEN DATEPART(HOUR, [Start]) BETWEEN 12 AND 17 THEN 'Afternoon'
		WHEN DATEPART(HOUR, [Start]) BETWEEN 18 AND 23 THEN 'Evening'
	END AS [Part of the Day],
	CASE 
		WHEN [Duration] <= 3 THEN 'Extra Short'
		WHEN [Duration] BETWEEN 4 AND 6 THEN 'Short'
		WHEN [Duration] > 6 THEN 'Long'
		ELSE 'Extra Long'
	END AS [Duration]
	FROM [Games]
	ORDER BY [Name],[Duration]

--18)
SELECT [ProductName], [OrderDate],
	DATEADD(DAY, 3, [OrderDate]) AS [Pay Due],
	DATEADD(MONTH, 1, [OrderDate]) AS [Deliver Due]
FROM [Orders]

--19)
--Problem 19. People Table
CREATE TABLE People
(
Id INT IDENTITY,
[Name] VARCHAR(50) NOT NULL,
[BirthDate] DATETIME NOT NULL
)
INSERT INTO People VALUES
('Victor', '2000-12-07 00:00:00.000'),
('Steven', '1992-09-10 00:00:00.000'),
('Stephen', '1910-09-19 00:00:00.000'),
('John', '2010-01-06 00:00:00.000')
SELECT [Name],
DATEDIFF(YEAR, BirthDate, GETDATE()) AS 'Age in Years',
DATEDIFF(MONTH, BirthDate, GETDATE()) AS 'Age in Months',
DATEDIFF(DAY, BirthDate, GETDATE()) AS [Age in Days],
DATEDIFF(MINUTE, BirthDate, GETDATE()) AS [Age in Minutes]
FROM People