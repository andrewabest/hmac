using System;
using System.Linq;
using System.Net.Http;
using System.Runtime.Caching;
using System.Threading.Tasks;

namespace Hmac
{
    public class SignatureValidator
    {
        private const int RequestMaxAgeInSeconds = 300;
        
        
        public async Task<bool> Validate(HttpRequestMessage request, string scope, string apiKey)
        {
            if (request.Headers.Contains("Authorization") == false) return false;

            var headerValues = GetAuthorizationHeaderValues(request.Headers.GetValues("Authorization").First());

            if (headerValues == null) return false;

            var incomingScope = headerValues[0];
            var nonce = headerValues[1];
            var timeStamp = headerValues[2];
            var signedHeaders = headerValues[3];
            var incomingSignature = headerValues[4];

            return await IsValidRequest(request, incomingScope, new Nonce(nonce), timeStamp, signedHeaders, incomingSignature, scope, apiKey);
        }

        private static string[] GetAuthorizationHeaderValues(string rawHeader)
        {
            if (rawHeader.StartsWith(SignatureProperties.AuthenticationScheme) == false) return new string[0];

            var credArray = rawHeader.Remove(0, SignatureProperties.AuthenticationScheme.Length).TrimStart().Split(':');

            return credArray.Length == 5 ? credArray : null;
        }

        private static async Task<bool> IsValidRequest(HttpRequestMessage request, string incomingScope, 
            Nonce nonce, string timeStamp, string signedHeaders, string incomingSignature, string scope, string apiKey)
        {
            if (!scope.Equals(incomingScope))
            {
                return false;
            }

            if (IsReplayRequest(nonce, timeStamp))
            {
                return false;
            }

            var payload = await request.Content.ReadAsStringAsync();

            var creator = new SignatureCreator();

            var comparisonSignature = creator.Create(
                request.Headers.Where(x => signedHeaders.Split(';').Contains(x.Key)), 
                request.RequestUri, 
                timeStamp,
                nonce, 
                payload, 
                incomingScope, 
                apiKey);

            var hmac = GetAuthorizationHeaderValues(comparisonSignature)[4];

            return incomingSignature.Equals(hmac);
        }

        private static bool IsReplayRequest(Nonce nonce, string requestTimeStamp)
        {
            var cachedValue = nonce.ToString();
            if (MemoryCache.Default.Contains(cachedValue))
            {
                return true;
            }

            var epochStart = new DateTime(1970, 01, 01, 0, 0, 0, 0, DateTimeKind.Utc);
            var currentTs = DateTime.UtcNow - epochStart;

            var serverTotalSeconds = Convert.ToUInt64(currentTs.TotalSeconds);
            var requestTotalSeconds = Convert.ToUInt64(requestTimeStamp);

            if ((serverTotalSeconds - requestTotalSeconds) > (ulong)RequestMaxAgeInSeconds)
            {
                return true;
            }

            MemoryCache.Default.Add(cachedValue, requestTimeStamp, DateTimeOffset.UtcNow.AddSeconds(RequestMaxAgeInSeconds));

            return false;
        }
    }
}
