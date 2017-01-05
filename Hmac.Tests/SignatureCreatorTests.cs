using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using NUnit.Framework;
using Shouldly;

namespace Hmac.Tests
{
    public class SignatureCreatorTests
    {
        private readonly Uri _applicationUri = new Uri("http://myapplication.com");
        private readonly string _scope = "MyApplication";
        private readonly string _apiKey = "123456";
        private readonly string _requestPayload = "payload";

        [Test]
        public async Task CreatedSignatureCanBeValidated()
        {
            var sut = new SignatureCreator();

            IEnumerable<KeyValuePair<string, IEnumerable<string>>> headers = new List<KeyValuePair<string, IEnumerable<string>>>
            {
                new KeyValuePair<string, IEnumerable<string>>("X-Custom-Header", new List<string> { "blah123blah" }),
                new KeyValuePair<string, IEnumerable<string>>("X-Custom-Header-Two", new List<string> { "456qwerty" })
            };

            var signature = sut.Create(headers, _applicationUri, DateTime.UtcNow.ToTimeStamp(), new Nonce(), _requestPayload, _scope, _apiKey);

            var validator = new SignatureValidator();

            var request = new HttpRequestMessage(HttpMethod.Post, _applicationUri)
            {
                Content = new StringContent(_requestPayload)
            };

            foreach (var header in headers)
            {
                request.Headers.Add(header.Key, header.Value);
            }

            request.Headers.Add("Authorization", signature);

            (await validator.Validate(request, _scope, _apiKey)).ShouldBe(true);
        }
    }
}
