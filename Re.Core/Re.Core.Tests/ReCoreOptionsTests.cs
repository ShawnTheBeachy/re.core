using Xunit;

namespace Re.Core.Tests
{
    public sealed class ReCoreOptionsTests
    {
        [Fact]
        public void CanAssignNotCompletedMessage()
        {
            var value = "Not completed.";
            var model = new ReCoreOptions
            {
                NotCompletedMessage = value
            };
            Assert.Equal(value, model.NotCompletedMessage);
        }

        [Fact]
        public void CanAssignSecretKey()
        {
            var value = "12345";
            var model = new ReCoreOptions
            {
                SecretKey = value
            };
            Assert.Equal(value, model.SecretKey);
        }

        [Fact]
        public void CanAssignVerificationFailedMessage()
        {
            var value = "Verification failed.";
            var model = new ReCoreOptions
            {
                VerificationFailedMessage = value
            };
            Assert.Equal(value, model.VerificationFailedMessage);
        }

        [Fact]
        public void CanInstantiateConcreteInstance()
        {
            Assert.False(typeof(ReCoreOptions).IsAbstract);
        }

        [Fact]
        public void CanInheritFromOptions()
        {
            Assert.False(typeof(ReCoreOptions).IsSealed);
        }

        [Fact]
        public void HasDefaultNotCompletedMessage()
        {
            var model = new ReCoreOptions();
            Assert.Equal(Strings.NOT_COMPLETED_DEFAULT_MESSAGE, model.NotCompletedMessage);
        }

        [Fact]
        public void HasDefaultVerificationFailedMessage()
        {
            var model = new ReCoreOptions();
            Assert.Equal(Strings.VERIFICATION_FAILED_DEFAULT_MESSAGE, model.VerificationFailedMessage);
        }
    }
}
