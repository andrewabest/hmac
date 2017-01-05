using System;

namespace Hmac
{
    public class Nonce
    {
        private readonly string _value;

        public Nonce()
        {
            _value = Guid.NewGuid().ToString("N");
        }

        public Nonce(string value)
        {
            _value = value;
        }

        public override string ToString()
        {
            return _value;
        }
    }
}