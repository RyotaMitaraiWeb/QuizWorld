using Microsoft.AspNetCore.Identity;
using Microsoft.VisualBasic;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizWorld.Infrastructure.Data.Entities.Identity
{
    /// <summary>
    /// This entity replaces the standard IdentityUser by using GUID IDs instead of the standard
    /// string IDs.
    /// </summary>
    public class ApplicationUser : IdentityUser<Guid>
    {
        public ApplicationUser()
        {
            Id = Guid.NewGuid();
        }


        public virtual ICollection<ApplicationUserRole> UserRoles { get; set; }
    }
}
