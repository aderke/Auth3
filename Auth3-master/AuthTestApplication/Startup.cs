using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(AuthTestApplication.Startup))]
namespace AuthTestApplication
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}