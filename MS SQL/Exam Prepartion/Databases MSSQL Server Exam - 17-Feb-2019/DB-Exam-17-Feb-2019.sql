CREATE DATABASE [School]
GO

USE [School]
GO

CREATE TABLE [Students](
[Id] INT PRIMARY KEY IDENTITY NOT NULL,
[FirstName] NVARCHAR(30) NOT NULL,
[MiddleName] NVARCHAR(25),
[LastName] NVARCHAR(30) NOT NULL,
[Age] INT CHECK([Age] > 0),
[Address] NVARCHAR(50),
[Phone] NVARCHAR(10)
)

CREATE TABLE [Subjects](
[Id] INT PRIMARY KEY IDENTITY NOT NULL,
[Name] NVARCHAR(20) NOT NULL,
[Lessons] INT CHECK([Lessons] > 0) NOT NULL
)

CREATE TABLE [StudentsSubjects](
[Id] INT PRIMARY KEY IDENTITY NOT NULL,
[StudentId] INT FOREIGN KEY REFERENCES [Students]([Id]) NOT NULL,
[SubjectId] INT FOREIGN KEY REFERENCES [Subjects]([Id]) NOT NULL,
[Grade] DECIMAL(18,2) CHECK([Grade] BETWEEN 2 AND 6) NOT NULL
)

CREATE TABLE [Exams](
[Id] INT PRIMARY KEY IDENTITY NOT NULL,
[Date] DATETIME,
[SubjectId] INT FOREIGN KEY REFERENCES [Subjects]([Id]) NOT NULL
)

CREATE TABLE [StudentsExams](
[StudentId] INT FOREIGN KEY REFERENCES [Students]([Id]) NOT NULL,
[ExamId] INT FOREIGN KEY REFERENCES [Exams]([Id]) NOT NULL,
[Grade] DECIMAL(18,2) CHECK([Grade] BETWEEN 2 AND 6) NOT NULL
PRIMARY KEY([StudentId],[ExamId])
)

CREATE TABLE [Teachers](
[Id] INT PRIMARY KEY IDENTITY NOT NULL,
[FirstName] NVARCHAR(20) NOT NULL,
[LastName] NVARCHAR(20) NOT NULL,
[Address] NVARCHAR(20) NOT NULL,
[Phone] VARCHAR(10),--?
[SubjectId] INT FOREIGN KEY REFERENCES [Subjects]([Id]) NOT NULL
)

CREATE TABLE [StudentsTeachers](
[StudentId] INT FOREIGN KEY REFERENCES [Students]([Id]) NOT NULL,
[TeacherId] INT FOREIGN KEY REFERENCES [Teachers]([Id]) NOT NULL,
PRIMARY KEY([StudentId],[TeacherId])
)

--2 INSERT
INSERT INTO [Teachers](FirstName,LastName,[Address],Phone,SubjectId)
VALUES
('Ruthanne','Bamb','84948 Mesta Junction','3105500146',6),
('Gerrard','Lowin','370 Talisman Plaza','3324874824',2),
('Merrile','Lambdin','81 Dahle Plaza','4373065154',5),
('Bert','Ivie','2 Gateway Circle','4409584510',4)


INSERT INTO [Subjects]([Name],Lessons)
VALUES
('Geometry',12),
('Health',10),
('Drama',7),
('Sports',9)
--3 UPDATE
UPDATE StudentsSubjects
SET Grade=6.00
WHERE SubjectId IN(1,2) AND Grade >=5.50

--4 DELETE
SELECT * FROM Teachers
WHERE Phone LIKE '%72%'

DELETE FROM StudentsTeachers
WHERE TeacherId IN(7,12,15,18,24,26)

DELETE FROM Teachers
WHERE Phone LIKE '%72%'

--5. Teen Students
SELECT FirstName,LastName,Age
		FROM [Students]
	WHERE Age >=12
ORDER BY FirstName,LastName

--6. Students Teachers
SELECT s.[FirstName],
		s.[LastName],
		COUNT(t.Id)  AS [TeachersCount]
		FROM StudentsTeachers AS st
		JOIN Students AS s
		ON st.StudentId = s.Id
		JOIN Teachers AS t
		ON st.TeacherId= t.Id
		GROUP BY s.FirstName,s.LastName

