using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Primitives;
using Re.Core.Interfaces;
using System;
using System.Threading.Tasks;

namespace Re.Core
{
    internal sealed class ReCoreFilter : IAsyncResourceFilter
    {
        private readonly ReCoreOptions _opts;
        private readonly IReCaptchaService _verificationService;

        public ReCoreFilter(ReCoreOptions opts, IReCaptchaService verificationService)
        {
            _opts = opts ?? throw new ArgumentNullException(nameof(opts));
            _verificationService = verificationService ?? throw new ArgumentNullException(nameof(verificationService));

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
                    if (!context.ModelState.IsValid)
                    {
                        await next();
                        return;
                    }

                    try
                    {
                        var response = await _verificationService.VerifyTokenAsync(reCaptcha[0]);
                        context.HttpContext.Features.Set(response);

                        if (!response.Success)
                        {
                            context.ModelState.AddModelError(Strings.FORM_KEY, _opts.VerificationFailedMessage);
                        }
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
