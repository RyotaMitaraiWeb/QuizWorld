# RoleService

## Methods
```cs
public RoleService(UserManager<ApplicationUser> userManager)
```

### Usage
Creates a new instance of the class, passing the provided arguments as dependencies.


```cs
public Task<ListUsersViewModel> GetUsersOfRole(string role, int page, SortingOrders order, int pageSize = 20)
```

### Usage
Retrieves a list of users that have the given ``role``. The list itself is paginated and sorted.

### Returns
A Task that resolves to a ``ListUsersViewModel`` representation of users that have the given role. The ``Total`` property represents the total amount of users in the database that have the role.

### Exceptions
- ``ArgumentException`` - the provided ``role`` does not exist.


```cs
public Task<Guid?> GiveUserRole(Guid userId, string role)
public Task<Guid?> GiveUserRole(string userId, string role)
```

### Usage
Attempts to give the user with the given ``userId`` the given ``role``. If passed as a GUID, the ID will be parsed to a string. This method refers to the [``RolesThatCanBeGivenOrRemoved``](https://github.com/RyotaMitaraiWeb/QuizWorld/blob/master/QuizWorld.Common/Constants/Roles/Roles.cs) constants to check if the provided role can be given to users on first place.

### Returns
- If the role was added successfully, a Task that resolves to the ID of the user (as a GUID)
- If the role cannot be added to the user (e.g. they already have it), a Task that resolves to ``null``

### Exceptions
- ``ArgumentException`` - the provided ``role`` cannot be given to users (note that this is different from it not existing; existing ones like Administrator cannot be given to users with this method) or a user with the provided ``userId`` does not exist.

```cs
public Task<Guid?> RemoveRoleFromUser(string userId, string role)
public Task<Guid?> RemoveRoleFromUser(Guid userId, string role)
```

### Usage
Attempts to remove the provided ``role`` from the user with the given ``userId``. If passed as a GUID, the ID will be parsed to a string. This method refers to the [``RolesThatCanBeGivenOrRemoved``](https://github.com/RyotaMitaraiWeb/QuizWorld/blob/master/QuizWorld.Common/Constants/Roles/Roles.cs) constants to check if the provided role can be removed from users on first place.

### Returns
- If the role was removed successfully, a Task that resolves to the ID of the user (as a GUID)
- If the role cannot be removed from the user (e.g. they already do not have it), a Task that resolves to ``null``

### Exceptions
- ``ArgumentException`` - the provided ``role`` cannot be removed from users (note that this is different from it not existing; existing ones like Administrator cannot be removed from users with this method) or a user with the provided ``userId`` does not exist.


```cs
public Task<ListUsersViewModel> GetUsersByUsername(string query, int page, SortingOrders order, int pageSize = 20)
```

### Usage
Retrieves a list of users whose usernames contain the given ``query``. The search is case insensitive. The list itself is paginated and sorted.

### Returns
A Task that resolves to a ``ListUsersViewModel`` representation of users that have the given role. The ``Total`` property represents the total amount of users in the database whose username contains the ``query``.