using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Primitives;
using Re.Core.Interfaces;
using Re.Core.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Re.Core.Tests
{
    public sealed class ReCoreFilterTests
    {
        private class MockHttpContext : DefaultHttpContext
        {
            private readonly bool _hasFormContentType;

            public MockHttpContext(bool hasFormContentType)
            {
                _hasFormContentType = hasFormContentType;
            }

            public override HttpRequest Request => new MockHttpRequest(this, _hasFormContentType);
        }

        private class MockHttpRequest : DefaultHttpRequest
        {
            private bool _hasFormContentType;

            public MockHttpRequest(HttpContext context, bool hasFormContentType) : base(context)
            {
                _hasFormContentType = hasFormContentType;
            }

            public override bool HasFormContentType => _hasFormContentType;
        }

        private class MockInvalidGenericExceptionService : IReCaptchaService
        {
            public Task VerifyTokenAsync(string token)
            {
                throw new Exception(Strings.VERIFICATION_FAILED_DEFAULT_MESSAGE);
            }
        }

        private class MockInvalidService : IReCaptchaService
        {
            public Task VerifyTokenAsync(string token)
            {
                throw new ReCoreVerificationException(Strings.VERIFICATION_FAILED_DEFAULT_MESSAGE);
            }
        }

        private class MockValidService : IReCaptchaService
        {
            public Task VerifyTokenAsync(string token)
            {
                return Task.CompletedTask;
            }
        }

        [Fact]
        public async Task AddsExceptionToHttpContextIfFormValueIsInvalid()
        {
            var (context, next) = GetPostingContexts(string.Empty);
            var filter = GetFilter();
            await filter.OnResourceExecutionAsync(context, next);

            var exception = context.HttpContext.Features.Get<ReCoreVerificationException>();
            Assert.NotNull(exception);
            Assert.Equal(Strings.NOT_COMPLETED_DEFAULT_MESSAGE, exception.Message);
        }

        [Fact]
        public async Task AddsExceptionToHttpContextIfVerificationFails()
        {
            var (context, next) = GetPostingContexts(null);
            context.HttpContext.Request.Form = new FormCollection(new Dictionary<string, StringValues>
            {
                ["g-recaptcha-response"] = "abcde",
                [Strings.FORM_VERSION_KEY] = "v2"
            });

            var filter = GetFilter();
            filter._verificationService = new MockInvalidService();
            await filter.OnResourceExecutionAsync(context, next);

            var exception = context.HttpContext.Features.Get<ReCoreVerificationException>();
            Assert.NotNull(exception);
            Assert.Equal(Strings.VERIFICATION_FAILED_DEFAULT_MESSAGE, exception.Message);
        }

        [Fact]
        public async Task AddsModelErrorIfFormValueIsInvalid()
        {
            var (context, next) = GetPostingContexts(string.Empty);

            var filter = GetFilter();
            await filter.OnResourceExecutionAsync(context, next);

            Assert.False(context.ModelState.IsValid);
            Assert.Equal(ModelValidationState.Invalid, context.ModelState.GetFieldValidationState(Strings.FORM_KEY));
            Assert.Equal(Strings.NOT_COMPLETED_DEFAULT_MESSAGE, context.ModelState[Strings.FORM_KEY].Errors.Single().ErrorMessage);
        }

        [Fact]
        public async Task AddsModelErrorIfVerificationFails()
        {
            var (context, next) = GetPostingContexts(null);
            context.HttpContext.Request.Form = new FormCollection(new Dictionary<string, StringValues>
            {
                ["g-recaptcha-response"] = "abcde",
                [Strings.FORM_VERSION_KEY] = "v2"
            });

            var filter = GetFilter();
            filter._verificationService = new MockInvalidService();
            await filter.OnResourceExecutionAsync(context, next);

            Assert.False(context.ModelState.IsValid);
            Assert.Equal(ModelValidationState.Invalid, context.ModelState.GetFieldValidationState(Strings.FORM_KEY));
            Assert.Equal(Strings.VERIFICATION_FAILED_DEFAULT_MESSAGE, context.ModelState[Strings.FORM_KEY].Errors.Single().ErrorMessage);
        }

        [Fact]
        public async Task CallsNextWhenDoesNotVerify()
        {
            var calledNext = false;

            var actionContext = new ActionContext(new DefaultHttpContext(), new RouteData(), new ActionDescriptor());
            var filters = new List<IFilterMetadata>();
            var values = new List<IValueProviderFactory>();
            var nextContext = new ResourceExecutedContext(actionContext, filters);

            var context = new ResourceExecutingContext(actionContext, filters, values);
            var next = new ResourceExecutionDelegate(() =>
            {
                calledNext = true;
                return Task.FromResult(nextContext);
            });

            var filter = GetFilter();
            await filter.OnResourceExecutionAsync(context, next);
            Assert.True(calledNext);
        }

        [Fact]
        public async Task CallsNextWhenVerificationSucceeds()
        {
            var calledNext = false;

            var actionContext = new ActionContext(new DefaultHttpContext(), new RouteData(), new ActionDescriptor());
            var filters = new List<IFilterMetadata>();
            var values = new List<IValueProviderFactory>();
            var nextContext = new ResourceExecutedContext(actionContext, filters);

            var context = new ResourceExecutingContext(actionContext, filters, values);
            var next = new ResourceExecutionDelegate(() =>
            {
                calledNext = true;
                return Task.FromResult(nextContext);
            });

            context.HttpContext.Request.Method = "POST";
            context.HttpContext.Request.ContentType = "application/x-www-form-urlencoded";
            context.HttpContext.Request.Form = new FormCollection(new Dictionary<string, StringValues>
            {
                ["g-recaptcha-response"] = "abcde",
                [Strings.FORM_VERSION_KEY] = "v2"
            });

            var filter = GetFilter();
            filter._verificationService = new MockValidService();
            await filter.OnResourceExecutionAsync(context, next);

            Assert.True(context.HttpContext.Request.Headers.ContainsKey(Strings.VERIFIED_HEADER));
            Assert.True(calledNext);
        }

        [Fact]
        public async Task DoesNotVerifyIfFormValueIsAbsent()
        {
            var (context, next) = GetContexts();
            context.HttpContext.Request.Method = "POST";
            context.HttpContext.Request.ContentType = "application/x-www-form-urlencoded";

            var filter = GetFilter();
            await filter.OnResourceExecutionAsync(context, next);

            Assert.False(context.HttpContext.Request.Headers.ContainsKey(Strings.VERIFIED_HEADER));
        }

        [Fact]
        public async Task DoesNotVerifyIfNotPostRequest()
        {
            var (context, next) = GetContexts();
            context.HttpContext.Request.Method = "GET";
            context.HttpContext.Request.ContentType = "application/x-www-form-urlencoded";
            context.HttpContext.Request.Form = new FormCollection(new Dictionary<string, StringValues>
            {
                ["g-recaptcha-response"] = "abcde"
            });

            var filter = GetFilter();
            await filter.OnResourceExecutionAsync(context, next);

            Assert.False(context.HttpContext.Request.Headers.ContainsKey(Strings.VERIFIED_HEADER));
        }

        [Fact]
        public async Task DoesNotVerifyIfNotFormContentType()
        {
            var (context, next) = GetContexts();
            context.HttpContext = new MockHttpContext(false);
            context.HttpContext.Request.Method = "POST";
            context.HttpContext.Request.Form = new FormCollection(new Dictionary<string, StringValues>
            {
                ["g-recaptcha-response"] = "abcde"
            });

            var filter = GetFilter();
            await filter.OnResourceExecutionAsync(context, next);

            Assert.False(context.HttpContext.Request.Headers.ContainsKey(Strings.VERIFIED_HEADER));
        }

        [Fact]
        public async Task DoesVerifyIfIsFormContentTypeAndHasFormValueAndIsPostRequest()
        {
            var (context, next) = GetPostingContexts(string.Empty);

            var filter = GetFilter();
            await filter.OnResourceExecutionAsync(context, next);

            Assert.True(context.HttpContext.Request.Headers.ContainsKey(Strings.VERIFIED_HEADER));
        }

        private (ResourceExecutingContext Context, ResourceExecutionDelegate Next) GetContexts()
        {
            var actionContext = new ActionContext(new DefaultHttpContext(), new RouteData(), new ActionDescriptor());
            var filters = new List<IFilterMetadata>();
            var values = new List<IValueProviderFactory>();
            var nextContext = new ResourceExecutedContext(actionContext, filters);

            var context = new ResourceExecutingContext(actionContext, filters, values);
            var next = new ResourceExecutionDelegate(() => Task.FromResult(nextContext));

            return (context, next);
        }

        private (ResourceExecutingContext Context, ResourceExecutionDelegate Next) GetPostingContexts(string reCaptchaValue)
        {
            var (context, next) = GetContexts();
            context.HttpContext.Request.Method = "POST";
            context.HttpContext.Request.ContentType = "application/x-www-form-urlencoded";
            context.HttpContext.Request.Form = new FormCollection(new Dictionary<string, StringValues>
            {
                ["g-recaptcha-response"] = reCaptchaValue
            });
            return (context, next);
        }

        private ReCoreFilter GetFilter()
        {
            var v2Service = new ReCaptchaV2Service(new MockHttpService(), new ReCoreOptions());
            var filter = new ReCoreFilter(new ReCoreOptions { SecretKey = "abcde" }, v2Service);
            return filter;
        }

        [Fact]
        public void ImplementsIAsyncResourceFilter()
        {
            Assert.IsAssignableFrom<IAsyncResourceFilter>(GetFilter());
        }

        [Fact]
        public void ThrowsIfOptsAreNull()
        {
            var services = new ServiceCollection();
            services.AddHttpClient();
            var provider = services.BuildServiceProvider();

            var service = new ReCaptchaV2Service(new MockHttpService(), new ReCoreOptions());
            Assert.Throws<ArgumentNullException>(() => new ReCoreFilter(null, service));
        }

        [Fact]
        public void ThrowsIfSecretKeyIsNotSet()
        {
            var v2Service = new ReCaptchaV2Service(new MockHttpService(), new ReCoreOptions());
            var exception = Assert.Throws<Exception>(() => new ReCoreFilter(new ReCoreOptions(), v2Service));
            Assert.Equal(Strings.SECRET_KEY_REQUIRED, exception.Message);
        }

        [Fact]
        public void ThrowsIfV2ServiceIsNull()
        {
            Assert.Throws<ArgumentNullException>(() => new ReCoreFilter(new ReCoreOptions(), null));
        }

        [Fact]
        public async Task ThrowsIfVersionIsInvalid()
        {
            var (context, next) = GetPostingContexts(null);
            context.HttpContext.Request.Form = new FormCollection(new Dictionary<string, StringValues>
            {
                ["g-recaptcha-response"] = "abcde",
                [Strings.FORM_VERSION_KEY] = "v4"
            });

            var filter = GetFilter();
            var exception = await Assert.ThrowsAsync<Exception>(async () => await filter.OnResourceExecutionAsync(context, next));
            Assert.Equal(Strings.INVALID_VERSION, exception.Message);
        }

        [Fact]
        public async Task ThrowsIfVersionKeyIsMissing()
        {
            var (context, next) = GetPostingContexts("abcde");

            var filter = GetFilter();
            var exception = await Assert.ThrowsAsync<Exception>(async () => await filter.OnResourceExecutionAsync(context, next));
            Assert.Equal(Strings.VERSION_REQUIRED, exception.Message);
        }

        [Fact]
        public async Task TransformsNormalExceptionsToCustomExceptions()
        {
            var (context, next) = GetPostingContexts(null);
            context.HttpContext.Request.Form = new FormCollection(new Dictionary<string, StringValues>
            {
                ["g-recaptcha-response"] = "abcde",
                [Strings.FORM_VERSION_KEY] = "v2"
            });

            var filter = GetFilter();
            filter._verificationService = new MockInvalidGenericExceptionService();
            await filter.OnResourceExecutionAsync(context, next);

            var exception = context.HttpContext.Features.Get<ReCoreVerificationException>();
            Assert.NotNull(exception);
            Assert.Equal(Strings.VERIFICATION_FAILED_DEFAULT_MESSAGE, exception.Message);
            Assert.NotNull(exception.InnerException);
            Assert.IsType<Exception>(exception.InnerException);
        }
    }
}
