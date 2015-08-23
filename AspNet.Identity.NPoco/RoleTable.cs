using System.Collections.Generic;
using NPoco;

namespace AspNet.Identity.NPoco
{

    /// <summary>
    /// Class that represents the Role table in the database
    /// </summary>
    public class RoleTable 
    {
        private Database _database;

        public RoleTable(Database database)
        {
            _database = database;
        }


        public int Delete(string roleId)
        {
            return 
                _database.Execute(
                    "DELETE FROM \r\n" +
                    "   Role \r\n" +
                    "WHERE \r\n" +
                    "   Id = @0", 
                    roleId);
        }


        public int Insert(IdentityRole role)
        {
            return _database.Insert("Role", "", role) != null ? 1 : 0;
        }


        public IEnumerable<IdentityRole> GetRoles()
        {
            return
                _database.Fetch<IdentityRole>(
                    "SELECT \r\n" +
                    "   * \r\n" +
                    "FROM \r\n" +
                    "   Role"
                );
        }


        public string GetRoleName(string roleId)
        {
           return 
                _database.ExecuteScalar<string>(
                    "SELECT \r\n" +
                    "   Name \r\n" +
                    "FROM \r\n" +
                    "   Role \r\n" +
                    "WHERE \r\n" +
                    "   Id = @0",
                    roleId);
        }


        public string GetRoleId(string roleName)
        {
            return 
                _database.ExecuteScalar<string>(
                    "SELECT \r\n" +
                    "   Id \r\n" +
                    "FROM \r\n" +
                    "   Role \r\n" +
                    "WHERE \r\n" +
                    "   Name = @0",
                    roleName);
        }


        public IdentityRole GetRoleById(string roleId)
        {
            return
                _database.FirstOrDefault<IdentityRole>(
                    "SELECT \r\n" +
                    "   * \r\n" +
                    "FROM \r\n" +
                    "   Role \r\n" +
                    "WHERE \r\n" +
                    "   Id = @0",
                    roleId
                );
        }


        public IdentityRole GetRoleByName(string roleName)
        {
            return
                _database.FirstOrDefault<IdentityRole>(
                    "SELECT \r\n" +
                    "   * \r\n" +
                    "FROM \r\n" +
                    "   Role \r\n" +
                    "WHERE \r\n" +
                    "   Name = @0",
                    roleName
                );
        }


        public int Update(IdentityRole role)
        {
            return
                _database.Update("Role", "Id", role);

        }

    }

}
