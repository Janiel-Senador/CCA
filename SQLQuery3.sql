-- 1. Create the 'Admin' role if it doesn't exist
IF NOT EXISTS (SELECT Id FROM AspNetRoles WHERE Name = 'Admin')
BEGIN
    INSERT INTO AspNetRoles (Id, Name, NormalizedName, ConcurrencyStamp) 
    VALUES (NEWID(), 'Admin', 'ADMIN', NEWID());
END

-- 2. Link your account to the Admin role
DECLARE @AdminRole NVARCHAR(450) = (SELECT Id FROM AspNetRoles WHERE Name = 'Admin');
DECLARE @AdminUser NVARCHAR(450) = (SELECT Id FROM AspNetUsers WHERE Email = 'admin@cca.com');

IF @AdminUser IS NOT NULL AND NOT EXISTS (SELECT 1 FROM AspNetUserRoles WHERE UserId = @AdminUser AND RoleId = @AdminRole)
BEGIN
    INSERT INTO AspNetUserRoles (UserId, RoleId) VALUES (@AdminUser, @AdminRole);
    PRINT '✅ admin@cca.com is now an Admin!';
END
ELSE
    PRINT '⚠️ User not found or already an Admin.';