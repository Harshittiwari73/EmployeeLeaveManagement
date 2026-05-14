
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Roles')
BEGIN
    CREATE TABLE Roles (
        RoleId INT PRIMARY KEY IDENTITY(1,1),
        RoleName NVARCHAR(50) NOT NULL UNIQUE
    );
    INSERT INTO Roles (RoleName) VALUES ('Admin'), ('Manager'), ('Employee');
END
GO
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Users')
BEGIN
    CREATE TABLE Users (
        UserId INT PRIMARY KEY IDENTITY(1,1),
        Username NVARCHAR(100) NOT NULL,
        Email NVARCHAR(100) NOT NULL UNIQUE,
        PhoneNumber NVARCHAR(20),
        Password NVARCHAR(MAX) NOT NULL,
        RoleId INT NOT NULL,
        IsActive BIT NOT NULL DEFAULT 1,
        CreatedDate DATETIME NOT NULL DEFAULT GETDATE(),
        CONSTRAINT FK_Users_Roles FOREIGN KEY (RoleId) REFERENCES Roles(RoleId)
    );

    DECLARE @AdminRoleId INT = (SELECT RoleId FROM Roles WHERE RoleName = 'Admin');
    INSERT INTO Users (Username, Email, PhoneNumber, Password, RoleId, IsActive, CreatedDate)
    VALUES ('Admin', 'admin@example.com', '1234567890', '$2a$11$qR5uX.p3V5t/nS.uE6V9u.pG9Z1I0m8o4V/WqO1u/U6y1t/X.y', @AdminRoleId, 1, GETDATE());
END
GO
