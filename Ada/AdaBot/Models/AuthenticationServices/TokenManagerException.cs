using System;

namespace AdaW10.Models.AuthenticationServices
{
    public class TokenManagerException : Exception 
    {
        public TokenManagerException()
        {
        }

        public TokenManagerException(string message)
        : base(message)
        {
        }

        public TokenManagerException(string message, Exception inner)
        : base(message, inner)
        {
        }
    }
}
