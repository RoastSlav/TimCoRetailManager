using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using TRMDataManager.Library.Models;

namespace TRMDataManager.Library.DataAccess
{
    public class UserData : IUserData
    {
        private readonly ISqlDataAccess _sqlDataAccess;
        private readonly IConfiguration _config;

        public UserData(ISqlDataAccess sqlDataAccess)
        {
            _sqlDataAccess = sqlDataAccess;
        }

        public List<UserModel> GetUserById(string Id)
        {
            var output = _sqlDataAccess.LoadData<UserModel, dynamic>("dbo.spUserLookup", new { Id }, "TRMData");

            return output;
        }

        public void CreateUser(UserModel user)
        {
            _sqlDataAccess.SaveData("dbo.spUser_Create",
                new {user.Id, user.FirstName, user.LastName, user.EmailAddress}, "TRMData");
        }
    }
}
