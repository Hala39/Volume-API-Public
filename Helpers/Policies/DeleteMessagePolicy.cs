using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using VAPI.Data;

namespace VAPI.Helpers
{
    public class DeleteMessagePolicy : IAuthorizationRequirement
    {
        
    }

    public class DeleteMessagePolicyHandler : AuthorizationHandler<DeleteMessagePolicy>
    {
        private readonly DataContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public DeleteMessagePolicyHandler(DataContext dbContext, 
            IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            _dbContext = dbContext;
        }
        
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, DeleteMessagePolicy requirement)
        {
            var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null) return Task.CompletedTask;

            var messageId = Guid.Parse(_httpContextAccessor.HttpContext?.Request.RouteValues
                .SingleOrDefault(x => x.Key == "id").Value?.ToString());
                
            var message = _dbContext.Messages.FirstOrDefault(m => m.Id == messageId);

            if (message.SenderId == userId || message.RecipientId == userId) context.Succeed(requirement);


            return Task.CompletedTask;
        }

    }
}


    
    