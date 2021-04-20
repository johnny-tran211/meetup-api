using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MeetingAPI.Authorization
{
    public enum OperationType
    {
        Create,
        Read,
        Update,
        Delete
    }
    public class ResourceOperationRequirement : IAuthorizationRequirement
    {
        public OperationType OperationType { get; }
        public ResourceOperationRequirement(OperationType operationType)
        {
            OperationType = operationType;
        }
    }
}
