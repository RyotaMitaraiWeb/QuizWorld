using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QuizWorld.Infrastructure.AuthConfig.Policies
{
    public class GuestRequirement : IAuthorizationRequirement
    {
    }
}
