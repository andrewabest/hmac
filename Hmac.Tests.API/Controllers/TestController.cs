using System.Web.Http;
using Hmac.WebAPI;

namespace Hmac.Tests.API.Controllers
{
    [RoutePrefix("api")]
    public class TestController : ApiController
    {
        [Route("test")]
        [AuthorizeHmac]
        [HttpPost]
        public IHttpActionResult Index([FromBody]string payload)
        {
            return Ok();
        }
    }
}
