﻿namespace TRMDataManager.Library.DataAccess;

public interface ISqlDataAccess
{
    string GetConnectionString(string name);
    List<T> LoadData<T, U>(string storedProcedure, U parameters, string connectionStringName);
    void SaveData<T>(string storedProcedure, T parameters, string connectionStringName);
    void StartTransaction(string connectionStringName);
    void SaveDataInTransaction<T>(string storedProcedure, T parameters);
    List<T> LoadDataInTransaction<T, U>(string storedProcedure, U parameters);
    void CommitTransaction();
    void RollbackTransaction();
    void Dispose();
}