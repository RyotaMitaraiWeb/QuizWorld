using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QuizWorld.Infrastructure.Data.Entities.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizWorld.Infrastructure.Data
{
    public class RoleConfigurer : IEntityTypeConfiguration<ApplicationRole>
    {
        public void Configure(EntityTypeBuilder<ApplicationRole> builder)
        {
            builder.HasData
            (
                new ApplicationRole()
                {
                    Id = new Guid("CACE07A2-930C-4029-9465-019D78EDCF68"),
                    Name = "User",
                    NormalizedName = "USER",
                },
                new ApplicationRole()
                {
                    Id = new Guid("90F00FD4-9966-4819-AA58-98836F0E0DDF"),
                    Name = "Administrator",
                    NormalizedName = "ADMINISTRATOR",
                },
                new ApplicationRole()
                {
                    Id = new Guid("6448A395-D249-4AEF-BF74-9CDEEBA69F33"),
                    Name = "Moderator",
                    NormalizedName = "MODERATOR",
                }
            );
        }
    }
}
