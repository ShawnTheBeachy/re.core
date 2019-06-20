using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Primitives;
using Re.Core.Interfaces;
using Re.Core.Services;
using System;
using System.Threading.Tasks;

namespace Re.Core
{
    internal sealed class ReCoreFilter : IAsyncResourceFilter
    {
        private readonly ReCoreOptions _opts;
        private readonly ReCaptchaV2Service _v2Service;
        internal IReCaptchaService _verificationService;

        public ReCoreFilter(ReCoreOptions opts, ReCaptchaV2Service v2Service)
        {
            _opts = opts ?? throw new ArgumentNullException(nameof(opts));
            _v2Service = v2Service ?? throw new ArgumentNullException(nameof(v2Service));

            if (string.IsNullOrWhiteSpace(opts.SecretKey))
            {
                throw new Exception(Strings.SECRET_KEY_REQUIRED);
            }
        }

        public async Task OnResourceExecutionAsync(ResourceExecutingContext context, ResourceExecutionDelegate next)
        {
            if (context.HttpContext.Request.Method == "POST" && context.HttpContext.Request.HasFormContentType &&
                (context.HttpContext.Request.Form?.TryGetValue("g-recaptcha-response", out var reCaptcha) ?? false))
            {
                context.HttpContext.Request.Headers.Add(Strings.VERIFIED_HEADER, new StringValues(string.Empty));
                var isReCaptchaValid = reCaptcha.Count > 0 && !string.IsNullOrEmpty(reCaptcha[0]);

                if (!isReCaptchaValid)
                {
                    context.ModelState.AddModelError(Strings.FORM_KEY, _opts.NotCompletedMessage);
                    context.HttpContext.Features.Set(new ReCoreVerificationException(_opts.NotCompletedMessage));
                }

                else
                {
                    if (!context.HttpContext.Request.Form.ContainsKey(Strings.FORM_VERSION_KEY))
                    {
                        throw new Exception(Strings.VERSION_REQUIRED);
                    }

                    if (!Enum.TryParse<Version>(context.HttpContext.Request.Form[Strings.FORM_VERSION_KEY].ToString(), out var version))
                    {
                        throw new Exception(Strings.INVALID_VERSION);
                    }

                    _verificationService = _verificationService ?? (version == Version.v2 ? _v2Service : null);

                    try
                    {
                        await _verificationService.VerifyTokenAsync(reCaptcha[0]);
                    }

                    catch (ReCoreVerificationException e)
                    {
                        context.ModelState.AddModelError(Strings.FORM_KEY, _opts.VerificationFailedMessage);
                        context.HttpContext.Features.Set(e);
                    }

                    catch (Exception e)
                    {
                        context.ModelState.AddModelError(Strings.FORM_KEY, _opts.VerificationFailedMessage);
                        context.HttpContext.Features.Set(new ReCoreVerificationException(_opts.VerificationFailedMessage, e));
                    }
                }
            }

            await next();
        }
    }
}
