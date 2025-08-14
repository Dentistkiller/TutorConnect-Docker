-- Ensure DB exists and apply schema if needed
IF DB_ID('TutorDb') IS NULL
BEGIN
    CREATE DATABASE TutorDb;
END
GO
USE TutorDb;
GO

-- Idempotent creates
IF OBJECT_ID('dbo.Students','U') IS NULL
BEGIN
    CREATE TABLE Students (
        StudentId INT IDENTITY(1,1) PRIMARY KEY,
        FullName NVARCHAR(100) NOT NULL,
        Email NVARCHAR(100) NOT NULL UNIQUE,
        PhoneNumber NVARCHAR(15)
    );
END

IF OBJECT_ID('dbo.Tutors','U') IS NULL
BEGIN
    CREATE TABLE Tutors (
        TutorId INT IDENTITY(1,1) PRIMARY KEY,
        FullName NVARCHAR(100) NOT NULL,
        Email NVARCHAR(100) NOT NULL UNIQUE,
        Subject NVARCHAR(50) NOT NULL
    );
END

IF OBJECT_ID('dbo.Sessions','U') IS NULL
BEGIN
    CREATE TABLE Sessions (
        SessionId INT IDENTITY(1,1) PRIMARY KEY,
        StudentId INT NOT NULL,
        TutorId INT NOT NULL,
        SessionDate DATETIME NOT NULL,
        DurationMinutes INT NOT NULL,
        FOREIGN KEY (StudentId) REFERENCES Students(StudentId),
        FOREIGN KEY (TutorId) REFERENCES Tutors(TutorId)
    );
END

-- Optional seed
IF NOT EXISTS (SELECT 1 FROM Students)
INSERT INTO Students (FullName, Email, PhoneNumber) VALUES
('Aisha Khan','aisha@example.com','0712345678'),
('Liam Smith','liam@example.com','0823456789');

IF NOT EXISTS (SELECT 1 FROM Tutors)
INSERT INTO Tutors (FullName, Email, Subject) VALUES
('Naledi Mokoena','naledi@example.com','Math'),
('Johan van Wyk','johan@example.com','Physics');
