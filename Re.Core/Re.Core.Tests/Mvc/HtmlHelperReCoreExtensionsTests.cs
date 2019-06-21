using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Xunit;

namespace Re.Core.Tests.Mvc
{
    public sealed class HtmlHelperReCoreExtensionsTests
    {
        private class MockContent : IHtmlContent
        {
            private string _content;

            public MockContent(string content)
            {
                _content = content ?? throw new ArgumentNullException(nameof(content));
            }

            public override string ToString()
            {
                return _content;
            }

            public void WriteTo(TextWriter writer, HtmlEncoder encoder)
            {

            }
        }

        private class MockHelper : IHtmlHelper
        {
            public Html5DateRenderingMode Html5DateRenderingMode { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

            public string IdAttributeDotReplacement => throw new NotImplementedException();

            public IModelMetadataProvider MetadataProvider => throw new NotImplementedException();

            public dynamic ViewBag => throw new NotImplementedException();

            public ViewContext ViewContext => throw new NotImplementedException();

            public ViewDataDictionary ViewData => throw new NotImplementedException();

            public ITempDataDictionary TempData => throw new NotImplementedException();

            public UrlEncoder UrlEncoder => throw new NotImplementedException();

            public IHtmlContent ActionLink(string linkText, string actionName, string controllerName, string protocol, string hostname, string fragment, object routeValues, object htmlAttributes)
            {
                throw new NotImplementedException();
            }

            public IHtmlContent AntiForgeryToken()
            {
                throw new NotImplementedException();
            }

            public MvcForm BeginForm(string actionName, string controllerName, object routeValues, FormMethod method, bool? antiforgery, object htmlAttributes)
            {
                throw new NotImplementedException();
            }

            public MvcForm BeginRouteForm(string routeName, object routeValues, FormMethod method, bool? antiforgery, object htmlAttributes)
            {
                throw new NotImplementedException();
            }

            public IHtmlContent CheckBox(string expression, bool? isChecked, object htmlAttributes)
            {
                throw new NotImplementedException();
            }

            public IHtmlContent Display(string expression, string templateName, string htmlFieldName, object additionalViewData)
            {
                throw new NotImplementedException();
            }

            public string DisplayName(string expression)
            {
                throw new NotImplementedException();
            }

            public string DisplayText(string expression)
            {
                throw new NotImplementedException();
            }

            public IHtmlContent DropDownList(string expression, IEnumerable<SelectListItem> selectList, string optionLabel, object htmlAttributes)
            {
                throw new NotImplementedException();
            }

            public IHtmlContent Editor(string expression, string templateName, string htmlFieldName, object additionalViewData)
            {
                throw new NotImplementedException();
            }

            public string Encode(object value)
            {
                throw new NotImplementedException();
            }

            public string Encode(string value)
            {
                throw new NotImplementedException();
            }

            public void EndForm()
            {
                throw new NotImplementedException();
            }

            public string FormatValue(object value, string format)
            {
                throw new NotImplementedException();
            }

            public string GenerateIdFromName(string fullName)
            {
                throw new NotImplementedException();
            }

            public IEnumerable<SelectListItem> GetEnumSelectList<TEnum>() where TEnum : struct
            {
                throw new NotImplementedException();
            }

            public IEnumerable<SelectListItem> GetEnumSelectList(Type enumType)
            {
                throw new NotImplementedException();
            }

            public IHtmlContent Hidden(string expression, object value, object htmlAttributes)
            {
                throw new NotImplementedException();
            }

            public string Id(string expression)
            {
                throw new NotImplementedException();
            }

            public IHtmlContent Label(string expression, string labelText, object htmlAttributes)
            {
                throw new NotImplementedException();
            }

            public IHtmlContent ListBox(string expression, IEnumerable<SelectListItem> selectList, object htmlAttributes)
            {
                throw new NotImplementedException();
            }

            public string Name(string expression)
            {
                throw new NotImplementedException();
            }

            public Task<IHtmlContent> PartialAsync(string partialViewName, object model, ViewDataDictionary viewData)
            {
                throw new NotImplementedException();
            }

            public IHtmlContent Password(string expression, object value, object htmlAttributes)
            {
                throw new NotImplementedException();
            }

            public IHtmlContent RadioButton(string expression, object value, bool? isChecked, object htmlAttributes)
            {
                throw new NotImplementedException();
            }

            public IHtmlContent Raw(string value)
            {
                return new MockContent(value);
            }

            public IHtmlContent Raw(object value)
            {
                throw new NotImplementedException();
            }

            public Task RenderPartialAsync(string partialViewName, object model, ViewDataDictionary viewData)
            {
                throw new NotImplementedException();
            }

            public IHtmlContent RouteLink(string linkText, string routeName, string protocol, string hostName, string fragment, object routeValues, object htmlAttributes)
            {
                throw new NotImplementedException();
            }

            public IHtmlContent TextArea(string expression, string value, int rows, int columns, object htmlAttributes)
            {
                throw new NotImplementedException();
            }

            public IHtmlContent TextBox(string expression, object value, string format, object htmlAttributes)
            {
                throw new NotImplementedException();
            }

            public IHtmlContent ValidationMessage(string expression, string message, object htmlAttributes, string tag)
            {
                throw new NotImplementedException();
            }

            public IHtmlContent ValidationSummary(bool excludePropertyErrors, string message, object htmlAttributes, string tag)
            {
                throw new NotImplementedException();
            }

            public string Value(string expression, string format)
            {
                throw new NotImplementedException();
            }
        }

        [Fact]
        public void ReturnsV2IfSpecified()
        {
            Assert.Equal(HtmlHelperReCoreExtensions.v2Template("abcde", Theme.Light), new MockHelper().reCAPTCHAv2("abcde").ToString());
        }

        [Fact]
        public void ReturnsV3IfSpecified()
        {
            Assert.Equal(HtmlHelperReCoreExtensions.v3Template("abcde", "action"), new MockHelper().reCAPTCHAv3("abcde", "action").ToString());
        }

        [Theory]
        [InlineData(Theme.Dark)]
        [InlineData(Theme.Light)]
        public void SetsThemeProperly(Theme theme)
        {
            Assert.Equal(HtmlHelperReCoreExtensions.v2Template("abcde", theme), new MockHelper().reCAPTCHAv2("abcde", theme).ToString());
        }

        [Fact]
        public void v2ThrowsIfSiteKeyIsEmpty()
        {
            Assert.Throws<ArgumentNullException>(() => new MockHelper().reCAPTCHAv2(null));
        }

        [Fact]
        public void v3ThrowsIfActionIsEmpty()
        {
            Assert.Throws<ArgumentNullException>(() => new MockHelper().reCAPTCHAv3("abcde", null));
            Assert.Throws<ArgumentNullException>(() => new MockHelper().reCAPTCHAv3("abcde", string.Empty));
            Assert.Throws<ArgumentNullException>(() => new MockHelper().reCAPTCHAv3("abcde", "  "));
        }

        [Fact]
        public void v3ThrowsIfSiteKeyIsEmpty()
        {
            Assert.Throws<ArgumentNullException>(() => new MockHelper().reCAPTCHAv3(null, "abcde"));
            Assert.Throws<ArgumentNullException>(() => new MockHelper().reCAPTCHAv3(string.Empty, "abcde"));
            Assert.Throws<ArgumentNullException>(() => new MockHelper().reCAPTCHAv3("  ", "abcde"));
        }
    }
}
