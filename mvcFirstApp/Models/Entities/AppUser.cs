using Microsoft.AspNetCore.Identity;

namespace mvcFirstApp.Models.Entities
{
    public class AppUser : IdentityUser // <> is for the primary key type its GUID by default
    {
        public string FullName { get; set; } = string.Empty;

        // Add here any additional properties or relationships you need for the user
    }
}
