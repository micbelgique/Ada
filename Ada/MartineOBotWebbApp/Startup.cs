using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(MartineOBotWebApp.Startup))]
namespace MartineOBotWebApp
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
