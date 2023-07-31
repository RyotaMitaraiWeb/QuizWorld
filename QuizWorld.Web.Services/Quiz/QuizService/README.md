# QuizService

## Methods
```cs
public QuizService(IRepository repository)
```

### Usage
Instantiates an instance of the class, passing the provided arguments as dependencies.

```cs
public Task<int> CreateQuiz(CreateQuizViewModel quiz, string userId)
public Task<int> CreateQuiz(CreateQuizViewModel quiz, Guid userId)
```

### Usage
Attempts to create a new entry in the database, using the provided ``quiz`` to construct it. The passed ``userId`` will be used to mark who the creator of the quiz is.

### Returns
A task that resolves to an integer, representing the newly-created quiz's ID.

### Exceptions
- ``InvalidOperationException`` - if ``userId`` is passed as a string that cannot be parsed into a GUID.


```cs
public Task<int?> DeleteQuizById(int id)
```

### Usage
Attempts to set the quiz with the given ``id``'s ``IsDeleted`` column to ``true``. When quizzes are marked as deleted, they won't be retrieved in any request. 

### Returns
- If deleted successfully, a Task that resolves to an integer, representing the deleted quiz's ID.
- If the quiz does not exist or is already marked as deleted, a Task that resolves to ``null``.


```cs
public Task<int?> EditQuizById(int id, EditQuizViewModel quiz)
```

### Usage
Attempts to edit the quiz with the given ``id``, using the passed ``quiz`` to apply the changes.

When a quiz is updated, its ``Version`` will be incremented by one and all the questions that came in ``quiz`` will be created with the same version. This means that questions from previous versions will not be retrieved when the user requests the quiz with all the questions (however, users are still able to grade their questions against older versions).

### Returns
- If edited successfully, a Task that resolves to an integer, representing the edited quiz's ID.
- If the quiz does not exist or is marked as deleted, a Task that resolves to ``null``.


```cs
public Task<QuizViewModel?> GetQuizById(int id)
```

### Usage
Attempts to retrieve a quiz alongside all its questions and answers. The answers of each question are shuffled.

### Returns
- If the quiz exists, a Task that resolves to a ``QuizViewModel`` representation of the quiz, with the answers of each question shuffled.
- If the quiz does not exist or is marked as deleted, a Task that resolves to ``null``.


```
public Task<CatalogueQuizViewModel> GetQuizzesByQuery(string query, int page, SortingCategories category, SortingOrders order, int pageSize = 6)
```

### Usage
Attempts to retrieve a paginated and sorted list of quizzes whose titles contain the given ``query`` and are not marked as deleted. The search is case and space insensitive.

### Returns
A task that resolves to a ``CatalogueQuizViewModel``. The ``Total`` property represents the total amount of quizzes in the database whose title contains the given ``query`` and are not marked as deleted.


```
public Task<CatalogueQuizViewModel> GetAllQuizzes(int page, SortingCategories category, SortingOrders order, int pageSize = 6)
```

### Usage
Attempts to retrieve a paginated and sorted list of quizzes that are not marked as deleted.

### Returns
A task that resolves to a ``CatalogueQuizViewModel``. The ``Total`` property represents the total amount of quizzes in the database that are not marked as deleted.

