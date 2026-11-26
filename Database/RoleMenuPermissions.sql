-- Role based menu permissions.
-- One row per role + menu. Missing row = no access.
-- Run this once against the SunwinInventory database.

IF OBJECT_ID('dbo.RoleMenuPermissions', 'U') IS NULL
BEGIN
    CREATE TABLE dbo.RoleMenuPermissions
    (
        Id          INT IDENTITY(1,1) NOT NULL
            CONSTRAINT PK_RoleMenuPermissions PRIMARY KEY,
        RoleId      INT NOT NULL
            CONSTRAINT FK_RoleMenuPermissions_Roles REFERENCES dbo.Roles(RoleId),
        MenuKey     VARCHAR(50) NOT NULL,
        CanView     BIT NOT NULL
            CONSTRAINT DF_RoleMenuPermissions_CanView DEFAULT (1),
        CanCreate   BIT NOT NULL
            CONSTRAINT DF_RoleMenuPermissions_CanCreate DEFAULT (0),
        CanEdit     BIT NOT NULL
            CONSTRAINT DF_RoleMenuPermissions_CanEdit DEFAULT (0),
        CanDelete   BIT NOT NULL
            CONSTRAINT DF_RoleMenuPermissions_CanDelete DEFAULT (0),
        CONSTRAINT UQ_RoleMenuPermissions_Role_Menu UNIQUE (RoleId, MenuKey)
    );
END;
GO

-- Existing roles keep working: grant every menu until restrictions are set.
INSERT INTO dbo.RoleMenuPermissions
    (RoleId, MenuKey, CanView, CanCreate, CanEdit, CanDelete)
SELECT r.RoleId, m.MenuKey, 1, 0, 0, 0
FROM dbo.Roles r
CROSS JOIN (VALUES
    ('warehouse'),
    ('product-category'),
    ('product-name'),
    ('supplier'),
    ('shipment'),
    ('purchase-entry'),
    ('customers'),
    ('sales-entry'),
    ('invoice-print'),
    ('customer-due'),
    ('customer-due-payment'),
    ('purchase-report'),
    ('sales-report'),
    ('stock-report'),
    ('designation'),
    ('role'),
    ('users')
) m(MenuKey)
WHERE NOT EXISTS
(
    SELECT 1
    FROM dbo.RoleMenuPermissions p
    WHERE p.RoleId = r.RoleId
      AND p.MenuKey = m.MenuKey
);
GO
