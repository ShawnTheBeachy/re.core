using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Re.Core.Interfaces;

namespace Re.Core.Tests
{
    public sealed class MockHttpService : IHttpService
    {
        private readonly bool _shouldNetworkRequestSucceed;
        private readonly bool _shouldVerifySucceed;

        public MockHttpService(bool shouldNetworkRequestSucceed = true, bool shouldVerifySucceed = true)
        {
            _shouldNetworkRequestSucceed = shouldNetworkRequestSucceed;
            _shouldVerifySucceed = shouldVerifySucceed;
        }

        public async Task<HttpResponseMessage> PostAsync(string requestUri, HttpContent content)
        {
            await Task.CompletedTask;
            var message = new HttpResponseMessage(_shouldNetworkRequestSucceed ? HttpStatusCode.OK : HttpStatusCode.BadRequest);

            var body = new
            {
                success = _shouldVerifySucceed
            };

            message.Content = new StringContent(JsonConvert.SerializeObject(body));
            return message;
        }
    }
}
