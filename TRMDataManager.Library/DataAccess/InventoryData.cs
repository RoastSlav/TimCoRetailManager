﻿using TRMDataManager.Library.Models;

namespace TRMDataManager.Library.DataAccess;

public class InventoryData : IInventoryData
{
    private readonly ISqlDataAccess _sqlDataAccess;

    public InventoryData(ISqlDataAccess sqlDataAccess)
    {
        _sqlDataAccess = sqlDataAccess;
    }

    public List<InventoryModel> GetInventory()
    {
        var output = _sqlDataAccess.LoadData<InventoryModel, dynamic>("dbo.spInventory_GetAll", new { }, "TRMData");
            
        return output;
    }

    public void SaveInventoryRecord(InventoryModel item)
    {
        _sqlDataAccess.SaveData("dbo.spInventory_Insert", item, "TRMData");
    }
}