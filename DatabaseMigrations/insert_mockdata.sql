INSERT INTO [aci-product-service].[dbo].[Categories] (Name) VALUES ('camera');
INSERT INTO [aci-product-service].[dbo].[Categories] (Name) VALUES ('kabel');
INSERT INTO [aci-product-service].[dbo].[Categories] (Name) VALUES ('laptop');

INSERT INTO [aci-product-service].[dbo].[Products] ([CatalogNumber], [Name], [Description], [InventoryLocation], [ProductState], [RequiresApproval], [CategoryId]) VALUES (1, 'Canon X32', 'Lorem ipsum dolor mir amet', 'Tilburg', 0, 0, 1)
INSERT INTO [aci-product-service].[dbo].[Products] ([CatalogNumber], [Name], [Description], [InventoryLocation], [ProductState], [RequiresApproval], [CategoryId]) VALUES (3, 'Dell laptop', 'Lorem ipsum dolor mir amet', 'Tilburg', 0, 0, 3)
INSERT INTO [aci-product-service].[dbo].[Products] ([CatalogNumber], [Name], [Description], [InventoryLocation], [ProductState], [RequiresApproval], [CategoryId]) VALUES (2, 'HDMI kabel', 'Lorem ipsum dolor mir amet', 'Eindhoven', 0, 0, 2)
INSERT INTO [aci-product-service].[dbo].[Products] ([CatalogNumber], [Name], [Description], [InventoryLocation], [ProductState], [RequiresApproval], [CategoryId]) VALUES (3, 'Apple macbook pro', 'Lorem ipsum dolor mir amet', 'Eindhoven', 0, 0, 3)

