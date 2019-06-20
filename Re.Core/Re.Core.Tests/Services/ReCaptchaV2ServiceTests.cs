using Re.Core.Interfaces;
using Re.Core.Services;
using System;
using System.Threading.Tasks;
using Xunit;

namespace Re.Core.Tests.Services
{
    public sealed class ReCaptchaV2ServiceTests
    {
        private ReCaptchaV2Service GetService(bool shouldNetworkRequestSucceed = true, bool shouldVerifySucceed = true)
        {
            return new ReCaptchaV2Service(new MockHttpService(shouldNetworkRequestSucceed, shouldVerifySucceed), new ReCoreOptions());
        }

        [Fact]
        public async Task AddsContentAsInnerExceptionIfNetworkRequestFails()
        {
            var service = GetService(false);
            var exception = await Assert.ThrowsAsync<ReCoreVerificationException>(async () => await service.VerifyTokenAsync("abcde"));
            Assert.Equal("{\"success\":true}", exception.InnerException.Message);
        }

        [Fact]
        public async Task AddsContentAsInnerExceptionIfVerificationFails()
        {
            var service = GetService(true, false);
            var exception = await Assert.ThrowsAsync<ReCoreVerificationException>(async () => await service.VerifyTokenAsync("abcde"));
            Assert.Equal("{\"success\":false}", exception.InnerException.Message);
        }

        [Fact]
        public void CannotInheritFromService()
        {
            Assert.True(typeof(ReCaptchaV2Service).IsSealed);
        }

        [Fact]
        public async Task FinishesIfVerificationSucceeds()
        {
            var service = GetService();
            await service.VerifyTokenAsync("abcde");
        }

        [Fact]
        public void ImplementsIRecaptchaService()
        {
            Assert.IsAssignableFrom<IReCaptchaService>(GetService());
        }

        [Fact]
        public void ThrowsIfHttpClientFactoryIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new ReCaptchaV2Service(null, new ReCoreOptions()));
        }

        [Fact]
        public async Task ThrowsIfNetworkRequestFails()
        {
            var service = GetService(false);
            var exception = await Assert.ThrowsAsync<ReCoreVerificationException>(async () => await service.VerifyTokenAsync("abcde"));
            Assert.Equal(Strings.VERIFICATION_FAILED_DEFAULT_MESSAGE, exception.Message);
        }

        [Fact]
        public async Task ThrowsIfVerificationFails()
        {
            var service = GetService(true, false);
            var exception = await Assert.ThrowsAsync<ReCoreVerificationException>(async () => await service.VerifyTokenAsync("abcde"));
            Assert.Equal(Strings.VERIFICATION_FAILED_DEFAULT_MESSAGE, exception.Message);
        }

        [Fact]
        public void ThrowsIfOptionsIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new ReCaptchaV2Service(new MockHttpService(), null));
        }
    }
}
