using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(MemberIntegralMS.Startup))]
namespace MemberIntegralMS
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
