using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(WebFarmNodeApp.Startup))]
namespace WebFarmNodeApp
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
