
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using SharedLibrary.Entities;
using System.Security.Claims;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace BlazorServer.Services;

public class CustomAuthorize : System.Web.Http.AuthorizeAttribute
{
    //private readonly PermissionAction[] permissionActions;

    //public CustomAuthorize(PermissionItem item, params PermissionAction[] permissionActions)
    //{
    //    this.permissionActions = permissionActions;
    //}

    protected override Boolean IsAuthorized(HttpActionContext actionContext)
    {
        var currentIdentity = actionContext.RequestContext.Principal.Identity;
        if (!currentIdentity.IsAuthenticated)
            return false;

        var userName = currentIdentity.Name;
        //using (var context = new DataContext())
        //{
        //    var userStore = new UserStore<ApplicationUser>(context);
        //    var userManager = new UserManager<ApplicationUser>(userStore);
        //    var user = userManager.FindByName(userName);

        //    if (user == null)
        //        return false;

        //    foreach (var role in permissionActions)
        //        if (!userManager.IsInRole(user.Id, Convert.ToString(role)))
        //            return false;

            return true;
        //}
    }
}

public class ClaimRequirementAttribute : TypeFilterAttribute
{
    public ClaimRequirementAttribute(string claimType, string claimValue) : base(typeof(ClaimRequirementFilter))
    {
        Arguments = new object[] { new Claim(claimType, claimValue) };
    }
}

public class ClaimRequirementFilter : Microsoft.AspNetCore.Mvc.Filters.IAuthorizationFilter
{
    readonly Claim _claim;

    public ClaimRequirementFilter(Claim claim)
    {
        _claim = claim;
    }

    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var hasClaim = context.HttpContext.User.Claims.Any(c => c.Type == _claim.Type && c.Value == _claim.Value);
        if (!hasClaim)
        {
            context.Result = new ForbidResult();
        }
    }
}