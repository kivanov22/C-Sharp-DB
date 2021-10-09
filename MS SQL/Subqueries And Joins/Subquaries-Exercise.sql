USE [SoftUni]

--01.Employee Address
SELECT TOP(5) e.[EmployeeID],
		e.[JobTitle],
		e.[AddressID],
		a.[AddressText]  
FROM [Employees] AS e
		LEFT JOIN[Addresses] AS a
		ON e.AddressID = a.AddressID
ORDER BY e.[AddressID]

--2.Addresses with Towns
SELECT TOP(50)
		e.FirstName,
		e.LastName,
		t.[Name] AS Town,
		a.AddressText
FROM [Employees] AS e
		 JOIN [Addresses] AS a
		 JOIN [Towns] AS t 
		ON a.TownID= t.TownID
		ON e.[AddressID] = a.[AddressID]
ORDER BY e.[FirstName],e.[LastName]

--3.Sales Employee
SELECT	e.EmployeeID,
		e.FirstName,
		e.LastName,
		d.[Name] AS [DepartmentName]
FROM [Employees] AS e
		INNER JOIN [Departments] AS d
		ON e.DepartmentID = d.DepartmentID
WHERE d.[Name]='Sales'
ORDER BY e.EmployeeID 

--4.Employee Departments
SELECT TOP(5) e.EmployeeID,
		e.FirstName,
		e.Salary,
		d.[Name] AS [DepartmentName]
FROM [Employees] AS e
		LEFT JOIN [Departments] AS d
		ON  d.[DepartmentID] = e.[DepartmentID]
WHERE e.Salary>15000
ORDER BY e.DepartmentID 

--5.Employees Without Project
SELECT TOP(3)	e.EmployeeID,
		e.FirstName
		FROM [Employees] AS e
		LEFT JOIN [EmployeesProjects] AS ep
		ON e.EmployeeID = ep.EmployeeID
		WHERE ep.EmployeeID IS NULL
		ORDER BY e.EmployeeID

--6.	Employees Hired After
SELECT	e.FirstName,
		e.LastName,
		e.HireDate,
		d.[Name]
FROM [Employees] AS e
		JOIN [Departments] AS d
		ON e.DepartmentID = d.DepartmentID AND d.[Name] IN('Sales','Finance')
WHERE e.HireDate > '01/01/1999'
ORDER BY e.HireDate

--7.Employees with Project
SELECT TOP(5)	e.[EmployeeID],
				e.[FirstName],
				p.[Name] AS [ProjectName]
FROM [Employees] AS e
		INNER JOIN [EmployeesProjects] AS ep 
		ON e.EmployeeID = ep.EmployeeID
		INNER JOIN [Projects] AS p
		ON ep.ProjectID = p.ProjectID
WHERE p.[StartDate] > '08/13/2002' AND p.[EndDate] IS NULL
ORDER BY e.[EmployeeID]

--8.Employee 24
SELECT	e.EmployeeID,
		e.FirstName,
		CASE 
			WHEN	DATEPART(YEAR,p.StartDate) >=2005 THEN NULL
			ELSE p.[Name]
		END
FROM [Employees] AS e
		INNER JOIN [EmployeesProjects] AS ep
		ON e.EmployeeID = ep.EmployeeID
		INNER JOIN [Projects] AS p
		ON ep.ProjectID=p.ProjectID
WHERE e.EmployeeID=24

--9.Employee Manager
SELECT	e.EmployeeID,
		e.FirstName,
		e.ManagerID,
		m.FirstName AS [ManagerName]
FROM [Employees] AS e
		 JOIN [Employees] AS m
		ON e.ManagerID = m.EmployeeID
WHERE e.ManagerID IN (3,7)
ORDER BY e.EmployeeID

--10. Employee Summary
SELECT TOP(50) e.EmployeeID,
		CONCAT(e.FirstName,' ',e.LastName) AS EmployeeName,
		CONCAT(m.FirstName,' ',m.LastName) AS ManagerName,
		d.[Name] AS DepartmentName
		FROM [Employees] AS e
		INNER JOIN [Employees] AS m
		ON m.EmployeeID=e.ManagerID
		INNER JOIN [Departments] AS d 
		ON d.DepartmentID=e.DepartmentID
