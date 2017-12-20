using System.Web.Http;
using Hmac.WebAPI;

namespace Hmac.Tests.API.Controllers
{
    [RoutePrefix("api")]
    public class TestController : ApiController
    {
        [Route("post")]
        [AuthorizeHmac]
        [HttpPost]
        public IHttpActionResult Post([FromBody]string payload)
        {
            return Ok();
        }

        [Route("get")]
        [AuthorizeHmac]
        [HttpGet]
        public IHttpActionResult Get([FromBody]string payload)
        {
            return Ok();
        }
    }
}
