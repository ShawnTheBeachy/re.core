using System;
using System.Net.Http;
using System.Threading.Tasks;
using Re.Core.Interfaces;

namespace Re.Core.Services
{
    internal sealed class HttpService : IHttpService
    {
        private readonly IHttpClientFactory _httpClientFactory;

        public HttpService(IHttpClientFactory httpClientFactory)
        {
            _httpClientFactory = httpClientFactory ?? throw new ArgumentNullException(nameof(httpClientFactory));
        }

        public async Task<HttpResponseMessage> PostAsync(string requestUri, HttpContent content)
        {
            using (var client = _httpClientFactory.CreateClient())
            {
                var response = await client.PostAsync(requestUri, content);
                return response;
            }
        }
    }
}
