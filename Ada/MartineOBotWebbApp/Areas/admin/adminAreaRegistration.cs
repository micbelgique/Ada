using System.Web.Mvc;

namespace MartineOBotWebApp.Areas.Admin
{
    public class AdminAreaRegistration : AreaRegistration 
    {
        public override string AreaName 
        {
            get 
            {
                return "Admin";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context) 
        {
            context.MapRoute(
                    name: "StaffMember",
                    url: "Admin/StaffMember/DeleteUnavailability/{id}/{memberId}",
                    defaults: new { controller = "StaffMember", action = "DeleteUnavailability" });
            context.MapRoute(
                "Admin_default",
                "Admin/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}