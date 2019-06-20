# Re.Core

[![Build Status](https://shawnthebeachy.visualstudio.com/Re.Core/_apis/build/status/Re.Core%20(develop)?branchName=develop)](https://shawnthebeachy.visualstudio.com/Re.Core/_build/latest?definitionId=6&branchName=develop)

A .NET Core library to help you easily add reCAPTCHA verification to your Razor Pages.

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
    
    else
    {
        // Either the user did not complete the reCAPTCHA or the server-side verification failed.
    }
}
```
