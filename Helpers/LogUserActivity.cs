using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using VAPI.Data;
using VAPI.Extensions;
using VAPI.Interfaces;

namespace API.Helpers
{
    public class LogUserActivity : IAsyncActionFilter
    {
        private readonly DataContext _context;
        private readonly IUserAccessor _userAccessor;
        public LogUserActivity(DataContext context, IUserAccessor userAccessor)
        {
            _userAccessor = userAccessor;
            _context = context;
        }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var resultContext = await next();

            if (!resultContext.HttpContext.User.Identity.IsAuthenticated) return;

            var userId = resultContext.HttpContext.User.GetUserId();
            // var repo = resultContext.HttpContext.RequestServices.GetService<IUserRepository>();
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            user.LastActive = DateTime.UtcNow;
            await _context.SaveChangesAsync();
        }

    }
}