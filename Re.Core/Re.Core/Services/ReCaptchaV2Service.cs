using Re.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Re.Core.Services
{
    internal sealed class ReCaptchaV2Service : IReCaptchaService
    {
        private readonly IHttpService _httpService;
        private readonly ReCoreOptions _opts;

        public ReCaptchaV2Service(IHttpService httpService, ReCoreOptions opts)
        {
            _httpService = httpService ?? throw new ArgumentNullException(nameof(httpService));
            _opts = opts ?? throw new ArgumentNullException(nameof(opts));
        }

        public async Task VerifyTokenAsync(string token)
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

            var match = Regex.Match(content, "\"success\":(true|false)", RegexOptions.IgnoreCase);
            var success = bool.Parse(match.Groups.Single().Value);

            if (!success)
            {
                throw new ReCoreVerificationException(_opts.VerificationFailedMessage, new Exception(content));
            }
        }
    }
}
