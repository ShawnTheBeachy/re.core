# Re.Core

A .NET Core library to help you easily add reCAPTCHA verification to your Razor Pages.

## Basic usage

### Startup.cs

```c#
services.AddMvc(x =>
{
    x.AddReCore();
})

services.AddReCore(x =>
{
    x.NotCompletedMessage = "Optional custom message";
    x.SecretKey = "<your-secret-key>";    // This is required to be set here.
    x.VerificationFailedMessage = "Optional custom message";
    return x;
});
```

### YourPage.cshtml

```cshtml
@using Re.Core;

<form method="POST">
  @Html.reCAPTCHA("<your-site-key>")
</form>
```

### YourPage.cshtml.cs

```c#
public void OnPost()
{
  if (ModelState.IsValid)
  {
    // Voila! The reCAPTCHA was completed and successfully verified.
  }
}
```
