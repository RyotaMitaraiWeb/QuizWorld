using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using QuizWorld.Common.Constants.Roles;
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
                    Name = Roles.User,
                    NormalizedName = Roles.User.ToUpper(),
                },
                new ApplicationRole()
                {
                    Id = new Guid("90F00FD4-9966-4819-AA58-98836F0E0DDF"),
                    Name = Roles.Admin,
                    NormalizedName = Roles.Admin.ToUpper(),
                },
                new ApplicationRole()
                {
                    Id = new Guid("6448A395-D249-4AEF-BF74-9CDEEBA69F33"),
                    Name = Roles.Moderator,
                    NormalizedName = Roles.Moderator.ToUpper(),
                }
            );
        }
    }
}
