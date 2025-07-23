
CREATE DATABASE PersonDirectoryDb;
GO

USE PersonDirectoryDb;
GO


IF OBJECT_ID('dbo.RelatedPersons', 'U') IS NOT NULL DROP TABLE dbo.RelatedPersons;
IF OBJECT_ID('dbo.PhoneNumbers', 'U') IS NOT NULL DROP TABLE dbo.PhoneNumbers;
IF OBJECT_ID('dbo.Persons', 'U') IS NOT NULL DROP TABLE dbo.Persons;
GO


CREATE TABLE Persons (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    FirstName NVARCHAR(100) NOT NULL,
    LastName NVARCHAR(100) NOT NULL,
    Gender NVARCHAR(20) NOT NULL CHECK (Gender IN (N'ქალი', N'კაცი')),
    PersonalNumber NVARCHAR(11) NOT NULL UNIQUE,
    DateOfBirth DATE NOT NULL
);
GO


CREATE TABLE PhoneNumbers (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    PersonId INT NOT NULL,
    Number NVARCHAR(50) NOT NULL,
    PhoneType NVARCHAR(20) NOT NULL CHECK (PhoneNumbers.Type IN (N'მობილური',N'ოფისი',N'სახლი')),
    CONSTRAINT FK_PhoneNumbers_Persons FOREIGN KEY (PersonId)
        REFERENCES Persons(Id) ON DELETE CASCADE
);
GO


CREATE TABLE RelatedPersons (
    Id INT IDENTITY(1,1) PRIMARY KEY,
    PersonId INT NOT NULL,
    RelatedPersonId INT NOT NULL,
    RelationType NVARCHAR(50) NOT NULL CHECK (RelationType IN (N'კოლეგა',N'ნაცნობი',N'ნათესავი',N'სხვა')),
    CONSTRAINT FK_RelatedPersons_Person FOREIGN KEY (PersonId) REFERENCES Persons(Id) ON DELETE CASCADE,
    CONSTRAINT FK_RelatedPersons_Related FOREIGN KEY (RelatedPersonId) REFERENCES Persons(Id) ON DELETE CASCADE,
    CONSTRAINT CK_RelatedPersons_NotSelfRelation CHECK (PersonId <> RelatedPersonId)
);
GO
