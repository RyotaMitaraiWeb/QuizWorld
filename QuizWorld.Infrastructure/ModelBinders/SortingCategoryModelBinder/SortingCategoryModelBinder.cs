using Microsoft.AspNetCore.Mvc.ModelBinding;
using QuizWorld.Common.Constants.Sorting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace QuizWorld.Infrastructure.ModelBinders
{
    /// <summary>
    /// If the "sort" query string is an invalid value (an unsupported sorting category), this model binder
    /// will bind the enum value "Title" of type "SortingCategories" to the parameter for which this binder is called.
    /// </summary>
    public class SortingCategoryModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null)
            {
                throw new ArgumentNullException(nameof(bindingContext));
            }

            var request = bindingContext.HttpContext.Request;
            var categoryQuery = request.Query["sort"].ToString();

            SortingCategories category = SortingCategories.Title;

            bool isCategoryEnum = Enum.TryParse(categoryQuery, true, out SortingCategories parsedCategory);

            if (isCategoryEnum)
            {
                category = parsedCategory;
            }


            bindingContext.Result = ModelBindingResult.Success(category);
            return Task.CompletedTask;
        }
    }
}
