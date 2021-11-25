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
    public class DeleteCommentPolicy : IAuthorizationRequirement
    {
        
    }

    public class DeleteCommentPolicyHandler : AuthorizationHandler<DeleteCommentPolicy>
    {
        private readonly DataContext _dbContext;
        private readonly IHttpContextAccessor _httpContextAccessor;
        public DeleteCommentPolicyHandler(DataContext dbContext, 
            IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
            _dbContext = dbContext;
        }
        
        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, DeleteCommentPolicy requirement)
        {
            var userId = context.User.FindFirstValue(ClaimTypes.NameIdentifier);

            if (userId == null) return Task.CompletedTask;

            var CommentId = Guid.Parse(_httpContextAccessor.HttpContext?.Request.RouteValues
                .SingleOrDefault(x => x.Key == "id").Value?.ToString());
                
            var comment = _dbContext.Comments.FirstOrDefault(m => m.Id == CommentId);

            if (comment.AppUserId == userId) context.Succeed(requirement);


            return Task.CompletedTask;
        }

    }
}


    
    