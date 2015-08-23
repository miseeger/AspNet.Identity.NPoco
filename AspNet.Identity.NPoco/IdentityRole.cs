using Microsoft.AspNet.Identity;

namespace AspNet.Identity.NPoco
{

    public class IdentityRole : IRole
    {

        public IdentityRole()
        {
            Id = RandomIdGenerator.GetBase62("R", 11);
        }

        public IdentityRole(string name) : this()
        {
            Name = name;
        }

        public IdentityRole(string name, string id)
        {
            Name = name;
            Id = id;
        }


        public string Id { get; set; }
        public string Name { get; set; }

    }

}
