using Microsoft.AspNetCore.Identity;
using SharedLibrary.Enums;

namespace SharedLibrary.Entities;

public class ApplicationUser : IdentityUser
{
    public string? FullName { get; set; }
    public bool IsActive { get; set; }
    public UserType UserType
    {
        get
        {
            if (Site != null)
            {
                return UserType.Site;
            }
            else if (Integrator != null)
            {
                return UserType.Integrator;
            }

            return UserType.NotDefined;
        }
        private set { }
    }

    public Site? Site { get; set; }
    public Integrator? Integrator { get; set; }

}
