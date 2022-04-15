using Microsoft.AspNetCore.Identity;

namespace BlazorServer.Data.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public string? Name { get; set; }
    }
}
