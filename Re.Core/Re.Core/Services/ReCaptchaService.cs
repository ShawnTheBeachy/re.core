using Newtonsoft.Json;
using Re.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Re.Core.Services
{
    internal sealed class ReCaptchaService : IReCaptchaService
    {
        private readonly IHttpService _httpService;
        private readonly ReCoreOptions _opts;

        public ReCaptchaService(IHttpService httpService, ReCoreOptions opts)
        {
            _httpService = httpService ?? throw new ArgumentNullException(nameof(httpService));
            _opts = opts ?? throw new ArgumentNullException(nameof(opts));
        }

        public async Task<VerificationResponse> VerifyTokenAsync(string token)
        {
            var body = new Dictionary<string, string>
            {
                ["secret"] = _opts.SecretKey,
                ["response"] = token
            };
            var response = await _httpService.PostAsync("https://www.google.com/recaptcha/api/siteverify",
                new FormUrlEncodedContent(body));
            var content = await response.Content.ReadAsStringAsync();

            if (!response.IsSuccessStatusCode)
            {
                throw new ReCoreVerificationException(_opts.VerificationFailedMessage, new Exception(content));
            }

            try
            {
                var verificationResponse = JsonConvert.DeserializeObject<VerificationResponse>(content);
                return verificationResponse;
            }

            catch (Exception e)
            {
                throw new ReCoreVerificationException(_opts.VerificationFailedMessage, e);
            }
        }
    }
}
