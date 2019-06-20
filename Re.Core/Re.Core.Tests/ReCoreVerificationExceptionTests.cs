using System;
using Xunit;

namespace Re.Core.Tests
{
    public sealed class ReCoreVerificationExceptionTests
    {
        [Fact]
        public void CanNotInheritFromCustomException()
        {
            Assert.True(typeof(ReCoreVerificationException).IsSealed);
        }

        [Fact]
        public void ConstructorsCallBase()
        {
            var exception = new ReCoreVerificationException();
            Assert.Equal("Exception of type 'Re.Core.ReCoreVerificationException' was thrown.", exception.Message);

            exception = new ReCoreVerificationException("Test");
            Assert.Equal("Test", exception.Message);

            var inner = new Exception();
            exception = new ReCoreVerificationException("Test", inner);
            Assert.Equal("Test", exception.Message);
            Assert.Equal(inner, exception.InnerException);
        }

        [Fact]
        public void InheritsFromException()
        {
            Assert.IsAssignableFrom<Exception>(new ReCoreVerificationException());
        }
    }
}
