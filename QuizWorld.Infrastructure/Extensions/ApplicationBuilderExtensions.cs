using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using QuizWorld.Common.Constants.Roles;
using QuizWorld.Infrastructure.Data.Entities.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizWorld.Infrastructure.Extensions
{
    public static class ApplicationBuilderExtensions
    {
        /// <summary>
        /// Gives the user with the given <paramref name="username"/> the Administrator and Moderator roles if they exist.
        /// </summary>
        /// <param name="app"></param>
        /// <param name="username"></param>
        /// <returns></returns>
        public static IApplicationBuilder SeedAdministrator(this IApplicationBuilder app, string username)
        {
            using IServiceScope scopedServices = app.ApplicationServices.CreateScope();

            IServiceProvider serviceProvider = scopedServices.ServiceProvider;

            UserManager<ApplicationUser> userManager =
                serviceProvider.GetRequiredService<UserManager<ApplicationUser>>();
            RoleManager<ApplicationRole> roleManager =
                serviceProvider.GetRequiredService<RoleManager<ApplicationRole>>();

            Task.Run(async () =>
            {
                ApplicationUser admin =
                    await userManager.FindByNameAsync(username);

                if (admin != null)
                {
                    await userManager.AddToRoleAsync(admin, Roles.Admin);
                    await userManager.AddToRoleAsync(admin, Roles.Moderator);
                }
            })
            .GetAwaiter()
            .GetResult();

            return app;
        }
    }
}
