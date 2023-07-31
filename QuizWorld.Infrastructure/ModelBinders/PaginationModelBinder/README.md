# PaginationModelBinder


## Methods
```cs
public Task BindModelAsync(ModelBindingContext bindingContext)
```

### Usage
Retrieves the value of the ``page`` query string and checks whether it's a positive integer. If the query string is missing, cannot be converted to an integer, or is a non-positive integer, this method will bind the value ``"1"`` to the parameter for which this model binder is called, otherwise, it will bind whatever value was passed to the query string.

### Exceptions
- ``ArgumentNullException`` - ``bindingContext`` is ``null``.