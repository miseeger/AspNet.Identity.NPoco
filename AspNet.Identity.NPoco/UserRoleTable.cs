using System.Collections.Generic;
using NPoco;

namespace AspNet.Identity.NPoco
{

    /// <summary>
    /// Class that represents the UserRole table in the database
    /// </summary>
    public class UserRoleTable
    {
        private Database _database;


        public UserRoleTable(Database database)
        {
            _database = database;
        }


        public List<string> FindByUserId(string userId)
        {
            return
                _database.Fetch<string>(
                    "SELECT \r\n" +
                    "   r.Name \r\n" +
                    "FROM \r\n" +
                    "   UserRole ur \r\n" +
                    "   JOIN Role r ON(ur.RoleId = r.Id) \r\n" +
                    "WHERE \r\n" +
                    "   ur.UserId = @0",
                    userId
                );
        }


        public int Delete(string userId)
        {
            return
                _database.Execute(
                    "DELETE FROM \r\n" +
                    "   UserRole \r\n" +
                    "WHERE \r\n" +
                    "   UserId = @0",
                    userId
                );
        }


        public int Delete(string roleId, string userId)
        {
            return
                _database.Execute(
                    "DELETE FROM \r\n" +
                    "   UserRole \r\n" +
                    "WHERE \r\n" +
                    "   RoleId = @0 \r\n" +
                    "   AND UserId = @1",
                    roleId
                    , userId
                );

        }


        public int Insert(IdentityUser user, string roleId)
        {
            return
                _database.Execute(
                    "INSERT INTO UserRole (\r\n" +
                    "   UserId \r\n" +
                    "   ,RoleId) \r\n" +
                    "VALUES (\r\n" +
                    "   @0 \r\n" +
                    "   ,@1)",
                    user.Id
                    , roleId
                );
        }

    }

}
