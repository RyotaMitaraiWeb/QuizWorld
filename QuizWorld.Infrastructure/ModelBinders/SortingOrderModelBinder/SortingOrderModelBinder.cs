using Microsoft.AspNetCore.Mvc.ModelBinding;
using QuizWorld.Common.Constants.Sorting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizWorld.Infrastructure.ModelBinders.SortingOrderModelBinder
{
    /// <summary>
    /// If the "sort" query string is an invalid value (an unsupported sorting order), this model binder
    /// will bind the enum value "Ascending" of type "SortingOrders" to the parameter for which this binder is called.
    /// </summary>
    public class SortingOrderModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null)
            {
                throw new ArgumentNullException(nameof(bindingContext));
            }

            var request = bindingContext.HttpContext.Request;
            var orderQuery = request.Query["order"].ToString();

            SortingOrders order = SortingOrders.Ascending;

            if (AllowedSortingOptions.Orders.ContainsKey(orderQuery))
            {
                order = AllowedSortingOptions.Orders[orderQuery];
            }
            else
            {
                bool isOrderEnum = Enum.TryParse(orderQuery, true, out SortingOrders parsedOrder);

                if (isOrderEnum)
                {
                    order = parsedOrder;
                }
            }

            bindingContext.Result = ModelBindingResult.Success(order);
            return Task.CompletedTask;
        }
    }
}
