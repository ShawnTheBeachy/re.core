using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Re.Core.Interfaces;
using Re.Core.Services;
using Xunit;

namespace Re.Core.Tests
{
    public sealed class MvcExtensionsTests
    {
        [Fact]
        public void AddsCustomFilter()
        {
            var opts = new MvcOptions();
            opts.AddReCore();

            Assert.Single(opts.Filters);
            Assert.Equal(typeof(ReCoreFilter), (opts.Filters[0] as TypeFilterAttribute).ImplementationType);
        }

        [Fact]
        public void DoesNotImplodeIfConfigExpressionIsNotSet()
        {
            var services = new ServiceCollection();
            services.AddReCore(null);
        }

        [Fact]
        public void RegistersServices()
        {
            var services = new ServiceCollection();
            services.AddReCore(x =>
            {
                x.SecretKey = "12345";
                return x;
            });

            Assert.Equal(3, services.Count);
            Assert.Equal(ServiceLifetime.Singleton, services[0].Lifetime);
            Assert.Equal(ServiceLifetime.Scoped, services[1].Lifetime);
            Assert.Equal(ServiceLifetime.Scoped, services[2].Lifetime);

            services.AddHttpClient();

            var provider = services.BuildServiceProvider();
            provider.GetRequiredService<ReCoreOptions>();
            provider.GetRequiredService<ReCaptchaV2Service>();
            Assert.IsType<HttpService>(provider.GetRequiredService<IHttpService>());
        }
    }
}
