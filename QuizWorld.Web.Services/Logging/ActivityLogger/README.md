# ActivityLogger

## Remarks
The activity logger is not a debugging tool, but is used to log the activity of moderators and administrators when performing security-sensitive tasks (like promoting users, deleting other users' quizzes, and so on).

## Methods
```cs
public ActivityLogger(IRepository repository)
```
Creates a new instance of the class, passing the provided arguments as dependencies.


```cs
public Task LogActivity(string message, DateTime date)
```

### Usage
Attempts to create a new log in the database with the given ``message`` and denoting that the activity occurred on the given ``date``.


```cs
public Task<ActivityLogsViewModel> RetrieveLogs(int page, SortingOrders order, int pageSize = 6)
```

### Usage
Retrieves a paginated and sorted list of logs.

### Returns
A Task that resolves to a ``ActivityLogsViewModel`` representation of the retrieved logs. The ``Total`` property represents the total amount of logs in the database.