CREATE PROCEDURE [dbo].[spProduct_GetById]
	@Id int
AS
begin
	set nocount on;

	SELECT Id,ProductName,[Description],RetailPrice,QuantityInStock, IsTaxable, ProductImage
	FROM dbo.Product
	WHERE Id = @Id;
end
