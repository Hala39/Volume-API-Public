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
    public class CommandPolicy : IAuthorizationRequirement
    {
    }

    public class CommandPolicyHandler : AuthorizationHandler<CommandPolicy>
    {
        private readonly DataContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public CommandPolicyHandler(DataContext dbContext, 
            IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            _dbContext = dbContext;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, CommandPolicy requirement)
        {
            var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null) return Task.CompletedTask;

            var postId = int.Parse(_httpContextAccessor.HttpContext?.Request.RouteValues
                .SingleOrDefault(x => x.Key == "id").Value?.ToString());

            var post = _dbContext.Posts.FirstOrDefault(p => p.Id == postId);

            if (post.AppUserId == userId) context.Succeed(requirement);

            return Task.CompletedTask;
        }
    }
}