using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizWorld.Infrastructure.Data.Entities
{
    /// <summary>
    /// Replaces the standard IdentityRole by replacing the primary key with a GUID
    /// </summary>
    public class ApplicationRole : IdentityRole<Guid>
    {
        public ApplicationRole()
        {
            this.Id = Guid.NewGuid();
        }

        public virtual ICollection<ApplicationUserRole> UserRoles { get; set; }

    }
}
