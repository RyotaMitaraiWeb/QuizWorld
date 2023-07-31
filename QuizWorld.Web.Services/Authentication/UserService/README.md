# UserService

## Methods

```cs
public UserService(UserManager<ApplicationUser> userManager, IJwtService jwtService)
```

### Usage
Creates a new instance ot the class, passing the provided arguments as dependencies

```cs
public Task<UserViewModel?> Register(RegisterViewModel user)
```

### Usage
Attempts to create a new user in the database, using the data from the provided ``user``. Each successfully-registered user has the role "User".

### Returns
- If the user is registered successfuly, a Task that resolves to an ``UserViewModel`` representation of the user. The model's ``Roles`` property will always be an array of strings that only contains the value ``"User"``. The returned user can be used to generate JWTs.
- If the user could not be registered successfully, a Task that resolves to ``null``.


```cs
public Task<UserViewModel?> Login(LoginViewModel user)
```

### Usage
Attempts to retrieve a user based on the provided ``user``'s username and compares the hashes of the provided password and the user's actual password.

### Returns
- If the user logged in successfully, a Task that resolves to an ``UserViewModel`` representation of the user. The returned user can be used to generate JWTs.
- If the provided user does not exist or the password is wrong, a Task that resolves to ``null``.


```cs
public Task<bool> Logout(string jwt)
```

### Usage
Uses the injected ``IJwtService`` to blacklist the ``jwt``. A blacklisted JWT cannot be used in authorized requests, even if it's otherwise valid.

**Note:** this method does not validate whether the JWT is valid; whoever calls the method should verify that themself before calling it.


```cs
public Task<bool> CheckIfUsernameIsTaken(string username)
```

### Usage
Checks if the ``username`` is already taken by another user. This is useful when validating a user that is registering.

### Returns
A Task that resolves to a boolean value that indicates whether the ``username`` is already taken by another user.