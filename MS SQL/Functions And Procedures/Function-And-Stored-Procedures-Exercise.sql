USE [SoftUni]

GO

--1.	Employees with Salary Above 35000
CREATE PROCEDURE usp_GetEmployeesSalaryAbove35000 
AS
BEGIN
	SELECT FirstName,
			LastName
		FROM [Employees]
		WHERE Salary > 35000
END

GO

EXEC usp_GetEmployeesSalaryAbove35000
GO


--2.	Employees with Salary Above Number
CREATE PROCEDURE usp_GetEmployeesSalaryAboveNumber @minSalary DECIMAL(18,4)
AS
BEGIN
	SELECT FirstName,
			LastName
		FROM [Employees]
		WHERE Salary >= @minSalary
END

GO

--3.	Town Names Starting With
CREATE PROCEDURE usp_GetTownsStartingWith @firstLetter CHAR(1)
AS 
BEGIN
	SELECT [Name]
			FROM Towns
			WHERE LEFT([Name],1)= @firstLetter
END
GO

--4.	Employees from Town
CREATE PROCEDURE usp_GetEmployeesFromTown @townName VARCHAR(50)
AS 
BEGIN
	SELECT e.[FirstName],
			e.[LastName]
			FROM Employees AS e
			LEFT JOIN Addresses AS a
			ON e.AddressID = a.AddressID
			LEFT JOIN [Towns] AS t
			ON a.TownID = t.TownID
			WHERE t.[Name] = @townName
END

EXEC usp_GetEmployeesFromTown 'Sofia'
GO

--5.	Salary Level Function
CREATE FUNCTION ufn_GetSalaryLevel(@salary DECIMAL(18,4))
RETURNS VARCHAR(7)
AS
BEGIN
	DECLARE @salaryLevel VARCHAR(7)
	
	IF @salary < 30000
	BEGIN
	--RETURN 'Low'
		SET @salaryLevel = 'Low'
	END
	ELSE IF @salary BETWEEN 30000 AND 50000
	BEGIN
	--RETURN 'Average'
		SET @salaryLevel = 'Average'
	END
	ELSE 
	BEGIN
	--RETURN 'High'
		SET @salaryLevel = 'High'
	END

	RETURN @salaryLevel
END

GO

SELECT [Salary],
		dbo.ufn_GetSalaryLevel(Salary)  AS [SalaryLevel]
		FROM Employees
		GO

--6.	Employees by Salary Level
CREATE PROCEDURE usp_EmployeesBySalaryLevel @salaryLevel VARCHAR(7)
AS
BEGIN
	SELECT [FirstName],
			[LastName]
	FROM Employees
	WHERE dbo.ufn_GetSalaryLevel(Salary) = @salaryLevel
END

GO

EXEC usp_EmployeesBySalaryLevel 'High'
GO

--7.	Define Function
CREATE FUNCTION ufn_IsWordComprised(@setOfLetters NVARCHAR(20), @word NVARCHAR(20)) 
RETURNS BIT
AS 
BEGIN
				SET @setOfLetters = LOWER(@setOfLetters)
				SET @word = LOWER(@word)
				DECLARE @CurrentIndex INT
				SET @CurrentIndex = 1
		WHILE (@CurrentIndex <= LEN(@word))
			BEGIN
					DECLARE @CurrentChar CHAR
					SET @CurrentChar = SUBSTRING(@word,@CurrentIndex,1 )
			IF (CHARINDEX(@CurrentChar,@setOfLetters) = 0)
				BEGIN
						RETURN 0
				END
					SET @CurrentIndex += 1
			END
		RETURN 1
END
GO

--8.	* Delete Employees and Departments
CREATE PROCEDURE usp_DeleteEmployeesFromDepartment (@departmentId INT) 
AS 
BEGIN 
		DELETE FROM [EmployeesProjects]
		WHERE [EmployeeID] IN (
				SELECT [EmployeeID]
				FROM [Employees]
				WHERE [DepartmentID] = @departmentId
				)

		UPDATE [Employees]
		SET [ManagerID] = NULL
		WHERE [ManagerID] IN (
					SELECT [EmployeeID]
				FROM [Employees]
				WHERE [DepartmentID] = @departmentId
				)
--We need to alter ManagerId column from Departments in order to be nullable because we need to set 
--ManagerID NULL of all Departments that have their Manager lately deleted
		ALTER TABLE [Departments]
		ALTER COLUMN [ManagerID] INT NULL

		UPDATE [Departments]
		SET [ManagerID] = NULL
		WHERE [ManagerID] IN (
				SELECT [EmployeeID]
				FROM [Employees]
				WHERE [DepartmentID] = @departmentId
				)

		DELETE FROM [Employees]
		WHERE DepartmentID = @departmentId

		DELETE FROM [Departments]
		WHERE DepartmentID = @departmentId

		SELECT COUNT(*)
		FROM [Employees]
		WHERE [DepartmentID] = @departmentId
END

EXEC usp_DeleteEmployeesFromDepartment 4
GO

--9.	Find Full Name
USE [Bank]
GO

CREATE PROCEDURE usp_GetHoldersFullName 
AS
BEGIN
		SELECT CONCAT([FirstName], ' ', [LastName] ) AS [Full Name]
				FROM [AccountHolders] AS ach
				LEFT JOIN Accounts AS ac
				ON ach.Id = ac.Id
END

EXEC usp_GetHoldersFullName
GO

--10.	People with Balance Higher Than
CREATE PROCEDURE usp_GetHoldersWithBalanceHigherThan @number INT
AS
BEGIN
		SELECT [FirstName],
				[LastName]
				FROM [Accounts] ac
				 JOIN AccountHolders as ach
				ON ac.Id = ach.Id
				WHERE Balance > @number
				ORDER BY FirstName,LastName
END

EXEC usp_GetHoldersWithBalanceHigherThan 50
GO

--11.	Future Value Function

CREATE FUNCTION ufn_CalculateFutureValue(@sum DECIMAL, @yearlyInterestRate FLOAT,  @numberOfYears INT)
RETURNS DECIMAL(4,4)
AS 
BEGIN 
	DECLARE @totalSum DECIMAL(4,4)

	@totalSum = @sum * (1 + @yearlyInterestRate * @numberOfYears)


	RETURN @totalSum 
END
