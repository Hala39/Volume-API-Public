using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VAPI.Data;
using VAPI.Dto;
using VAPI.Entities;
using VAPI.Interfaces;

namespace VAPI.MediatR.Posts
{
    public class Update
    {
        public class Command : IRequest<Result<Unit>>
        {
            public string Description { get; set; }
            public int PostId { get; set; }
            
        }

        public class Handler : IRequestHandler<Command, Result<Unit>>
        {
            private readonly DataContext _context;
            private readonly IUserAccessor _userAccessor;
            public Handler(DataContext context, IUserAccessor userAccessor)
            {
                _userAccessor = userAccessor;
                _context = context;
            }

            public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == _userAccessor.GetUserId());

                var post = await _context.Posts.FirstOrDefaultAsync(p => p.Id == request.PostId);

                post.Description = request.Description;
                
                var result = await _context.SaveChangesAsync() > 0;

                if (!result) return Result<Unit>.Failure("Failed to update post.");

                return Result<Unit>.Success(Unit.Value);
            }
        }
    }
}