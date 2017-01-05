using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Hmac
{
    public static class HttpClientExtensions
    {
        public static Task<HttpResponseMessage> PostAsJsonWithHmacAsync<T>(this HttpClient client, Uri requestUri, T value)
        {
            var payload = JsonConvert.SerializeObject(value);

            var header = new SignatureCreator().Create(
                client.DefaultRequestHeaders, 
                requestUri, 
                DateTime.UtcNow.ToTimeStamp(), 
                new Nonce(), 
                payload, 
                "Scope", 
                "ApiKey");

            client.DefaultRequestHeaders.Add("Authorization", header);

            return client.PostAsync(requestUri, new StringContent(payload, Encoding.UTF8, "application/json"));
        }
    }
}