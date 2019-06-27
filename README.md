# Re.Core

[![Build Status](https://dev.azure.com/shawnthebeachy/Re.Core/_apis/build/status/Re.Core%20(master)?branchName=master)](https://dev.azure.com/shawnthebeachy/Re.Core/_build/latest?definitionId=7&branchName=master)

A .NET Core library to help you easily add reCAPTCHA verification to your Razor Pages.

## Disclaimer

This is just a little project I made to reduce code duplication in one of my apps. It may not use all the best practices for reCAPTCHA.

## Basic usage

### Startup.cs

```c#
services.AddMvc(x =>
{
    x.AddReCore();
});

services.AddReCore(x =>
{
    x.NotCompletedMessage = "Optional custom message";
    x.SecretKey = "<your-secret-key>";
    x.VerificationFailedMessage = "Optional custom message";
    return x;
});
```

### YourPage.cshtml (v2)

```cshtml
@using Re.Core;

<form method="POST">
    @Html.reCAPTCHAv2("<your-site-key>")
</form>
```

### YourPage.cshtml (v3)

```cshtml
@using Re.Core;

<form method="POST">
    @Html.reCAPTCHAv3("<your-site-key>", "action")
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
    
    else
    {
        // Either the user did not complete the reCAPTCHA or the server-side verification failed.
    }
}
```
