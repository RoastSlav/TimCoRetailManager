using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TRMDataManager.Library.DataAccess;
using TRMDataManager.Library.Models;

namespace TRMApi.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class SaleController : ControllerBase
{
    private readonly ISaleData _saleData;
    private readonly ILogger<SaleController> _logger;

    public SaleController(ISaleData saleData, ILogger<SaleController> logger)
    {
        _saleData = saleData;
        _logger = logger;
    }
        
    [HttpPost]
    public void Post(SaleModel sale)
    {
        string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        _saleData.SaveSale(sale, userId);
        _logger.LogInformation("Created a sale record by user {userId}", userId);
    }

    [Authorize(Roles = "Admin,Manager")]
    [Route("GetSalesReport")]
    [HttpGet]
    public List<SaleReportModel> GetSalesReport()
    {
        string userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

        _logger.LogInformation("User {userId} requested a sales report", userId);
        return _saleData.GetSaleReport();
    }

    [AllowAnonymous]
    [Route("GetTaxRate")]
    [HttpGet]
    public decimal GetTaxRate()
    {
        return _saleData.GetTaxRate();
    }
}