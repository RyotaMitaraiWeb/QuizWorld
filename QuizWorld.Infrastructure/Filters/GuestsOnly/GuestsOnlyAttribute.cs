using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizWorld.Infrastructure.Filters.GuestsOnly
{
    /// <summary>
    /// Prevents access to a given route or controller by logged in users. Only guests can
    /// access it.
    /// </summary>
    public class GuestsOnlyAttribute : ServiceFilterAttribute
    {
        public GuestsOnlyAttribute() : base(typeof(GuestsOnlyFilter))
        {
        }
    }
}
