using AdaWebApp.ViewModels;
using Microsoft.AspNet.Identity.Owin;
using System.Threading.Tasks;
using System.Web.Mvc;
using AdaWebApp.Models.DAL.Repositories;

namespace AdaWebApp.Controllers
{
    public class AccountController : Controller
    {
        private readonly AuthRepository _authRepo;

        public AccountController(){
            _authRepo = new AuthRepository(); 
        }

        // GET: Account
        public ActionResult Login(string returnUrl)
        {
            if (HttpContext.User.Identity.IsAuthenticated)
                return RedirectToAction("Index", "Home");

            ViewBag.ReturnUrl = returnUrl; 
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(IdentityLoginViewModel model, string returnUrl)
        {
            if (!ModelState.IsValid)
                return View(model);

            var result = await _authRepo.ApplicationSignInManager.PasswordSignInAsync
                (model.Username, model.Password, model.CheckMe, false);

            switch (result)
            {
                case SignInStatus.Success:

                    if (Url.IsLocalUrl(returnUrl))
                        return Redirect(returnUrl);

                    return RedirectToAction("Index", "Home");

                default:
                    ModelState.AddModelError("loginError", "Your username or password is wrong");
                    return View(model);
            }
        }

        public ActionResult Logout()
        { 
            _authRepo.AuthenticationManager.SignOut();
            return RedirectToAction("Index", "Home"); 
        }
    }
}