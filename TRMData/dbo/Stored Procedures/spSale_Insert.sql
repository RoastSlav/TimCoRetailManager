CREATE PROCEDURE [dbo].[spSale_Insert]
	@Id INT OUTPUT,
	@CashierId NVARCHAR(128),
	@SaleDate DATETIME2,
	@SubTotal MONEY,
	@Tax MONEY,
	@Total MONEY
	AS
begin
	set nocount on;

	insert into dbo.Sale(CashierId,SaleDate,SubTotal,Tax,Total)
	values (@CashierId,@SaleDate,@SubTotal,@Tax,@Total);

	select @Id = Scope_Identity();
end