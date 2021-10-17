CREATE DATABASE [EntityRelations]

USE [EntityRelations]


--1) Task
CREATE TABLE [Passports](
		[PassportID] INT PRIMARY KEY NOT NULL,
		[PassportNumber] CHAR(8) NOT NULL
		)

CREATE TABLE [Persons](
	[PersonID] INT PRIMARY KEY IDENTITY NOT NULL,
	[FirstName]	VARCHAR(50) NOT NULL,
	[Salary] DECIMAL(8,2) NOT NULL,

	[PassportID] INT FOREIGN KEY REFERENCES [Passports]([PassportID]) UNIQUE NOT NULL
	)

INSERT INTO [Passports]([PassportID],[PassportNumber])
VALUES 
(101,'N34FG21B'),
(102,'K65LO4R7'),
(103,'ZE657QP2')

INSERT INTO [Persons]([FirstName],[Salary],[PassportID])
VALUES
('Roberto',43300.00,102),
('Tom', 56100.00,103),
('Yana',60200.00,101)

--2) Task
CREATE TABLE [Manufacturers](
[ManufacturerID] INT PRIMARY KEY IDENTITY NOT NULL,
[Name] VARCHAR(50) NOT NULL,
[EstablishedOn] DATE NOT NULL
)

CREATE TABLE [Models](
[ModelID] INT PRIMARY KEY IDENTITY(101,1) NOT NULL,
[Name] VARCHAR(50) NOT NULL,
[ManufacturerID] INT FOREIGN KEY REFERENCES [Manufacturers]([ManufacturerID]) NOT NULL
)

INSERT INTO [Manufacturers]([Name],[EstablishedOn])
VALUES 
	('BMW','07/03/1916'),
	('Tesla','01/01/2003'),
	('Lada','01/05/1966')

INSERT INTO [Models]([Name],[ManufacturerID])
VALUES 
	('X1',1),
	('i6',1),
	('Model S',2),
	('Model X',2),
	('Model 3',2),
	('Nova',3)

--03)Task
CREATE TABLE [Students](
[StudentID] INT PRIMARY KEY IDENTITY NOT NULL,
[Name] VARCHAR(50) NOT NULL
)

CREATE TABLE [Exams](
[ExamID] INT PRIMARY KEY IDENTITY(101,1) NOT NULL,
[Name] VARCHAR(75) NOT NULL
)

--mapping table
CREATE TABLE [StudentsExams](
[StudentID] INT FOREIGN KEY REFERENCES [Students]([StudentID]) NOT NULL,
[ExamID] INT FOREIGN KEY REFERENCES [Exams]([ExamID]) NOT NULL
PRIMARY KEY([StudentID],[ExamID]) --Composite key
)

INSERT INTO [Students]([Name])
VALUES
	('Mila'),
	('Toni'),
	('Ron')

INSERT INTO [Exams]([Name])
VALUES
	('SpringMVC'),
	('Neo4j'),
	('Oracle 11g')

--4) Task
CREATE TABLE [Teachers](
[TeacherID] INT PRIMARY KEY IDENTITY(101,1) NOT NULL,
[Name] VARCHAR(50) NOT NULL,
[ManagerID] INT FOREIGN KEY REFERENCES [Teachers]([TeacherID])
)

INSERT INTO [Teachers] ([Name],[ManagerID])
VALUES
('John',NULL),
('Maya',106),
('Silvia',106),
('Ted',105),
('Mark',101),
('Greta',101)

--05. Online Store Database 
CREATE DATABASE Store

CREATE TABLE ItemTypes(
	ItemTypeID INT IDENTITY NOT NULL,
	[Name] NVARCHAR(50) NOT NULL,

	CONSTRAINT PK_ItemTypes_ItemTypeID
	PRIMARY KEY(ItemTypeID)
)

