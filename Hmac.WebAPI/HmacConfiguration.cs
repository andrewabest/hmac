namespace Hmac.WebAPI
{
    public class HmacConfiguration
    {
        public string Scope { get; private set; }
        public string ApiKey { get; private set; }

        public static HmacConfiguration Create()
        {
            return new HmacConfiguration();
        }

        public HmacConfiguration WithScope(string scope)
        {
            Scope = scope;
            return this;
        }

        public HmacConfiguration WithApiKey(string apiKey)
        {
            ApiKey = apiKey;
            return this;
        }

        public void Configure()
        {
            Hmac.Configuration = this;
        }
    }
}