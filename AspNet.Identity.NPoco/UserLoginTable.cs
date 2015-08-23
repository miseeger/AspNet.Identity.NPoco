using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNet.Identity;
using NPoco;

namespace AspNet.Identity.NPoco
{

    /// <summary>
    /// Class that represents the UserLogin table in the database
    /// </summary>
    public class UserLoginTable
    {
        private readonly Database _database;

        public UserLoginTable(Database database)
        {
            _database = database;
        }


        public int Delete(IdentityUser user, UserLoginInfo login)
        {
            return
               _database.Execute(
                   "DELETE FROM \r\n" +
                   "   UserLogin \r\n" +
                   "WHERE \r\n" +
                   "   UserId = @0 \r\n" +
                   "   AND LoginProvider = @1 \r\n" +
                   "   AND ProviderKey = @2",
                   user.Id
                   , login.LoginProvider
                   , login.ProviderKey
               );
        }


        public int Delete(string userId)
        {
            return
               _database.Execute(
                   "DELETE FROM \r\n" +
                   "   UserLogin \r\n" +
                   "WHERE \r\n" +
                   "   UserId = @0",
                   userId
               );
        }


        public int Insert(IdentityUser user, UserLoginInfo login)
        {
            return
                _database.Execute(
                    "INSERT INTO UserLogin (\r\n" +
                    "   LoginProvider \r\n" +
                    "   ,ProviderKey \r\n" +
                    "   ,UserId) \r\n" +
                    "VALUES ( \r\n" +
                    "   @0 \r\n" +
                    "   ,@1 \r\n" +
                    "   ,@2)",
                    login.LoginProvider
                    , login.ProviderKey
                    , user.Id
                );
        }


        public string FindUserIdByLogin(UserLoginInfo userLogin)
        {
            return
                _database.ExecuteScalar<string>(
                    "SELECT \r\n" +
                    "   UserId \r\n" +
                    "FROM \r\n" +
                    "   UserLogin \r\n" +
                    "WHERE \r\n" +
                    "   LoginProvider = @0\r\n" +
                    "   AND ProviderKey = @1",
                    userLogin.LoginProvider
                    , userLogin.ProviderKey
                );
        }


        public List<UserLoginInfo> FindByUserId(string userId)
        {
            return
                _database.Fetch<IdentityUserLogin>(
                    "SELECT \r\n" +
                    "   * \r\n" +
                    "FROM \r\n" +
                    "   UserLogin \r\n" +
                    "WHERE \r\n" +
                    "   UserId = @0",
                    userId
                )
                .Select(userLogin => new UserLoginInfo(userLogin.LoginProvider, userLogin.ProviderKey))
                .ToList();
        }

    }

}
