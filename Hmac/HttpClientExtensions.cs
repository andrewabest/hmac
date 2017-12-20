using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Hmac
{
    public static class HttpClientExtensions
    {
        public static Task<HttpResponseMessage> GetWithHmacAsync(this HttpClient client, Uri requestUri, string scope, string apiKey)
        {
            var header = new SignatureCreator().CreateWithoutContent(
                HttpMethod.Get,
                client.DefaultRequestHeaders,
                requestUri,
                DateTime.UtcNow.ToTimeStamp(),
                new Nonce(),
                scope,
                apiKey);

            client.DefaultRequestHeaders.Add(SignatureValidator.AuthorizationHeaderKey, header);

            return client.GetAsync(requestUri);
        }

        public static Task<HttpResponseMessage> PostAsJsonWithHmacAsync<T>(this HttpClient client, Uri requestUri, T content, string scope, string apiKey)
        {
            var serializedContent = JsonConvert.SerializeObject(content);

            var header = new SignatureCreator().Create(
                HttpMethod.Post,
                client.DefaultRequestHeaders, 
                requestUri, 
                DateTime.UtcNow.ToTimeStamp(), 
                new Nonce(), 
                serializedContent, 
                scope, 
                apiKey);

            client.DefaultRequestHeaders.Add(SignatureValidator.AuthorizationHeaderKey, header);

            return client.PostAsync(requestUri, new StringContent(serializedContent, Encoding.UTF8, "application/json"));
        }
    }
}