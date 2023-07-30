# AppJwtBearer

## Remarks
This class inherits ```JwtBearerEvents`` and extends its behavior. In addition to validating the JWT, this class will also check if it's blacklisted, returning 401 errors if the JWT is indeed blacklisted (even if it's otherwise valid).

This class is registered in ``Program.cs`` and is automatically applied to any route that uses the ``[Authorize]`` attribute with a Bearer authentication scheme.

## Methods
```cs
public AppJwtBearerEvents(IJwtService jwtService)
```
Initializes an instance of the class, passing an [``IJwtService``](https://github.com/RyotaMitaraiWeb/QuizWorld/blob/master/QuizWorld.Web.Contracts/Authentication/JsonWebToken/IJwtService.cs) class as a dependency.


```cs
public override Task TokenValidated(TokenValidatedContext context)
```
This method retrieves the user's JWT from the Authorization header and checks if it's blacklisted. If it's blacklisted, the authorization fails and this short-circuits the request with a 401 error. If the token is not blacklisted, the method defers to the ``JwtBearerEvents``'s base method implementation.