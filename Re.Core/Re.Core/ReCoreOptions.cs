namespace Re.Core
{
    public class ReCoreOptions
    {
        public string NotCompletedMessage { get; set; }
        public string SecretKey { get; set; }
        public string VerificationFailedMessage { get; set; }

        public ReCoreOptions()
        {
            NotCompletedMessage = Strings.NOT_COMPLETED_DEFAULT_MESSAGE;
            VerificationFailedMessage = Strings.VERIFICATION_FAILED_DEFAULT_MESSAGE;
        }
    }
}
