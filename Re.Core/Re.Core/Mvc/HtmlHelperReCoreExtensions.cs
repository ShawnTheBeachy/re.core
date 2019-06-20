using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;

namespace Re.Core
{
    public static class HtmlHelperReCoreExtensions
    {
        internal static Func<string, Theme, string> v2Template => (string siteKey, Theme theme) =>
$@"
<script src=""https://www.google.com/recaptcha/api.js"" async defer></script>
<input type=""hidden"" name=""{Strings.FORM_VERSION_KEY}"" value=""v2"" />
<div class=""g-recaptcha"" data-sitekey=""{siteKey}"" data-theme=""{theme.ToString().ToLower()}""></div>
";

        public static IHtmlContent reCAPTCHA(this IHtmlHelper helper,
                                             string siteKey,
                                             Version version = Version.v2,
                                             Theme theme = Theme.Light)
        {
            if (string.IsNullOrWhiteSpace(siteKey))
            {
                throw new ArgumentNullException(nameof(siteKey));
            }

            return version == Version.v2 ? helper.reCAPTCHAv2(siteKey, theme) : null;
        }

        private static IHtmlContent reCAPTCHAv2(this IHtmlHelper helper,
                                                string siteKey,
                                                Theme theme)
        {
            return helper.Raw(v2Template(siteKey, theme));
        }
    }
}
