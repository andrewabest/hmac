using System.Web.Http;
using Hmac.WebAPI;
using Owin;

namespace Hmac.Tests.API
{
    public class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            var config = new HttpConfiguration();

            config.MapHttpAttributeRoutes();

            HmacConfiguration
                .Create()
                .WithApiKey("123456")
                .WithScope("MyApp")
                .Configure();

            app.UseWebApi(config);
        }
    }
}