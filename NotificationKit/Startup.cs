using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(NotificationKit.Startup))]

namespace NotificationKit
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
