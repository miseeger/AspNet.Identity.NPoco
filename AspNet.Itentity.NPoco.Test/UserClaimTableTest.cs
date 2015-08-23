using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using AspNet.Identity.NPoco;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NPoco;

namespace AspNet.Itentity.NPoco.Test
{

    [TestClass]
    public class UserClaimTableTest
    {

        private static Database _database;
        private static UserTable<IdentityUser> _userTable;
        private static UserClaimTable _userClaimTable;

        private const string name1 = "John Doe";
        private const string name2 = "Jane Doe";
        private const string email1 = "john.doe@inter.net";
        private const string email2 = "jane.doe@inter.net";

        private const string cType1 = ClaimTypes.Country;
        private const string cValue1 = "NoMansLand";

        private const string cType2 = ClaimTypes.Gender;
        private const string cValue2 = "male";


        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            _database = new Database(IdentityConstants.ConnecionName);
            _userTable = new UserTable<IdentityUser>(_database);
            _userClaimTable = new UserClaimTable(_database);

            _userTable.Insert(new IdentityUser() { UserName = name1, Email = email1 });
            _userTable.Insert(new IdentityUser() { UserName = name2, Email = email2 });
        }


        [ClassCleanup]
        public static void ClassCleanup()
        {
            _database.Execute("DELETE FROM User");
        }


        private static int CreateUserClaim(IdentityUser user, string type, string value)
        {
            return _userClaimTable.Insert(new Claim(type, value), user.Id);
        }


        [TestMethod]
        public void It_creates_and_inserts_new_userclaim()
        {
            ClaimsIdentity claims;

            using (var transaction = _database.GetTransaction())
            {
                var user = _userTable.GetUserByName(name1).FirstOrDefault();
                CreateUserClaim(user, cType1, cValue1);
                claims = _userClaimTable.FindByUserId(user.Id);
                transaction.Dispose();
            }

            Assert.IsTrue(claims.HasClaim(cType1, cValue1));
        }


        [TestMethod]
        public void It_finds_userclaims_by_userid()
        {
            ClaimsIdentity claims;

            using (var transaction = _database.GetTransaction())
            {
                var user = _userTable.GetUserByName(name1).FirstOrDefault();
                CreateUserClaim(user, cType1, cValue1);
                CreateUserClaim(user, cType2, cValue2);
                claims = _userClaimTable.FindByUserId(user.Id);
                transaction.Dispose();
            }

            Assert.IsTrue(claims.HasClaim(cType1, cValue1));
            Assert.IsTrue(claims.HasClaim(cType2, cValue2));
        }


        [TestMethod]
        public void It_deletes_all_userclaims_of_user()
        {
            int result;
            ClaimsIdentity claims;

            using (var transaction = _database.GetTransaction())
            {
                var user = _userTable.GetUserByName(name1).FirstOrDefault();
                CreateUserClaim(user, cType1, cValue1);
                result = _userClaimTable.Delete(user.Id);
                claims = _userClaimTable.FindByUserId(user.Id);
                transaction.Dispose();
            }

            Assert.AreEqual(1, result);
            Assert.IsTrue(!claims.Claims.Any());
        }


        [TestMethod]
        public void It_deletes_one_userClaim_of_a_user()
        {
            int result;
            ClaimsIdentity claims;

            using (var transaction = _database.GetTransaction())
            {
                var user = _userTable.GetUserByName(name1).FirstOrDefault();
                CreateUserClaim(user, cType1, cValue1);
                CreateUserClaim(user, cType2, cValue2);
                result = _userClaimTable.Delete(user, new Claim(cType2, cValue2));
                claims = _userClaimTable.FindByUserId(user.Id);
                transaction.Dispose();
            }


            Assert.AreEqual(1, result);
            Assert.IsTrue(claims.Claims.Count() == 1);
            Assert.IsTrue(claims.HasClaim(cType1, cValue1));
        }

    }

}


