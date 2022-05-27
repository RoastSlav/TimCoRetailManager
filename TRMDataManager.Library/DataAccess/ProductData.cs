using TRMDataManager.Library.Models;

namespace TRMDataManager.Library.DataAccess;

public class ProductData : IProductData
{
    private readonly ISqlDataAccess _sqlDataAccess;

    public ProductData(ISqlDataAccess sqlDataAccess)
    {
        _sqlDataAccess = sqlDataAccess;
    }
        
    public List<ProductModel> GetProducts()
    {
        var output = _sqlDataAccess.LoadData<ProductModel, dynamic>("dbo.spProduct_GetAll", new { }, "TRMData");

        return output;
    }

    public ProductModel GetProductById(int productId)
    {
        var output = _sqlDataAccess.LoadData<ProductModel, dynamic>("dbo.spProduct_GetById", new { Id = productId }, "TRMData").FirstOrDefault();

        return output;
    }
}