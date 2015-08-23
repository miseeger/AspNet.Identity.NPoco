using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Identity;
using NPoco;

namespace AspNet.Identity.NPoco
{

    /// <summary>
    /// Class that implements the key ASP.NET Identity role store iterfaces
    /// </summary>
    public class RoleStore<TRole> : IQueryableRoleStore<TRole>
        where TRole : IdentityRole
    {
        private RoleTable roleTable;
        public Database Database { get; private set; }

        public RoleStore()
        {
            new RoleStore<TRole>(new Database(IdentityConstants.ConnecionName));
        }

        public RoleStore(Database database)
        {
            Database = database;
            roleTable = new RoleTable(database);
        }


        public IQueryable<TRole> Roles
        {
            get { return roleTable.GetRoles() as IQueryable<TRole>; }

        }


        public Task CreateAsync(TRole role)
        {
            if (role == null)
            {
                throw new ArgumentNullException(IdentityConstants.Role);
            }

            roleTable.Insert(role);

            return Task.FromResult<object>(null);
        }


        public Task DeleteAsync(TRole role)
        {
            if (role == null)
            {
                throw new ArgumentNullException(IdentityConstants.User);
            }

            roleTable.Delete(role.Id);

            return Task.FromResult<Object>(null);
        }


        public Task<TRole> FindByIdAsync(string roleId)
        {
            TRole result = roleTable.GetRoleById(roleId) as TRole;

            return Task.FromResult<TRole>(result);
        }


        public Task<TRole> FindByNameAsync(string roleName)
        {
            TRole result = roleTable.GetRoleByName(roleName) as TRole;

            return Task.FromResult<TRole>(result);
        }


        public Task UpdateAsync(TRole role)
        {
            if (role == null)
            {
                throw new ArgumentNullException(IdentityConstants.User);
            }

            roleTable.Update(role);

            return Task.FromResult<Object>(null);
        }


        public void Dispose()
        {
            if (Database != null)
            {
                Database.Dispose();
                Database = null;
            }
        }

    }

}
