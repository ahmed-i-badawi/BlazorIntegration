using BlazorServer.Services;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication.Internal;
using SharedLibrary.Dto;
using System.Linq;
using System.Security.Claims;
using System.Text.Json;
using System.Threading.Tasks;

public class CustomUserFactory : AccountClaimsPrincipalFactory<RemoteUserAccount>
{
    private readonly IClientOperations _clientOperations;

    public CustomUserFactory(IAccessTokenProviderAccessor accessor, IClientOperations clientOperations)
        : base(accessor)
    {
        _clientOperations = clientOperations;
    }

    public async override ValueTask<ClaimsPrincipal> CreateUserAsync(
        RemoteUserAccount account,
        RemoteAuthenticationUserOptions options)
    {
        var user = await base.CreateUserAsync(account, options);

        if (user.Identity.IsAuthenticated)
        {
            var identity = (ClaimsIdentity)user.Identity;
            var roleClaims = identity.FindAll(identity.RoleClaimType);

            if (roleClaims != null && roleClaims.Any())
            {
                var existingClaims = new System.Collections.Generic.List<Claim>();

                foreach (var existingClaim in roleClaims)
                {
                    existingClaims.Add(existingClaim);
                }
                foreach (var existingClaim in existingClaims)
                {
                    identity.RemoveClaim(existingClaim);
                }

                var rolesElem = account.AdditionalProperties[identity.RoleClaimType];

                if (rolesElem is JsonElement roles)
                {
                    if (roles.ValueKind == JsonValueKind.Array)
                    {
                        foreach (var role in roles.EnumerateArray())
                        {
                            identity.AddClaim(new Claim(options.RoleClaim, role.GetString()));
                        }
                    }
                    else
                    {
                        identity.AddClaim(new Claim(options.RoleClaim, roles.GetString()));
                    }
                }
            }

          
        }



        return user;
    }
}