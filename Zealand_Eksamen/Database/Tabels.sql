create database Zealand_Eksamen

use Zealand_Eksamen;

CREATE TABLE Employee (
                          EmployeeID INT IDENTITY(1,1) PRIMARY KEY,
                          FullName NVARCHAR(100) NOT NULL,
                          Email NVARCHAR(100) NOT NULL UNIQUE,
                          Role NVARCHAR(50) NOT NULL,         -- Eksaminator, Censor, Admin osv.
                          Department NVARCHAR(100) NULL
);

CREATE TABLE Class (
                       ClassID INT IDENTITY(1,1) PRIMARY KEY,
                       ClassCode NVARCHAR(50) NOT NULL UNIQUE,   -- F.eks. "DAT-RO-F-V25B"
                       Semester NVARCHAR(20) NOT NULL,
                       Programme NVARCHAR(100) NOT NULL          -- F.eks. "Datamatiker"
);

CREATE TABLE ExamType (
                          ExamTypeID INT IDENTITY(1,1) PRIMARY KEY,
                          TypeName NVARCHAR(50) NOT NULL UNIQUE     -- F.eks. "Mundtlig", "Projekt"
);

CREATE TABLE Exam (
                      ExamID INT IDENTITY(1,1) PRIMARY KEY,
                      ExamName NVARCHAR(100) NOT NULL,
                      Form NVARCHAR(100) NULL,                  -- F.eks. "Mundtlig + Projekt"
                      HasSupervision BIT NOT NULL DEFAULT 0,    -- Ja/Nej for tilsyn
                      EstimatedStudents INT NULL,
                      CensorType NVARCHAR(50) NULL,             -- Intern/Ekstern
                      OrdinaryDeliveryDate DATE NULL,
                      OrdinaryStartDate DATE NULL,
                      OrdinaryEndDate DATE NULL,
                      ReexamDeliveryDate DATE NULL,
                      ReexamDate DATE NULL,
                      ClassID INT NOT NULL,                     -- FK til Class
                      ExamTypeID INT NULL,                      -- FK til ExamType
                      CONSTRAINT FK_Exam_Class FOREIGN KEY (ClassID) REFERENCES Class(ClassID),
                      CONSTRAINT FK_Exam_ExamType FOREIGN KEY (ExamTypeID) REFERENCES ExamType(ExamTypeID)
);

CREATE TABLE ExamAssignment (
                                ExamAssignmentID INT IDENTITY(1,1) PRIMARY KEY,
                                ExamID INT NOT NULL,
                                EmployeeID INT NOT NULL,
                                RoleInExam NVARCHAR(50) NULL,            -- Eksaminator, Censor osv.
                                CONSTRAINT FK_ExamAssignment_Exam FOREIGN KEY (ExamID) REFERENCES Exam(ExamID) ON DELETE CASCADE,
                                CONSTRAINT FK_ExamAssignment_Employee FOREIGN KEY (EmployeeID) REFERENCES Employee(EmployeeID) ON DELETE CASCADE,
                                CONSTRAINT UQ_ExamAssignment UNIQUE (ExamID, EmployeeID) -- Undgå dubletter
);
