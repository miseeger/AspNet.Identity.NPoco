using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using NPoco;

namespace AspNet.Identity.NPoco
{

    /// <summary>
    /// Class that implements the key ASP.NET Identity user store interfaces
    /// </summary>
    public class UserStore<TUser> : IUserLoginStore<TUser>,
        IUserClaimStore<TUser>,
        IUserRoleStore<TUser>,
        IUserPasswordStore<TUser>,
        IUserSecurityStampStore<TUser>,
        IQueryableUserStore<TUser>,
        IUserEmailStore<TUser>,
        IUserPhoneNumberStore<TUser>,
        IUserTwoFactorStore<TUser, string>,
        IUserLockoutStore<TUser, string>,
        IUserStore<TUser>
        where TUser : IdentityUser
    {
        private UserTable<TUser> _userTable;
        private RoleTable _roleTable;
        private UserRoleTable _userRoleTable;
        private UserClaimTable _userClaimTable;
        private UserLoginTable _userLoginTable;

        public Database NPocoDb { get; private set; }

        public IQueryable<TUser> Users
        {
            get
            {
                return _userTable.GetUsers().AsQueryable();
            }
        }

        public UserStore()
        {
            new UserStore<TUser>(new Database(IdentityConstants.ConnecionName));
        }

        public UserStore(Database nPocoDb)
        {
            NPocoDb = nPocoDb;
            _userTable = new UserTable<TUser>(nPocoDb);
            _roleTable = new RoleTable(nPocoDb);
            _userRoleTable = new UserRoleTable(nPocoDb);
            _userClaimTable = new UserClaimTable(nPocoDb);
            _userLoginTable = new UserLoginTable(nPocoDb);
        }

        
        public Task CreateAsync(TUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(IdentityConstants.User);
            }

            _userTable.Insert(user);

            return Task.FromResult<object>(null);
        }


        public Task<TUser> FindByIdAsync(string userId)
        {
            if (string.IsNullOrEmpty(userId))
            {
                throw new ArgumentException(IdentityConstants.NoUserId);
            }

            TUser result = _userTable.GetUserById(userId) as TUser;
            if (result != null)
            {
                return Task.FromResult<TUser>(result);
            }

            return Task.FromResult<TUser>(null);
        }


        public Task<TUser> FindByNameAsync(string userName)
        {
            if (string.IsNullOrEmpty(userName))
            {
                throw new ArgumentException(IdentityConstants.NoUserName);
            }

            List<TUser> result = _userTable.GetUserByName(userName) as List<TUser>;

            // Should I throw if > 1 user?
            if (result != null && result.Count == 1)
            {
                return Task.FromResult<TUser>(result[0]);
            }

            return Task.FromResult<TUser>(null);
        }


        public Task UpdateAsync(TUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(IdentityConstants.User);
            }

            _userTable.Update(user);

