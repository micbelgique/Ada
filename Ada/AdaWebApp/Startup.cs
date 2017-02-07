using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(AdaWebApp.Startup))]
namespace AdaWebApp
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
