using Newtonsoft.Json;
using Re.Core.Interfaces;
using Re.Core.Services;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Re.Core.Tests.Services
{
    public sealed class ReCaptchaServiceTests
    {
        [Fact]
        public async Task AddsContentAsInnerExceptionIfNetworkRequestFails()
        {
            var service = GetService(false);
            var exception = await Assert.ThrowsAsync<ReCoreVerificationException>(async () => await service.VerifyTokenAsync("abcde"));
            Assert.Equal("{\"success\":true}", exception.InnerException.Message);
        }

        [Fact]
        public async Task AddsJsonExceptionAsInnerExceptionIfVerificationThrowsException()
        {
            var service = GetService(true, true, true);
            var exception = await Assert.ThrowsAsync<ReCoreVerificationException>(async () => await service.VerifyTokenAsync("abcde"));

            Assert.Equal(Strings.VERIFICATION_FAILED_DEFAULT_MESSAGE, exception.Message);
            Assert.IsType<JsonReaderException>(exception.InnerException);
        }

        [Fact]
        public void CannotInheritFromService()
        {
            Assert.True(typeof(ReCaptchaService).IsSealed);
        }

        [Fact]
        public async Task FinishesIfVerificationSucceeds()
        {
            var service = GetService();
            await service.VerifyTokenAsync("abcde");
        }

        private ReCaptchaService GetService(bool shouldNetworkRequestSucceed = true, bool shouldVerifySucceed = true,
                                            bool shouldReturnInvalidJson = false)
        {
            return new ReCaptchaService(
                new MockHttpService(shouldNetworkRequestSucceed, shouldVerifySucceed, shouldReturnInvalidJson),
                new ReCoreOptions());
        }

        [Fact]
        public void ImplementsIRecaptchaService()
        {
            Assert.IsAssignableFrom<IReCaptchaService>(GetService());
        }

        [Fact]
        public async Task ReturnsResponseIfVerificationFails()
        {
            var service = GetService(true, false);
            var response = await service.VerifyTokenAsync("abcde");

            Assert.False(response.Success);
        }

        [Fact]
        public async Task ReturnsResponseIfVerificationSucceeds()
        {
            var service = GetService();
            var response = await service.VerifyTokenAsync("abcde");

            Assert.True(response.Success);
        }

        [Fact]
        public void ThrowsIfHttpClientFactoryIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new ReCaptchaService(null, new ReCoreOptions()));
        }

        [Fact]
        public async Task ThrowsIfNetworkRequestFails()
        {
            var service = GetService(false);
            var exception = await Assert.ThrowsAsync<ReCoreVerificationException>(async () => await service.VerifyTokenAsync("abcde"));
            Assert.Equal(Strings.VERIFICATION_FAILED_DEFAULT_MESSAGE, exception.Message);
        }

        [Fact]
        public void ThrowsIfOptionsIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new ReCaptchaService(new MockHttpService(), null));
        }
    }
}
