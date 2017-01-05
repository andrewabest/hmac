using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;

namespace Hmac
{
    public class SignatureCreator
    {
        public string Create(
            IEnumerable<KeyValuePair<string, IEnumerable<string>>> headers, 
            Uri uri, 
            string requestTimeStamp,
            Nonce nonce,
            string payload, 
            string scope, 
            string apiKey)
        {
            var serializedValue = JsonConvert.SerializeObject(payload);
            var content = Encoding.UTF8.GetBytes(serializedValue);
            var sha256 = SHA256.Create();
            var requestContentHash = sha256.ComputeHash(content);
            var requestContentHashBase64String = Convert.ToBase64String(requestContentHash);

            var canonicalUri = Uri.EscapeUriString(uri.AbsolutePath);
            var canonicalQueryString = Uri.EscapeUriString(uri.Query);
            var canonicalHeaders =
                headers
                    .OrderBy(x => x.Key)
                    .Select(
                        x =>
                            x.Key.ToLowerInvariant() + ':' +
                            x.Value.Aggregate((acc, str) => acc + "," + str))
                    .Aggregate((acc, str) => acc + Environment.NewLine + str);

            var signedHeaders =
                headers
                    .OrderBy(x => x.Key)
                    .Select(x => x.Key)
                    .Aggregate((acc, str) => acc + ";" + str);

            var canonicalRequest =
                "POST" + Environment.NewLine +
                canonicalUri + Environment.NewLine +
                canonicalQueryString + Environment.NewLine +
                canonicalHeaders + Environment.NewLine +
                signedHeaders + Environment.NewLine + 
                requestContentHashBase64String;

            var canonicalRequestContent = Encoding.UTF8.GetBytes(canonicalRequest);
            var canonicalRequestContentHash = sha256.ComputeHash(canonicalRequestContent);
            var canonicalRequestContentHashBase64String = Convert.ToBase64String(canonicalRequestContentHash);

            var stringToSign =
                scope + Environment.NewLine +
                nonce + Environment.NewLine + 
                requestTimeStamp + Environment.NewLine + 
                canonicalRequestContentHashBase64String;

            var secretKeyBytes = Encoding.UTF8.GetBytes(apiKey);
            var stringToSignBytes = Encoding.UTF8.GetBytes(stringToSign);

            using (var hmac = new HMACSHA256(secretKeyBytes))
            {
                var signatureBytes = hmac.ComputeHash(stringToSignBytes);
                var requestSignature = Convert.ToBase64String(signatureBytes);
                var header = new AuthenticationHeaderValue(SignatureProperties.AuthenticationScheme, $"{scope}:{nonce}:{requestTimeStamp}:{signedHeaders}:{requestSignature}");

                return header.ToString();
            }
        }
    }
}