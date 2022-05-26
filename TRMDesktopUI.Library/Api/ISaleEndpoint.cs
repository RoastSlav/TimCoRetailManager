using TRMDesktopUI.Library.Models;

namespace TRMDesktopUI.Library.Api;

public interface ISaleEndpoint
{
    Task PostSale(SaleModel sale);
}