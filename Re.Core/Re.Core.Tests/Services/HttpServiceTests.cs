using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using Re.Core.Interfaces;
using Re.Core.Services;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace Re.Core.Tests.Services
{
    public sealed class HttpServiceTests
    {
        [Fact]
        public void CannotInheritFromService()
        {
            Assert.True(typeof(HttpService).IsSealed);
        }

        private HttpService GetService()
        {
            var services = new ServiceCollection();
            services.AddHttpClient();
            var provider = services.BuildServiceProvider();

            return new HttpService(provider.GetRequiredService<IHttpClientFactory>());
        }

        [Fact]
        public void ImplementsIHttpService()
        {
            Assert.IsAssignableFrom<IHttpService>(GetService());
        }

        [Fact]
        public async Task ReturnsResponseFromPost()
        {
            var value = "Hello!";
            var service = GetService();
            var response = await service.PostAsync("https://postman-echo.com/post", new StringContent(value));
            var content = await response.Content.ReadAsStringAsync();
            var jObj = JObject.Parse(content);

            Assert.Equal(value, jObj.Value<string>("data"));
        }

        [Fact]
        public void ThrowsIfHttpClientFactoryIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new HttpService(null));
        }
    }
}
