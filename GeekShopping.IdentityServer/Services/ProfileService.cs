using System.Security.Claims;
using Duende.IdentityServer.Extensions;
using Duende.IdentityServer.Models;
using Duende.IdentityServer.Services;
using GeekShopping.IdentityServer.Model;
using IdentityModel;
using Microsoft.AspNetCore.Identity;

namespace GeekShopping.IdentityServer.Services;

public class ProfileService : IProfileService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly IUserClaimsPrincipalFactory<ApplicationUser> _claimsPrincipalFactory;

    public ProfileService(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager, IUserClaimsPrincipalFactory<ApplicationUser> claimsPrincipalFactory)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _claimsPrincipalFactory = claimsPrincipalFactory;
    }
    
    public async Task GetProfileDataAsync(ProfileDataRequestContext context)
    {
        string? subjectId = context.Subject.GetSubjectId();
        ApplicationUser? user = await _userManager!.FindByIdAsync(subjectId);
        if (user != null)
        {
            ClaimsPrincipal userClaims = await _claimsPrincipalFactory.CreateAsync(user);
            List<Claim> claims = userClaims.Claims.ToList();
            claims.Add(new Claim(JwtClaimTypes.FamilyName, user.LastName ?? string.Empty));
            claims.Add(new Claim(JwtClaimTypes.GivenName, user.FirstName ?? string.Empty));

            if (_userManager.SupportsUserRole)
            {
                IList<string> roles = await _userManager.GetRolesAsync(user);
                foreach (string role in roles)
                {
                    claims.Add(new Claim(JwtClaimTypes.Role, role));
                    if (_roleManager.SupportsRoleClaims)
                    {
                        IdentityRole? identityRole = await _roleManager.FindByNameAsync(role);
                        if (identityRole != null)
                        {
                            claims.AddRange(await _roleManager.GetClaimsAsync(identityRole));
                        }
                    }
                }
            }
            context.IssuedClaims = claims;
        }
    }

    public async Task IsActiveAsync(IsActiveContext context)
    {
        string? subjectId = context.Subject.GetSubjectId();
        ApplicationUser? user = await _userManager!.FindByIdAsync(subjectId);
        context.IsActive = user != null;
    }

}
