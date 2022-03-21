CREATE PROCEDURE [dbo].[spInventory_GetAll]
AS
BEGIN
	set nocount on;

	select [ProductId], [Quantity], [PurchasePrice], [PurchaseDate]
	from dbo.Inventory;
END
