using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("Re.Core.Tests")]
namespace Re.Core
{
    internal static class Strings
    {
        internal const string FORM_KEY = "reCAPTCHA";
        internal const string FORM_VERSION_KEY = "reCAPTCHA-Version";
        internal const string INVALID_VERSION = "Invalid reCAPTCHA version provided in form data.";
        internal const string NOT_COMPLETED_DEFAULT_MESSAGE = "You must complete the reCAPTCHA challenge.";
        internal const string SECRET_KEY_REQUIRED = "Secret key must be set in the Re.Core options.";
        internal const string VERIFICATION_FAILED_DEFAULT_MESSAGE = "reCAPTCHA verification failed.";
        internal const string VERIFIED_HEADER = "Verified-reCAPTCHA";
        internal const string VERSION_REQUIRED = "reCAPTCHA version is required in form data.";
    }
}
