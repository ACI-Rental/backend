INSERT INTO [ProductService].[dbo].[Categories] (Name) VALUES ('camera');
INSERT INTO [ProductService].[dbo].[Categories] (Name) VALUES ('kabel');
INSERT INTO [ProductService].[dbo].[Categories] (Name) VALUES ('laptop');

INSERT INTO [ProductService].[dbo].[Products] ([CatalogNumber], [Name], [Description], [InventoryLocation], [ProductState], [RequiresApproval], [CategoryId]) VALUES (1, 'Canon X32', 'Lorem ipsum dolor mir amet', 'Tilburg', 0, 0, 1)
INSERT INTO [ProductService].[dbo].[Products] ([CatalogNumber], [Name], [Description], [InventoryLocation], [ProductState], [RequiresApproval], [CategoryId]) VALUES (3, 'Dell laptop', 'Lorem ipsum dolor mir amet', 'Tilburg', 0, 0, 3)
INSERT INTO [ProductService].[dbo].[Products] ([CatalogNumber], [Name], [Description], [InventoryLocation], [ProductState], [RequiresApproval], [CategoryId]) VALUES (2, 'HDMI kabel', 'Lorem ipsum dolor mir amet', 'Eindhoven', 0, 0, 2)
INSERT INTO [ProductService].[dbo].[Products] ([CatalogNumber], [Name], [Description], [InventoryLocation], [ProductState], [RequiresApproval], [CategoryId]) VALUES (3, 'Apple macbook pro', 'Lorem ipsum dolor mir amet', 'Eindhoven', 0, 0, 3)

INSERT INTO UserService.dbo.UserInfo (Name, Studentnumber, Password) VALUES (N'admin', N'123', N'admin');
INSERT INTO UserService.dbo.Users (UserInfoId, RefreshToken, Banned, BannedUntil, RoleId) VALUES (1, null, 0, null, null);
