using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin.Security;

namespace AdaWebApp.Models.DAL.Repositories
{
    public class AuthRepository
    {
        #region Definition of Managers
        private ApplicationUserManager _applicationUserManager;
        public ApplicationUserManager ApplicationUserManager {
            get { return _applicationUserManager ?? HttpContext.Current.GetOwinContext().GetUserManager<ApplicationUserManager>(); }
            set { _applicationUserManager = value;  }
        }

        private ApplicationSignInManager _applicationSignInManager;
        public ApplicationSignInManager ApplicationSignInManager
        {
            get { return _applicationSignInManager ?? HttpContext.Current.GetOwinContext().Get<ApplicationSignInManager>(); }
            set { _applicationSignInManager = value; }
        }

        private ApplicationRoleManager _applicationRoleManager;
        public ApplicationRoleManager ApplicationRoleManager
        {
            get { return _applicationRoleManager ?? HttpContext.Current.GetOwinContext().Get<ApplicationRoleManager>(); }
            set { _applicationRoleManager = value; }
        }

        public IAuthenticationManager AuthenticationManager => 
            HttpContext.Current.GetOwinContext().Authentication;

        #endregion

        public async Task<IdentityUser> FindUser(string userName, string password)
        {
            return await ApplicationUserManager.FindAsync(userName, password); 
        }
    }
}