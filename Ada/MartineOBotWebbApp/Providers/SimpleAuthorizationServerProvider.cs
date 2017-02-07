using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security.OAuth;
using System.Security.Claims;
using System.Threading.Tasks;
using MartineOBotWebApp.Models.DAL.Repositories;

namespace MartineOBotWebApp.Providers
{
    public class SimpleAuthorizationServerProvider : OAuthAuthorizationServerProvider
    {
        public override async Task ValidateClientAuthentication(OAuthValidateClientAuthenticationContext context)
        {  
            // This call is required...
            // but we're not using client authentication, so validate and move on...
            await Task.FromResult(context.Validated()); 
        }

        public override async Task GrantResourceOwnerCredentials(OAuthGrantResourceOwnerCredentialsContext context)
        {
            AuthRepository authRepo = new AuthRepository(); 
            IdentityUser user = await authRepo.FindUser(context.UserName, context.Password);

            if (user == null)
            {
                context.SetError("invalid_grant", "The user name or password is incorrect.");
                return;
            }
            
            // Create or retrieve a ClaimsIdentity to represent the 
            // Authenticated user:
            var identity = new ClaimsIdentity(context.Options.AuthenticationType);
            identity.AddClaim(new Claim("sub", context.UserName));

            // Add user's roles to the claim
            foreach(IdentityUserRole role in user.Roles){
                identity.AddClaim(new Claim(ClaimTypes.Role,
                    authRepo.ApplicationRoleManager.FindByIdAsync(role.RoleId).Result.Name));
            }

            // Identity info will ultimately be encoded into an Access Token
            // as a result of this call:
            context.Validated(identity);
        }
    }
}