CREATE TABLE Items(
	ItemID INT IDENTITY NOT NULL,
	[Name] NVARCHAR(50) NOT NULL,
	ItemTypeID INT NOT NULL

	CONSTRAINT PK_Items_ItemID
	PRIMARY KEY(ItemID),

	CONSTRAINT FK_Items_ItemsType
	FOREIGN KEY(ItemTypeID)
	REFERENCES ItemTypes(ItemTypeID)
)

CREATE TABLE Cities(
	CityID INT IDENTITY NOT NULL,
	[Name] NVARCHAR(50) NOT NULL,

	CONSTRAINT PK_Cities_CityID
	PRIMARY KEY(CityID)
)

CREATE TABLE Customers(
	CustomerID INT IDENTITY NOT NULL,
	[Name] NVARCHAR(50) NOT NULL,
	Birthdaty DATE,
	CityID INT NOT NULL,

	CONSTRAINT PK_Customers
	PRIMARY KEY(CustomerID),

	CONSTRAINT FK_Customers_Cities
	FOREIGN KEY(CityID)
	REFERENCES Cities(CityID)
)

CREATE TABLE Orders(
	OrderID INT IDENTITY NOT NULL,
	CustomerID INT NOT NULL,

	CONSTRAINT PK_Orders
	PRIMARY KEY (OrderID),

	CONSTRAINT FK_Orders_Customers
	FOREIGN KEY (CustomerID)
	REFERENCES Customers(CustomerID)
)

CREATE TABLE OrderItems(
	OrderID INT NOT NULL,
	ItemID INT NOT NULL,

	CONSTRAINT PK_OrderItems
	PRIMARY KEY(OrderID, ItemID),

	CONSTRAINT FK_PK_OrderItems_Orders
	FOREIGN KEY(OrderID)
	REFERENCES Orders(OrderID),

	CONSTRAINT FK_PK_OrderItems_Items
	FOREIGN KEY(ItemID)
	REFERENCES Items(ItemID)
)


--07.University Database

CREATE DATABASE University 

CREATE TABLE Subjects(
	SubjectID INT IDENTITY NOT NULL,
	SubjectName NVARCHAR(50) NOT NULL,

	CONSTRAINT PK_Subjects
	PRIMARY KEY(SubjectID)
)

CREATE TABLE Majors(
	MajorID INT IDENTITY NOT NULL,
	[Name] NVARCHAR(50) NOT NULL,

	CONSTRAINT PK_Majors
	PRIMARY KEY(MajorID)
)

CREATE TABLE Students(
	StudentID INT IDENTITY NOT NULL,
	StudentNumber NVARCHAR(50) NOT NULL,
	StudentName NVARCHAR(50) NOT NULL,
	MajorID INT NOT NULL,

	CONSTRAINT PK_Students
	PRIMARY KEY(StudentID),

	CONSTRAINT FK_Students_Majors
	FOREIGN KEY(MajorID)
	REFERENCES Majors(MajorID)
)

CREATE TABLE Payments(
	PaymentID INT IDENTITY NOT NULL,
	PaymentDate DATE NOT NULL,
	PaymentAmount MONEY NOT NULL,
	StudentID INT NOT NULL,

	CONSTRAINT PK_Payments
	PRIMARY KEY (PaymentID),

	CONSTRAINT FK_Payments_Students
	FOREIGN KEY (StudentID)
	REFERENCES Students(StudentID)
)

CREATE TABLE Agenda(
	StudentID INT NOT NULL,
	SubjectID INT NOT NULL,

	CONSTRAINT PK_Agenda
	PRIMARY KEY(StudentID, SubjectID),

	CONSTRAINT FK_Agenda_Students
	FOREIGN KEY(StudentID)
	REFERENCES Students(StudentID),

	CONSTRAINT FK_Agenda_Subjects
	FOREIGN KEY(SubjectID)
	REFERENCES Subjects(SubjectID)
)

--9.Peaks in Rila
SELECT m.MountainRange, p.PeakName, p.Elevation FROM Peaks AS p

JOIN Mountains AS m ON m.Id = p.MountainId
WHERE MountainRange = 'Rila'
ORDER BY p.Elevation DESC