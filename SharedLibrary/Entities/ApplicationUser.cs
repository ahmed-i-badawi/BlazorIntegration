using Microsoft.AspNetCore.Identity;

namespace SharedLibrary.Entities;

public class ApplicationUser : IdentityUser
{
    public string? FullName { get; set; }
    public Site? Site { get; set; }
}
