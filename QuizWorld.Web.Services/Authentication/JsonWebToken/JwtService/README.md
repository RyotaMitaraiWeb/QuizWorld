# JwtService

## Remarks
Because the ``IJWTBlacklist`` is a singleton, this service is also registered as a service. All JWTs passed (except for ``RemoveBearer``) must have the ``Bearer `` part removed before passing them. Use the aforementioned ``RemoveBearer`` method before calling any of those methods. 

## Methods
```cs
public JwtService(IJwtBlacklist blacklist, IConfiguration config)
```

### Usage
Creates a new instance of the class, passing the provided arguments as dependencies


```cs
public Task<bool> CheckIfJWTHasBeenInvalidated(string jwt)
```

### Usage
Checks if the provided ``jwt`` is blacklisted.

### Returns
A Task that resolves to a boolean value that indicates whether the token is blacklisted.


```cs
public string GenerateJWT(UserViewModel user)
```

### Usage
Attempts to generate a JWT based on the provided ``user``. The JWT is valid for 24 hours.

### Returns
A JWT with the user encoded in it.


```cs
public UserViewModel DecodeJWT(string jwt)
```

### Usage
Attempts to retrieve the user from the provided ``jwt``.

### Returns
The user that was decoded in the JWT.

### Exceptions
- ``InvalidOperationException`` - at least one of the properties of the user object is ``null`` after decoding.


```cs
public Task<bool> InvalidateJWT(string jwt)
```

### Usage
Attempts to blacklist the provided ``jwt`` using the injected ``IJwtBlacklist``.

### Returns
A Task that resolves to a boolean value that indicates whether the operation was successful.


```cs
public string RemoveBearer(string? bearerToken)
```

### Usage
JWTs in authorized requests are sent in the Authorization header under the value ``Bearer {jwt}``. You can use this method to remove ``Bearer `` part (note the space) and work directly with the JWT.

### Returns
- If ``bearerToken`` is ``null``, an empty string.
- Otherwise, a token in which all instances of ``Bearer `` have been removed.


```cs
public Task<bool> CheckIfJWTIsValid(string jwt)
```

### Usage
Checks if the provided ``jwt`` is valid. This is useful if you need to validate the token, but do not have access to the ``Authorize`` attribute (e.g. implementation of a policy).

### Returns
A Task that resolves to a boolean value that indicates whether the token is valid or not.