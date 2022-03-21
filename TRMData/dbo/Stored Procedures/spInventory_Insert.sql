CREATE PROCEDURE [dbo].[spInventory_Insert]
	@ProductId int,
	@Quantity int,
	@PurchasePrice money,
	@PurchaseDate datetime2
 
AS
BEGIN
	set nocount on;

	insert into dbo.Inventory(ProductId,Quantity,PurchasePrice,PurchaseDate)
	values (@ProductId,@Quantity,@PurchasePrice,@PurchaseDate);
END
