﻿using MeetingAPI.Entities;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MeetingAPI.Authorization
{
    public class MeetupResourceOperationHandler : AuthorizationHandler<ResourceOperationRequirement, Meetup>
    {
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, ResourceOperationRequirement requirement, Meetup resource)
        {
            if (requirement.OperationType == OperationType.Create || requirement.OperationType == OperationType.Read) 
            {
                context.Succeed(requirement);
            }

            var userId = context.User.FindFirst(user => user.Type == ClaimTypes.NameIdentifier).Value;
            if (resource.CreateById == int.Parse(userId)) 
            {
                context.Succeed(requirement);
            }
            return Task.CompletedTask;
        }
    }
}
