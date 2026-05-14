
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID('Roles') AND name = 'IsActive')
BEGIN
    ALTER TABLE Roles ADD IsActive BIT NOT NULL DEFAULT 1;
END
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Permissions')
BEGIN
    CREATE TABLE Permissions (
        PermissionId INT PRIMARY KEY IDENTITY(1,1),
        ModuleName NVARCHAR(100) NOT NULL,
        PermissionName NVARCHAR(100) NOT NULL,
        Description NVARCHAR(250)
    );

    INSERT INTO Permissions (ModuleName, PermissionName) VALUES ('User', 'User List'), ('User', 'Add/Edit User'), ('User', 'Delete User');

    INSERT INTO Permissions (ModuleName, PermissionName) VALUES ('Role', 'Role List'), ('Role', 'Add/Edit Role'), ('Role', 'Delete Role');
    INSERT INTO Permissions (ModuleName, PermissionName) VALUES ('Property', 'Property List'), ('Property', 'Add Property'), ('Property', 'Edit Property'), ('Property', 'Delete Property'), ('Property', 'Violation List'), ('Property', 'View Property');
    INSERT INTO Permissions (ModuleName, PermissionName) VALUES ('Share Documents', 'Document List'), ('Share Documents', 'Add Document'), ('Share Documents', 'Edit Document'), ('Share Documents', 'Delete Document'), ('Share Documents', 'View Document');
    INSERT INTO Permissions (ModuleName, PermissionName) VALUES ('Elections', 'Election List'), ('Elections', 'Add/Edit Election');
  
    INSERT INTO Permissions (ModuleName, PermissionName) VALUES ('Email Template', 'Email Template List'), ('Email Template', 'Add/Edit Email Template'), ('Email Template', 'Delete Email Template');
END
GO

IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'RolePermissions')
BEGIN
    CREATE TABLE RolePermissions (
        RolePermissionId INT PRIMARY KEY IDENTITY(1,1),
        RoleId INT NOT NULL,
        PermissionId INT NOT NULL,
        IsAllowed BIT NOT NULL DEFAULT 0,
        CONSTRAINT FK_RolePermissions_Roles FOREIGN KEY (RoleId) REFERENCES Roles(RoleId),
        CONSTRAINT FK_RolePermissions_Permissions FOREIGN KEY (PermissionId) REFERENCES Permissions(PermissionId)
    );
END
GO
