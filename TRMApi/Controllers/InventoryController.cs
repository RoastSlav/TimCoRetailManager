using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TRMDataManager.Library.DataAccess;
using TRMDataManager.Library.Models;

namespace TRMApi.Controllers;

[Route("api/[controller]")]
[ApiController]
[Authorize]
public class InventoryController : ControllerBase
{
    private readonly IInventoryData _inventoryData;
    private readonly ILogger<InventoryController> _logger;

    public InventoryController(IInventoryData inventoryData, ILogger<InventoryController> logger)
    {
        _inventoryData = inventoryData;
        _logger = logger;
    }

    [Authorize(Roles = "Manager,Admin")]
    [HttpGet]
    public List<InventoryModel> Get()
    {
        return _inventoryData.GetInventory();
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    public void Post(InventoryModel item)
    {
        _inventoryData.SaveInventoryRecord(item);
        _logger.LogInformation("Added {quantity} items to inventory with ID: {itemId}",item.Quantity, item.ProductId);
    }
        
}