--7. Students to Go
SELECT CONCAT(st.FirstName,' ',st.LastName) AS [Full Name]
		FROM Students AS st
		LEFT JOIN StudentsExams AS se
		ON st.Id= se.StudentId
		WHERE se.StudentId IS NULL
		ORDER BY [Full Name]

--8. Top Students
SELECT TOP(10) st.FirstName,
		st.LastName,
		CAST(AVG(se.Grade) AS DECIMAL(3,2)) AS [Grade]
		FROM StudentsExams AS se
		JOIN Students AS st
		ON st.Id = se.StudentId
		GROUP BY se.StudentId, st.FirstName,st.LastName
		ORDER BY CAST(AVG(se.Grade) AS DECIMAL(3,2)) DESC,st.FirstName, st.LastName


--9. Not So In The Studying

SELECT		CONCAT(st.FirstName,' ',st.MiddleName + ' ',st.LastName) AS [Full Name]			
			FROM StudentsSubjects AS sb
			RIGHT JOIN Students AS st
			ON sb.StudentId= st.Id
			WHERE sb.Id IS NULL
			ORDER BY [Full Name]

--10. Average Grade per Subject
SELECT s.[Name],
		AVG(sb.Grade) AS [AverageGrade]
		FROM Subjects AS s
		JOIN StudentsSubjects AS sb
		ON s.Id = sb.SubjectId
		GROUP BY s.[Name],s.Id
		ORDER BY s.Id

		GO
--11. Exam Grades
CREATE FUNCTION udf_ExamGradesToUpdate(@studentId INT, @grade DECIMAL(6,2))
RETURNS NVARCHAR(MAX)
AS
BEGIN
DECLARE @error NVARCHAR(MAX)

	IF NOT EXISTS((SELECT * FROM StudentsExams WHERE StudentId = @studentId))
	BEGIN
		SET @error = 'The student with provided id does not exist in the school!'
		RETURN @error
	END

	IF (@grade > 6.00)
	BEGIN
		SET @error = 'Grade cannot be above 6.00!'
		RETURN @error
	END

	DECLARE @studentName NVARCHAR(MAX) = (SELECT FirstName FROM Students WHERE Id = @studentId)

	DECLARE @count INT = 
		(
		SELECT 
		SUM(K.Count) 
		FROM (
		SELECT 
		COUNT(StudentId) AS [Count]
		FROM StudentsExams 
		WHERE StudentId = @studentId AND Grade != @grade AND 
		  Grade BETWEEN @grade - 0.50 AND @grade + 0.50 
		GROUP BY StudentId, ExamId, Grade
		) AS K
		)

	DECLARE @output NVARCHAR(MAX) = 
				'You have to update ' + 
				CAST(@count AS NVARCHAR(MAX)) + 
				' grades for the student ' + 
				@studentName

	RETURN @output
END
GO

SELECT dbo.udf_ExamGradesToUpdate(12, 6.20)
GO

SELECT dbo.udf_ExamGradesToUpdate(12, 5.50)
GO

SELECT dbo.udf_ExamGradesToUpdate(121, 5.50)
GO

--12. Exclude from school
CREATE PROC usp_ExcludeFromSchool(@StudentId INT)
AS
BEGIN
	
	IF NOT EXISTS (SELECT * FROM Students WHERE Id = @StudentId)
	BEGIN
		THROW 50001, 'This school has no student with the provided id!', 1
	END

	DELETE 
		ST
		FROM StudentsTeachers ST 
		WHERE ST.StudentId = @StudentId

	DELETE 
		SE
		FROM StudentsExams SE
		WHERE SE.StudentId = @StudentId

	DELETE 
		SB
		FROM StudentsSubjects SB
		WHERE SB.StudentId = @StudentId

	DELETE 
		Students 
		WHERE Id = @StudentId
	
END

GO

EXEC usp_ExcludeFromSchool 1
SELECT COUNT(*) FROM Students
GO


EXEC usp_ExcludeFromSchool 301
GO