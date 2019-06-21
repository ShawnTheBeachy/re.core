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
<div class=""g-recaptcha"" data-sitekey=""{siteKey}"" data-theme=""{theme.ToString().ToLower()}""></div>
";

        internal static Func<string, string, string> v3Template = (string siteKey, string action) =>
$@"
<script src=""https://www.google.com/recaptcha/api.js?render={siteKey}""></script>
<script>
    grecaptcha.ready(function() {{
        grecaptcha.execute('{siteKey}', {{ action: '{action}' }})
            .then(function(token) {{
                document.getElementById('g-recaptcha-response').value = token;
            }});
    }});
</script>
<input type=""hidden"" id=""g-recaptcha-response"" name=""g-recaptcha-response"" />
";

        public static IHtmlContent reCAPTCHAv2(this IHtmlHelper helper, string siteKey, Theme theme = Theme.Light)
        {
            if (string.IsNullOrWhiteSpace(siteKey))
            {
                throw new ArgumentNullException(nameof(siteKey));
            }

            return helper.Raw(v2Template(siteKey, theme));
        }

        public static IHtmlContent reCAPTCHAv3(this IHtmlHelper helper, string siteKey, string action)
        {
            if (string.IsNullOrWhiteSpace(action))
            {
                throw new ArgumentNullException(nameof(action));
            }

            if (string.IsNullOrWhiteSpace(siteKey))
            {
                throw new ArgumentNullException(nameof(siteKey));
            }

            return helper.Raw(v3Template(siteKey, action));
        }
    }
}
