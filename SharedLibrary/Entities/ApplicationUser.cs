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
            if (SiteId != null)
            {
                return UserType.Site;
            }
            else if (IntegratorId != null)
            {
                return UserType.Integrator;
            }

            return UserType.NotDefined;
        }
        private set { }
    }

    public int? SiteId { get; set; }
    public Site? Site { get; set; }

    public string? IntegratorId { get; set; }
    public Integrator? Integrator { get; set; }

}