            return Task.FromResult<object>(null);
        }


        public void Dispose()
        {
            if (NPocoDb != null)
            {
                NPocoDb.Dispose();
                NPocoDb = null;
            }
        }


        public Task AddClaimAsync(TUser user, Claim claim)
        {
            if (user == null)
            {
                throw new ArgumentNullException(IdentityConstants.User);
            }

            if (claim == null)
            {
                throw new ArgumentNullException(IdentityConstants.User);
            }

            _userClaimTable.Insert(claim, user.Id);

            return Task.FromResult<object>(null);
        }


        public Task<IList<Claim>> GetClaimsAsync(TUser user)
        {
            ClaimsIdentity identity = _userClaimTable.FindByUserId(user.Id);

            return Task.FromResult<IList<Claim>>(identity.Claims.ToList());
        }


        public Task RemoveClaimAsync(TUser user, Claim claim)
        {
            if (user == null)
            {
                throw new ArgumentNullException(IdentityConstants.User);
            }

            if (claim == null)
            {
                throw new ArgumentNullException(IdentityConstants.Claim);
            }

            _userClaimTable.Delete(user, claim);

            return Task.FromResult<object>(null);
        }


        public Task AddLoginAsync(TUser user, UserLoginInfo login)
        {
            if (user == null)
            {
                throw new ArgumentNullException(IdentityConstants.User);
            }

            if (login == null)
            {
                throw new ArgumentNullException(IdentityConstants.Login);
            }

            _userLoginTable.Insert(user, login);

            return Task.FromResult<object>(null);
        }


        public Task<TUser> FindAsync(UserLoginInfo login)
        {
            if (login == null)
            {
                throw new ArgumentNullException(IdentityConstants.Login);
            }

            var userId = _userLoginTable.FindUserIdByLogin(login);
            if (userId != null)
            {
                TUser user = _userTable.GetUserById(userId) as TUser;
                if (user != null)
                {
                    return Task.FromResult<TUser>(user);
                }
            }

            return Task.FromResult<TUser>(null);
        }


        public Task<IList<UserLoginInfo>> GetLoginsAsync(TUser user)
        {
            List<UserLoginInfo> userLogins = new List<UserLoginInfo>();
            if (user == null)
            {
                throw new ArgumentNullException(IdentityConstants.User);
            }

            List<UserLoginInfo> logins = _userLoginTable.FindByUserId(user.Id);
            if (logins != null)
            {
                return Task.FromResult<IList<UserLoginInfo>>(logins);
            }

            return Task.FromResult<IList<UserLoginInfo>>(null);
        }


        public Task RemoveLoginAsync(TUser user, UserLoginInfo login)
        {
            if (user == null)
            {
                throw new ArgumentNullException(IdentityConstants.User);
            }

            if (login == null)
            {
                throw new ArgumentNullException(IdentityConstants.Login);
            }

            _userLoginTable.Delete(user, login);

            return Task.FromResult<Object>(null);
        }


        public Task AddToRoleAsync(TUser user, string roleName)
        {
            if (user == null)
            {
                throw new ArgumentNullException(IdentityConstants.User);
            }

            if (string.IsNullOrEmpty(roleName))
            {
                throw new ArgumentException(IdentityConstants.NoRolename);
            }

            string roleId = _roleTable.GetRoleId(roleName);
            if (!string.IsNullOrEmpty(roleId))
            {
                _userRoleTable.Insert(user, roleId);
            }

            return Task.FromResult<object>(null);
        }


        public Task<IList<string>> GetRolesAsync(TUser user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(IdentityConstants.User);
            }

            List<string> roles = _userRoleTable.FindByUserId(user.Id);
            {
                if (roles != null)
                {
                    return Task.FromResult<IList<string>>(roles);
                }
            }

            return Task.FromResult<IList<string>>(null);
        }


        public Task<bool> IsInRoleAsync(TUser user, string role)
        {
            if (user == null)
            {
                throw new ArgumentNullException(IdentityConstants.User);
            }

            if (string.IsNullOrEmpty(role))
            {
                throw new ArgumentNullException(IdentityConstants.Role);
            }

            List<string> roles = _userRoleTable.FindByUserId(user.Id);
            {
                if (roles != null && roles.Contains(role))
                {
                    return Task.FromResult<bool>(true);
                }
            }

            return Task.FromResult<bool>(false);
        }


        public Task RemoveFromRoleAsync(TUser user, string role)
        {
            var roleId = _roleTable.GetRoleId(role);

            return
                Task.FromResult<int>(
                    string.IsNullOrEmpty(roleId)
                        ? 0
                        : _userRoleTable.Delete(roleId, user.Id)
                );
        }


        public Task DeleteAsync(TUser user)
        {
            if (user != null)
            {
                _userTable.Delete(user);
            }

            return Task.FromResult<Object>(null);
        }


        public Task<string> GetPasswordHashAsync(TUser user)
        {
            string passwordHash = _userTable.GetPasswordHash(user.Id);

            return Task.FromResult<string>(passwordHash);
        }


        public Task<bool> HasPasswordAsync(TUser user)
        {
            var hasPassword = !string.IsNullOrEmpty(_userTable.GetPasswordHash(user.Id));

            return Task.FromResult<bool>(Boolean.Parse(hasPassword.ToString()));
        }


        public Task SetPasswordHashAsync(TUser user, string passwordHash)
        {
            user.PasswordHash = passwordHash;

            return Task.FromResult<Object>(null);
        }


        public Task SetSecurityStampAsync(TUser user, string stamp)
        {
            user.SecurityStamp = stamp;

            return Task.FromResult(0);

        }


        public Task<string> GetSecurityStampAsync(TUser user)
        {
            return Task.FromResult(user.SecurityStamp);
        }


        public Task SetEmailAsync(TUser user, string email)
        {
            user.Email = email;
            _userTable.Update(user);

            return Task.FromResult(0);

        }


        public Task<string> GetEmailAsync(TUser user)
        {
            return Task.FromResult(user.Email);
        }


        public Task<bool> GetEmailConfirmedAsync(TUser user)
        {
            return Task.FromResult(user.EmailConfirmed);
        }


        public Task SetEmailConfirmedAsync(TUser user, bool confirmed)
        {
            user.EmailConfirmed = confirmed;
            _userTable.Update(user);

            return Task.FromResult(0);
        }


        public Task<TUser> FindByEmailAsync(string email)
        {
            if (String.IsNullOrEmpty(email))
            {
                throw new ArgumentNullException(IdentityConstants.Email);
            }

            TUser result = _userTable.GetUserByEmail(email) as TUser;
            if (result != null)
            {
                return Task.FromResult<TUser>(result);
            }

            return Task.FromResult<TUser>(null);
        }


        public Task SetPhoneNumberAsync(TUser user, string phoneNumber)
        {
            user.PhoneNumber = phoneNumber;
            _userTable.Update(user);

            return Task.FromResult(0);
        }


        public Task<string> GetPhoneNumberAsync(TUser user)
        {
            return Task.FromResult(user.PhoneNumber);
        }


        public Task<bool> GetPhoneNumberConfirmedAsync(TUser user)
        {
            return Task.FromResult(user.PhoneNumberConfirmed);
        }


        public Task SetPhoneNumberConfirmedAsync(TUser user, bool confirmed)
        {
            user.PhoneNumberConfirmed = confirmed;
            _userTable.Update(user);

            return Task.FromResult(0);
        }


        public Task SetTwoFactorEnabledAsync(TUser user, bool enabled)
        {
            user.TwoFactorEnabled = enabled;
            _userTable.Update(user);

            return Task.FromResult(0);
        }


        public Task<bool> GetTwoFactorEnabledAsync(TUser user)
        {
            return Task.FromResult(user.TwoFactorEnabled);
        }


        public Task<DateTimeOffset> GetLockoutEndDateAsync(TUser user)
        {
            return
                Task.FromResult(user.LockoutEndDateUtc.HasValue
                    ? new DateTimeOffset(DateTime.SpecifyKind(user.LockoutEndDateUtc.Value, DateTimeKind.Utc))
                    : new DateTimeOffset());
        }


        public Task SetLockoutEndDateAsync(TUser user, DateTimeOffset lockoutEnd)
        {
            user.LockoutEndDateUtc = lockoutEnd.UtcDateTime;
            _userTable.Update(user);

            return Task.FromResult(0);
        }


        public Task<int> IncrementAccessFailedCountAsync(TUser user)
        {
            user.AccessFailedCount++;
            _userTable.Update(user);

            return Task.FromResult(user.AccessFailedCount);
        }


        public Task ResetAccessFailedCountAsync(TUser user)
        {
            user.AccessFailedCount = 0;
            _userTable.Update(user);

            return Task.FromResult(0);
        }


        public Task<int> GetAccessFailedCountAsync(TUser user)
        {
            return Task.FromResult(user.AccessFailedCount);
        }


        public Task<bool> GetLockoutEnabledAsync(TUser user)
        {
            return Task.FromResult(user.LockoutEnabled);
        }


        public Task SetLockoutEnabledAsync(TUser user, bool enabled)
        {
            user.LockoutEnabled = enabled;
            _userTable.Update(user);

            return Task.FromResult(0);
        }

    }

}
