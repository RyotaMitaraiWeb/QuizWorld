# CanWorkWithRoles policy handler/requirement

## Remarks
``CanWorkWithRolesHandler`` and ``CanWorkWithRolesRequirement`` allow you to set up a policy to allow access to a certain resource only if the user has the ability to interact with users' roles on a massive scale, like viewing the roles of users in a list or outright changing them (in this case, the requirement is that the user has at least one of the needed roles for this). The handler takes care of verifying the user's credentials, while the requirement is used to instantiate the policy when registering your policy in ``Program.cs``.

Note that any unauthorized request (e.g. normally 401 and 403 errors) is automatically returned as a 404 error as a security measure. The user's roles are checked against the ones in the database and not the one from the JWT; the reason for this is that a user's role can be changed by an administrator and the server does not keep an active track of the issued JWTs in order to invalidate them on promotion/demotion.

## Methods (Requirement)
```cs
public CanWorkWithRolesRequreiment(bool logActivity, params string[] roles)
```

### Usage
Initializes an instance of the class and sets the ``RolesThatCanChangeRoles`` property to an array of all roles that are passed as parameters. The property is used by the handler to determine if the user can access the requested resource. In addition, the constructor will set the ``LogActivity`` property to the respective value. The handler will use this property to determine whether an authorized entry should be logged in the database (for example, you may want to log when someone changes a user's role, but not when they view the list of users and their roles).

## Methods (Handler)
```cs
public CanWorkWithRolesHandler(IHttpContextAccessor http, UserManager<ApplicationUser> userManager, IJwtService jwtService)
```

### Usage
Initializes an instance of the class and passes the provided arguments as dependencies.

```cs
protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, CanAccessLogsRequirement requirement)
```

### Usage
Retrieves the user's JWT from the Authorization header and attempts to retrieve the user's roles from the database. If the user has at least one of the roles in the requirement's ``RolesThatCanChangeRoles``, the request is authorized. In any other case, the request is short-circuited with a 404 error. If the requirement's ``LogActivity`` is set to ``true``, the handler will also log in the database that the user attempted to send a request to the given endpoint (including the method of the request).

