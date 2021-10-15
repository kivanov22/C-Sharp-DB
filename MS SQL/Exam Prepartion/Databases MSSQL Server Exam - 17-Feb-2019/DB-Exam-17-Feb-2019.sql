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

