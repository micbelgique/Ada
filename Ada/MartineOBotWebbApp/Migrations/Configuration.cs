using MartineOBotWebApp.Models.DAL;
using MartineOBotWebApp.Models.Entities;

namespace MartineOBotWebApp.Migrations
{
    using Microsoft.AspNet.Identity;
    using Microsoft.AspNet.Identity.EntityFramework;
    using System.Data.Entity.Migrations;

    internal sealed class Configuration : DbMigrationsConfiguration<ApplicationDbContext>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(ApplicationDbContext context)
        {
            #region Creation of roles and default 
            // Creation of roles
            RoleManager<IdentityRole> roleManager = new RoleManager<IdentityRole>
                (new RoleStore<IdentityRole>(context));

            // To avoid duplication
            if (!roleManager.RoleExists("admin"))
            {
                // Addition of roles in database
                roleManager.Create(new IdentityRole("user"));
                roleManager.Create(new IdentityRole("admin"));
                roleManager.Create(new IdentityRole("martineobot"));
            }

            // Creations of default user Admin - Passw0rd
            ApplicationUserManager userManager =
                new ApplicationUserManager(new UserStore<ApplicationUser>(context));

            // To avoid duplication
            if (userManager.FindByName("admin") == null)
            {
                ApplicationUser adminUser = new ApplicationUser { UserName = "admin", Email = "admin@mail.be" };
                userManager.Create(adminUser, "Passw0rd");
                userManager.AddToRole(adminUser.Id, "user");
                userManager.AddToRole(adminUser.Id, "admin");
            }

            if(userManager.FindByName("martineobot") == null)
            {
                ApplicationUser martineUser = new ApplicationUser { UserName = "martineobot", Email = "martineobot@mail.be" };

                userManager.Create(martineUser, "Passw0rd");
                userManager.AddToRole(martineUser.Id, "user");
                userManager.AddToRole(martineUser.Id, "martineobot");
            }
            #endregion

            base.Seed(context);
        }
    }
}
