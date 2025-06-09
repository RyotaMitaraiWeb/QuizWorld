# SortingOrderModelBinder


## Methods
```cs
public Task BindModelAsync(ModelBindingContext bindingContext)
```

### Usage
Retrieves the value of the ``order`` query string and checks whether it's a valid sorting order. There are two mechanisms for validating this:

1) The binder will first check if the client has supplied the value in its "short" name. It will refer to the [``AllowedSortingOrders``](https://github.com/RyotaMitaraiWeb/QuizWorld/blob/master/QuizWorld.Common/Constants/Sorting/AllowedSortingOptions.cs) dictionary and check if the dictionary contains a key equivalent to the supplied value. If there is, this method will bind the value of the given key to the paramater for which this model binder was called.

2) If the above fails, the binder will try to convert the value directly to a [``SortingOrders``](https://github.com/RyotaMitaraiWeb/QuizWorld/blob/master/QuizWorld.Common/Constants/Sorting/SortingOrders.cs) enum (the conversion being case insensitive). If successful, the result is binded to the paramater for which the model binder was called.

If both of the above fail, this method will bind ``SortingOrders.Ascending`` to the paramter for which the model binder was called.

### Exceptions
- ``ArgumentNullException`` - ``bindingContext`` is ``null``.