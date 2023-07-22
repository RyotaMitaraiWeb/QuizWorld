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
        /// Creates a user with the given <paramref name="username"/> and a hash of <paramref name="password"/> and gives them all roles.
        /// </summary>
        /// <param name="app"></param>
        /// <param name="username">The password in plain format. The password will be hashed.</param>
        /// /// <param name="password"></param>
        /// <returns></returns>
        public static IApplicationBuilder SeedAdministrator(this IApplicationBuilder app, string username, string password)
        {
            using IServiceScope scopedServices = app.ApplicationServices.CreateScope();

            IServiceProvider serviceProvider = scopedServices.ServiceProvider;
            var hasher = new PasswordHasher<ApplicationUser>();

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
                    await userManager.AddToRoleAsync(admin, Roles.User);
                    return;
                    
                }

                admin = new ApplicationUser()
                {
                    UserName = username,
                    NormalizedUserName = username.ToUpper(),
                };

                admin.PasswordHash = hasher.HashPassword(admin, password);


                await userManager.CreateAsync(admin);
                await userManager.AddToRoleAsync(admin, Roles.Admin);
                await userManager.AddToRoleAsync(admin, Roles.Moderator);
                await userManager.AddToRoleAsync(admin, Roles.User);
            })
            .GetAwaiter()
            .GetResult();

            return app;
        }
    }
}
