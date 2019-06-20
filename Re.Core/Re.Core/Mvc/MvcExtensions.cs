using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using Re.Core.Services;
using System;

namespace Re.Core
{
    public static class MvcExtensions
    {
        public static IServiceCollection AddReCore(this IServiceCollection services,
                                                    Func<ReCoreOptions, ReCoreOptions> configurationExpression)
        {
            var expr = configurationExpression ?? new Func<ReCoreOptions, ReCoreOptions>(x => x);
            var opts = new ReCoreOptions();
            opts = expr(opts);

            if (string.IsNullOrWhiteSpace(opts.SecretKey))
            {
                throw new Exception(Strings.SECRET_KEY_REQUIRED);
            }

            services.AddSingleton(opts);
            services.AddScoped<ReCaptchaV2Service>();
            return services;
        }

        public static MvcOptions AddReCore(this MvcOptions opts)
        {
            opts.Filters.Add(typeof(ReCoreFilter));
            return opts;
        }
    }
}
