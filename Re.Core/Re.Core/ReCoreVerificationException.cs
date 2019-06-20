using System;

namespace Re.Core
{
    public sealed class ReCoreVerificationException : Exception
    {
        public ReCoreVerificationException()
        {
            // Empty.
        }

        public ReCoreVerificationException(string message) : base(message)
        {
            // Empty.
        }

        public ReCoreVerificationException(string message, Exception innerException) : base(message, innerException)
        {
            // Empty.
        }
    }
}
