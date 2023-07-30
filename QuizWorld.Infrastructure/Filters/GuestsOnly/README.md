# GuestsOnly

## Remarks
This filter can be applied to any route via the [``GuestsOnlyAttribute``](https://github.com/RyotaMitaraiWeb/QuizWorld/blob/master/QuizWorld.Infrastructure/Filters/GuestsOnly/GuestsOnlyAttribute.cs) class (annotated as ``[GuestsOnly]``). In order for this filter to work, it must be combined with the ``AllowAnonymous`` attribute, as all routes are authorized by default. Example:

```cs
[AllowAnonymous]
[GuestsOnly]
public IActionResult MyRoute()
{
	// insert route behavior here
}
```

## Methods (filter handler)

```cs
public Task OnAuthorizationAsync(AuthorizationFilterContext context)
```
### Usage
Retrieves the bearer token from the ``Authorization`` header and checks if it's valid. If the token is missing, invalid, expired, or is found in the JWT blacklist, the user's request will be authorized, otherwise, the filter will short-circuit the request with a 403 error. If any exception is thrown within the method, the filter will short-circuit the request with a 503 error.