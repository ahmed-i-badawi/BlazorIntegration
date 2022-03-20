using Microsoft.AspNetCore.Identity;

namespace BlazorServer.Data.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }
}
