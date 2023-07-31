# CanPerformOwnerActions policy handler/requirement

## Remarks
``CanPerformOwnerActionsHandler`` and ``CanPerformOwnerActionsRequirement`` allow you to set up a policy to allow access to a certain resource only if the user is the creator of a given quiz or has one of the specified roles. The handler takes care of verifying the user's credentials, while the requirement is used to instantiate the policy when registering your policy in ``Program.cs``.

The handler retrieves the quiz's ID from an ``{id}`` route variable and therefore can only be used in routes that also feature it.

Note that the user's roles are checked against the ones in the database and not the one from the JWT; the reason for this is that a user's role can be changed by an administrator and the server does not keep an active track of the issued JWTs in order to invalidate them on promotion/demotion.

## Methods (Requirement)
```cs
public CanPerformOwnerActionsRequirement(params string[] roles)
```

### Usage
Initializes an instance of the class and sets the ``RolesThatCanPerformAction`` property to an array of all roles that are passed as parameters. The property is used by the handler to determine if the user can access the requested resource, assuming that they are not the creator of the quiz.

## Methods (Handler)
```cs
public CanPerformOwnerActionsHandler(IHttpContextAccessor http, UserManager<ApplicationUser> userManager, IJwtService jwtService)
```

### Usage
Initializes an instance of the class and passes the provided arguments as dependencies.

```cs
protected override async Task HandleRequirementAsync(AuthorizationHandlerContext context, CanAccessLogsRequirement requirement)
```

### Usage
Retrieves the user's JWT from the Authorization header. If the user is not logged in, the request is short-circuited with a 401 error. The handler then attempts to retrieve the quiz with the given ``id`` (retrieved from route variable ``{id}``). If the quiz does not exist or is marked as deleted, the handler short-circuits the request with a 404 error. If the user is the creator of the quiz or has one of the roles specified in the requirement's ``RolesThatCanPerformAction``, the request is authorized, otherwise, it is short-circuited with a 403 error.

If the handler throws an exception at any point (e.g. cannot access the database because it's down), the handler will short-circuit the request with a 503 error.