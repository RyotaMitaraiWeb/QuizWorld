using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.WebUtilities;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace QuizWorld.Infrastructure.ModelBinders
{
    [Obsolete("Use simple classes instead")]
    /// <summary>
    /// If the "page" query string is an invalid value (a non-integer or a non-positive integer), this model binder
    /// will bind the integer "1" to the parameter for which this binder is called.
    /// </summary>
    public class PaginationModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null)
            {
                throw new ArgumentNullException(nameof(bindingContext));
            }

            var request = bindingContext.HttpContext.Request;
            var pageQuery = request.Query["page"].ToString();

            bool isAnIntenger = int.TryParse(pageQuery, out int page);

            if (!isAnIntenger)
            {
                page = 1;
            }
            else
            {
                if (page < 1)
                {
                    page = 1;
                }
            }


            bindingContext.Result = ModelBindingResult.Success(page);
            return Task.CompletedTask;
        }
    }
}
