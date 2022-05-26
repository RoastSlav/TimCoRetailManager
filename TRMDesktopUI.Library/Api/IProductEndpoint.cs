using TRMDesktopUI.Library.Models;

namespace TRMDesktopUI.Library.Api;

public interface IProductEndpoint
{
    Task<List<ProductModel>> GetAll();
}