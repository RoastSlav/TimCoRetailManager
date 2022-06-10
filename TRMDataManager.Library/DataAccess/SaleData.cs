using System.Configuration;
using Microsoft.Extensions.Configuration;
using TRMDataManager.Library.Models;

namespace TRMDataManager.Library.DataAccess;

public class SaleData : ISaleData
{
    private readonly IProductData _productData;
    private readonly ISqlDataAccess _sqlDataAccess;
    private readonly IConfiguration _config;

    public SaleData(IProductData productData, ISqlDataAccess sqlDataAccess, IConfiguration config)
    {
        _productData = productData;
        _sqlDataAccess = sqlDataAccess;
        _config = config;
    }

    public decimal GetTaxRate()
    {
        string rateText = _config.GetValue<string>("TaxRate");

        bool IsValidTaxRate = Decimal.TryParse(rateText, out decimal output);

        if (IsValidTaxRate == false)
        {
            throw new ConfigurationErrorsException("The tax rate is not set up properly");
        }

        output /= 100;

        return output;
    }

    private List<SaleDetailDBModel> CollectProductDetailsInSale(SaleModel saleInfo)
    {
        List<SaleDetailDBModel> details = new();
        var taxRate = GetTaxRate();

        foreach (var item in saleInfo.SaleDetails)
        {
            var detail = new SaleDetailDBModel
            {
                ProductId = item.ProductId,
                Quantity = item.Quantity
            };

            //Get the information about this product
            var productInfo = _productData.GetProductById(detail.ProductId);

            if (productInfo == null)
            {
                throw new($"The product Id of {detail.ProductId} could not be found in the database.");
            }

            detail.PurchasePrice = (productInfo.RetailPrice * detail.Quantity);

            if (productInfo.IsTaxable)
            {
                detail.Tax = detail.PurchasePrice * taxRate;
            }

            details.Add(detail);
        }
        return details;
    }

    public void SaveSale(SaleModel saleInfo,string cashierId)
    {
        var details = CollectProductDetailsInSale(saleInfo);

        SaleDbModel sale = new()
        {
            SubTotal = details.Sum(x => x.PurchasePrice),
            Tax = details.Sum(x => x.Tax),
            CashierId = cashierId
        };
        sale.Total = sale.SubTotal + sale.Tax;
            
        try
        {
            _sqlDataAccess.StartTransaction("TRMData");

            _sqlDataAccess.SaveDataInTransaction("dbo.spSale_Insert", sale);

            //Get the ID from the sale model
            sale.Id = _sqlDataAccess.LoadDataInTransaction<int, dynamic>("spSale_Lookup", new { sale.CashierId, sale.SaleDate }).FirstOrDefault();

            foreach (var item in details)
            {
                item.SaleId = sale.Id;
                _sqlDataAccess.SaveDataInTransaction("dbo.spSaleDetail_Insert", item);
            }

            _sqlDataAccess.CommitTransaction();
        }
        catch
        {
            _sqlDataAccess.RollbackTransaction();
            throw;
        }
    }

    public List<SaleReportModel> GetSaleReport()
    {
        var output = _sqlDataAccess.LoadData<SaleReportModel, dynamic>("dbo.spSale_SaleReport", new { }, "TRMData");

        return output;
    }
}