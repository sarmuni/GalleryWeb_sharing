using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(GalleryWeb.Startup))]
namespace GalleryWeb
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
