using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(StyleCop.Analyzers.Status.Web.Startup))]
namespace StyleCop.Analyzers.Status.Web
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
        }
    }
}
