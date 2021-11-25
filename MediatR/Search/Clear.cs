using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VAPI.Data;
using VAPI.Entities;
using VAPI.Interfaces;

namespace VAPI.MediatR.Search
{
    public class Clear
    {
        public class Command : IRequest<Result<Unit>>
        {

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
                var searchOperations = await _context.SearchOperations
                    .Where(so => so.AppUserId == _userAccessor.GetUserId()).ToListAsync();

                _context.SearchOperations.RemoveRange(searchOperations);

                var result = await _context.SaveChangesAsync() > 0;

                if (result) return Result<Unit>.Success(Unit.Value);

                return Result<Unit>.Failure("Operation failed!"); 
            }
        }
    }
}