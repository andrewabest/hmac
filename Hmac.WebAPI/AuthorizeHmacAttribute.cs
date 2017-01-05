using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Controllers;
using System.Web.Http.Filters;

namespace Hmac.WebAPI
{
    public class AuthorizeHmacAttribute : AuthorizationFilterAttribute
    {
        public override async Task OnAuthorizationAsync(HttpActionContext actionContext, CancellationToken cancellationToken)
        {
            var success = await new SignatureValidator().Validate(actionContext.Request, Hmac.Configuration.Scope, Hmac.Configuration.ApiKey);

            if (success == false)
            {
                actionContext.Response =  new HttpResponseMessage(HttpStatusCode.Unauthorized);
            }
        }
    }
}