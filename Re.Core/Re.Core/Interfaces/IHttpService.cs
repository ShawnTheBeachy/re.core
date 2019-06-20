using System.Net.Http;
using System.Threading.Tasks;

namespace Re.Core.Interfaces
{
    internal interface IHttpService
    {
        Task<HttpResponseMessage> PostAsync(string requestUri, HttpContent content);
    }
}
