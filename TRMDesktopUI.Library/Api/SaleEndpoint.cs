using TRMDesktopUI.Library.Models;

namespace TRMDesktopUI.Library.Api;

public class SaleEndpoint : ISaleEndpoint
{
    private readonly IAPIHelper _apiHelper;
    public SaleEndpoint(IAPIHelper apiHelper)
    {
        _apiHelper = apiHelper;
    }

    public async Task PostSale(SaleModel sale)
    {
        using HttpResponseMessage response = await _apiHelper.ApiClient.PostAsJsonAsync("/api/Sale", sale);
        if (response.IsSuccessStatusCode)
        {
            //TODO Log successful call?
        }
        else
        {
            throw new(response.ReasonPhrase);
        }
    }
}