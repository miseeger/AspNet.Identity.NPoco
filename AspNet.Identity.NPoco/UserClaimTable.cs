using System.Collections.Generic;
using System.Security.Claims;
using NPoco;

namespace AspNet.Identity.NPoco
{

    /// <summary>
    /// Class that represents the UserClaim table in the database
    /// </summary>
    public class UserClaimTable
    {
        private Database _database;

        public UserClaimTable(Database database)
        {
            _database = database;
        }


        public ClaimsIdentity FindByUserId(string userId)
        {
            var userClaims = _database.Fetch<IdentityUserClaim>(
                    "SELECT \r\n" +
                    "   * \r\n" +
                    "FROM \r\n" +
                    "   UserClaim \r\n" +
                    "WHERE \r\n" +
                    "   UserId = @0",
                    userId
                );

            var claims = new ClaimsIdentity();
            foreach (var userClaim in userClaims)
            {
                var claim = new Claim(userClaim.ClaimType, userClaim.ClaimValue);
                claims.AddClaim(claim);
            }

            return claims;
        }


        public int Delete(string userId)
        {
            return
                _database.Execute(
                    "DELETE FROM \r\n" +
                    "   UserClaim \r\n" +
                    "WHERE \r\n" +
                    "   UserId = @0",
                    userId
                );
        }


        public int Insert(Claim userClaim, string userId)
        {
            return
                _database.Insert("UserClaim", "",
                    new IdentityUserClaim()
                    {
                        Id = RandomIdGenerator.GetBase62("UC", 10)
                        , ClaimType = userClaim.Type
                        , ClaimValue = userClaim.Value
                        , UserId = userId
                    }
                ) != null
                    ? 1
                    : 0;
        }


        public int Delete(IdentityUser user, Claim claim)
        {
            return
                _database.Execute(
                    "DELETE FROM \r\n" +
                    "   UserClaim \r\n" +
                    "WHERE \r\n" +
                    "   UserId = @0 \r\n" +
                    "   AND ClaimValue = @1 \r\n" +
                    "   AND ClaimType = @2",
                    user.Id
                    , claim.Value
                    , claim.Type
                );
        }

    }

}
