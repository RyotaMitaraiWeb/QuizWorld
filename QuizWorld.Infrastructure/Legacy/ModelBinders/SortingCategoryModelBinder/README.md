# SortingCategoryModelBinder


## Methods
```cs
public Task BindModelAsync(ModelBindingContext bindingContext)
```

### Usage
Retrieves the value of the ``sort`` query string and checks whether it's a valid category sorting category. The method will attempt to convert the value directly to a [``SortingCategories``](https://github.com/RyotaMitaraiWeb/QuizWorld/blob/master/QuizWorld.Common/Constants/Sorting/SortingCategories.cs) enum (the conversion being case insensitive). If successful, the result is binded to the parameter for which this model binder is called, otherwise, it will bind ``SortingCategories.Title``.

### Exceptions
- ``ArgumentNullException`` - ``bindingContext`` is ``null``.