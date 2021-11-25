using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VAPI.Data;
using VAPI.Entities;

namespace VAPI.MediatR.Posts
{
    public class Delete
    {
        public class Command : IRequest<Result<Unit>>
        {
            public int PostId { get; set; }
            
        }

        public class Handler : IRequestHandler<Command, Result<Unit>>
        {
            private readonly DataContext _context;
            public Handler(DataContext context)
            {
                _context = context;
            }
            public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
            {
                var post = await _context.Posts.FirstOrDefaultAsync(p => p.Id == request.PostId);

                _context.Posts.Remove(post);
                var result = await _context.SaveChangesAsync() > 0;

                if (!result) return Result<Unit>.Failure("Failed to delete the post");

                return Result<Unit>.Success(Unit.Value);
            }
        }
    }
}