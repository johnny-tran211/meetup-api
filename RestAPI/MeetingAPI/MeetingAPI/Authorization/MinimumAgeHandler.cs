using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MeetingAPI.Authorization
{
    public class MinimumAgeHandler : AuthorizationHandler<MinimumAgeRequirement>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, MinimumAgeRequirement requirement)
        {
            if (context.User.HasClaim(c => c.Type == ClaimTypes.DateOfBirth)) 
            {
                var dateOfBirth = DateTime.ParseExact(context.User.FindFirst(c => c.Type == ClaimTypes.DateOfBirth).Value, "dd-MM-yyyy", null);
                if (dateOfBirth.AddYears(requirement.MinimumAge) <= DateTime.Now)
                {
                    context.Succeed(requirement);
                }
            }
            
            return Task.CompletedTask;
        }
    }
}