ORDER BY e.EmployeeID

--11. Min Average Salary
SELECT TOP(1) AVG(Salary) AS AverageSalary
		FROM Employees AS e
		JOIN Departments AS d
		ON d.DepartmentID = e.DepartmentID
		GROUP BY e.DepartmentID
		ORDER BY AverageSalary
--12. Highest Peaks in Bulgaria
USE Geography

	SELECT  c.CountryCode,
			m.MountainRange,
			p.PeakName,
			p.Elevation
			FROM [Peaks] AS p
			LEFT JOIN [Mountains] AS m
			ON p.MountainId = m.Id
			LEFT JOIN [MountainsCountries] AS mc
			ON m.Id = mc.MountainId
			INNER JOIN [Countries] AS c
			ON mc.CountryCode = c.CountryCode
			WHERE c.CountryCode = 'BG' AND p.Elevation > 2835
			ORDER BY p.Elevation DESC

--13. Count Mountain Ranges
SELECT  c.CountryCode,
		COUNT(mc.MountainId) AS MountainRanges
		FROM Countries AS c
		LEFT JOIN MountainsCountries AS mc
		ON c.CountryCode = mc.CountryCode
		WHERE c.CountryCode IN ('BG','RU','US')
		GROUP BY c.CountryCode

--14. Countries with Rivers
SELECT TOP(5)	c.CountryName,
		r.RiverName
		FROM Countries AS c
		LEFT JOIN CountriesRivers AS cr
		ON c.CountryCode = cr.CountryCode
		LEFT JOIN Rivers AS r 
		ON r.Id = cr.RiverId  
		WHERE c.ContinentCode = 'AF'
		ORDER BY c.CountryName 

--15. *Continents and Currencies
SELECT	[ContinentCode],[CurrencyCode], COUNT([CurrencyCode]) AS [CurrencyCount]
		FROM [Countries]
		GROUP BY [ContinentCode],[CurrencyCode]


--16.Countries Without Any Mountains
SELECT COUNT(*) AS Count
	FROM Countries AS c
	LEFT JOIN MountainsCountries AS mc ON mc.CountryCode = c.CountryCode 
	WHERE mc.MountainId IS NULL

--17. Highest Peak and Longest River by Country
SELECT TOP(5) CountryName, MAX(p.Elevation) AS HighestPeak, MAX(r.Length) AS LongestRiver
	FROM Countries AS c
	LEFT JOIN MountainsCountries AS mc ON mc.CountryCode = c.CountryCode
	LEFT JOIN Mountains AS m ON m.Id = mc.MountainId
	LEFT JOIN Peaks AS p ON p.MountainId = m.Id
	LEFT JOIN CountriesRivers AS cr ON c.CountryCode = cr.CountryCode
	LEFT JOIN Rivers AS r ON cr.RiverId = r.Id
GROUP BY CountryName
ORDER BY HighestPeak DESC, LongestRiver DESC, CountryName

--18
SELECT TOP(5) k.CountryName,k.PeakName, k.HighestPeak, k.MountainRange
		FROM
	(SELECT CountryName, 
		ISNULL(p.PeakName, '(no highest peak)') AS PeakName,
		ISNULL(m.MountainRange, '(no mountain)') AS MountainRange,
		ISNULL(MAX(p.Elevation),0) AS HighestPeak,
		DENSE_RANK() OVER (PARTITION BY CountryName ORDER BY MAX(p.Elevation) DESC) AS Ranked
		FROM Countries AS c
		LEFT JOIN MountainsCountries AS mc ON mc.CountryCode = c.CountryCode
		LEFT JOIN Mountains AS m ON m.Id = mc.MountainId
		LEFT JOIN Peaks AS p ON p.MountainId = m.Id
	GROUP BY CountryName, p.PeakName, m.MountainRange) AS k
	WHERE Ranked = 1
	ORDER BY CountryName,PeakName