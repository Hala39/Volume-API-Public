using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VAPI.Data;
using VAPI.Entities;

namespace VAPI.MediatR.Comments
{
    public class Delete
    {
        public class Command : IRequest<Result<Unit>>
        {
            public Guid CommentId { get; set; }

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
                var comment = await _context.Comments.FirstOrDefaultAsync(c => c.Id == request.CommentId);

                _context.Comments.Remove(comment);

                var result = await _context.SaveChangesAsync() > 0;

                if (result) return Result<Unit>.Success(Unit.Value);
                return Result<Unit>.Failure("Unable to delete comment.");
            }
        }
    }
}