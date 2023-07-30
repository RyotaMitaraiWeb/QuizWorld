# Common

## Overview
This project holds constant values, custom enums, and other utility values. The idea behind this is to avoid magic strings, numbers, and other stuff, making it easier to change the value if needed and providing a better IntelliSense.

## Navigation

### InvalidActionsMessages
Holds constant values related to failed actions (such as an unsuccessful login, insufficient permissions, and etc.).

### Roles
Holds constant values related to roles, such as their names. You are also given two useful collections: 
- ``RolesThatCanBeGivenOrRemoved`` - an array of strings denoting which roles can be given to or removed from users (for example, one cannot promote a user to an administrator, even though the role exists in the application)
- ``AvailableRoles`` - an array that lists the roles that exist in the application.

### Sorting
Provides constant values related to sorting and ordering catalogue-like pages. The following custom enums are provided:

```cs
public enum SortingCategories
{
    Title,
    CreatedOn,
    UpdatedOn,
}
```

```cs
public enum SortingOrders
{
    Ascending,
    Descending,
}
```

In addition, you are provided with two dictionaries: 
```cs
public static readonly Dictionary<string, SortingCategories> Categories = new()
{
    { "title", SortingCategories.Title },
    { "createdOn", SortingCategories.CreatedOn },
    { "updatedOn", SortingCategories.UpdatedOn },
};
```

```cs
public static readonly Dictionary<string, SortingOrders> Orders = new()
{
    { "asc", SortingOrders.Ascending },
    { "desc", SortingOrders.Descending },
};
```

Those can be used to check if the requested sorting order or category is valid.

### Types
Holds constants related to the question types.

```cs
public enum QuestionTypes
{  
    SingleChoice,
    MultipleChoice,
    Text
}
```

```cs
public static class QuestionTypesFullNames
{
    public const string SingleChoice = "Single-choice";
    public const string MultipleChoice = "Multiple-choice";
    public const string Text = "Text";
}
```

```cs
public static class QuestionTypesShortNames
{
    public const string SingleChoice = "single";
    public const string MultipleChoice = "multiple";
    public const string Text = "text";
}
```

### ValidationErrorMessages
Holds constants with errors for failed validations

### ValidationRules
Holds constants with values for validation (for example, maximum length of username, minimum amount of wrong answers for a single-choice question, and so on.)