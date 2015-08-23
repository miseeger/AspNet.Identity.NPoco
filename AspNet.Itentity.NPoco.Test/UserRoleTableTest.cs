using System;
using System.Collections.Generic;
using System.Linq;
using AspNet.Identity.NPoco;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NPoco;

namespace AspNet.Itentity.NPoco.Test
{

    [TestClass]
    public class UserRoleTableTest
    {

        private static Database _database;
        private static RoleTable _roleTable;
        private static UserTable<IdentityUser> _userTable;
        private static UserRoleTable _userRoleTable;

        private const string name1 = "John Doe";
        private const string name2 = "Jane Doe";
        private const string email1 = "john.doe@inter.net";
        private const string email2 = "jane.doe@inter.net";


        private const string role1 = "Development";
        private const string role2 = "Controlling";


        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            _database = new Database(IdentityConstants.ConnecionName);
            _roleTable = new RoleTable(_database);
            _userTable = new UserTable<IdentityUser>(_database);
            _userRoleTable = new UserRoleTable(_database);

            _userTable.Insert(new IdentityUser() { UserName = name1, Email = email1 });
            _userTable.Insert(new IdentityUser() { UserName = name2, Email = email2 });

            _roleTable.Insert(new IdentityRole() { Name = role1 });
            _roleTable.Insert(new IdentityRole() { Name = role2 });
        }


        [ClassCleanup]
        public static void ClassCleanup()
        {
            _database.Execute("DELETE FROM User");
            _database.Execute("DELETE FROM Role");
        }


        private static int CreateUserRole(IdentityUser user, string role)
        {
            return _userRoleTable.Insert(user, _roleTable.GetRoleId(role));
        }


        [TestMethod]
        public void It_creates_and_inserts_new_userrole()
        {
            string userRole;

            using (var transaction = _database.GetTransaction())
            {
                var user = _userTable.GetUserByName(name1).FirstOrDefault();
                CreateUserRole(user, role1);
                userRole = _userRoleTable.FindByUserId(user.Id).FirstOrDefault();
                transaction.Dispose();
            }

            Assert.AreEqual(role1, userRole);
        }


        [TestMethod]
        public void It_finds_roles_by_userid()
        {
            List<string> userRoles;

            using (var transaction = _database.GetTransaction())
            {
                var user = _userTable.GetUserByName(name1).FirstOrDefault();
                CreateUserRole(user, role1);
                CreateUserRole(user, role2);
                userRoles = _userRoleTable.FindByUserId(user.Id);
                transaction.Dispose();
            }

            Assert.IsTrue(userRoles.Contains(role1));
            Assert.IsTrue(userRoles.Contains(role2));
        }


        [TestMethod]
        public void It_deletes_all_roles_of_user()
        {
            List<string> userRoles;
            int result;

            using (var transaction = _database.GetTransaction())
            {
                var user = _userTable.GetUserByName(name1).FirstOrDefault();
                CreateUserRole(user, role1);
                CreateUserRole(user, role2);
                result = _userRoleTable.Delete(user.Id);
                userRoles = _userRoleTable.FindByUserId(user.Id);
                transaction.Dispose();
            }

            Assert.AreEqual(2, result);
            Assert.IsTrue(!userRoles.Any());
        }


        [TestMethod]
        public void It_deletes_one_role_of_a_user()
        {
            List<string> userRoles;
            int result;

            using (var transaction = _database.GetTransaction())
            {
                var user = _userTable.GetUserByName(name1).FirstOrDefault();
                CreateUserRole(user, role1);
                CreateUserRole(user, role2);
                var roleId = _roleTable.GetRoleId(role2);
                result = _userRoleTable.Delete(roleId, user.Id);
                userRoles = _userRoleTable.FindByUserId(user.Id);
                transaction.Dispose();
            }

            Assert.AreEqual(1, result);
            Assert.IsTrue(userRoles.Count == 1);
            Assert.IsTrue(userRoles.Contains(role1));
        }

    }

}


