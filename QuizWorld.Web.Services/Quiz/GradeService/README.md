# GradeService

## Remarks
It is important to note that this service by itself does not do any grading. All the grading happens on the client; this service merely provides the client with the correct answers of a given quiz or question.

## Methods
```cs
public GradeService(IRepository repository)
```

### Usage
Creates a new instance of the class, passing the provided arguments as dependencies.


```cs
public Task<GradedQuestionViewModel?> GetCorrectAnswersForQuestionById(Guid questionId, int version)
```

### Usage
Attempts to retrieve all correct answers for the question with the given ``questionId`` and ``version``. The version does not have to match the current version of the quiz, thus users can request the correct answers for questions of older versions.

### Returns
- if a question of the given ID and version exists, a Task that resolves to an ``GradedQuestionViewModel`` representation of the question.
- if a question of the given ID and version does not exist, a Task that resolves to ``null``.

### Exceptions
- ``InvalidOperationException`` - the quiz is not in instant mode (in that case, you need to use ``GetCorrectAnswersForQuestionsByQuizId``)


```cs
public Task<IEnumerable<GradedQuestionViewModel>?> GetCorrectAnswersForQuestionsByQuizId(int quizId, int version)
```

### Usage
Attempts to retrieve all correct answers for the questions with the given ``version`` that belong to the quiz with the given ``quizId``. The version does not have to match the current version of the quiz, thus users can request the correct answers for questions of older versions.

### Returns
- if a quiz of the given ID and version exists, a Task that resolves to an ``IEnumerable<GradedQuestionViewModel>`` representation of the questions.
- if a question of the given ID and version does not exist, a Task that resolves to ``null``.

### Exceptions
- ``InvalidOperationException`` - the quiz is in instant mode (in that case, you need to use ``GetCorrectAnswersForQuestionById``)