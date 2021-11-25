using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VAPI.Data;
using VAPI.Entities;
using VAPI.Interfaces;

namespace VAPI.MediatR.Messages
{
    public class Check
    {
        public class Query : IRequest<Result<bool>>
        {
            
        }

        public class Handler : IRequestHandler<Query, Result<bool>>
        {
            private readonly IUserAccessor _userAccessor;
            private readonly DataContext _context;
            public Handler(DataContext context, IUserAccessor userAccessor)
            {
                _context = context;
                _userAccessor = userAccessor;
            }

            public async Task<Result<bool>> Handle(Query request, CancellationToken cancellationToken)
            {
                var messages = await _context.Messages
                    .Where(n => n.RecipientId == _userAccessor.GetUserId() 
                && n.Seen == false).ToListAsync();

                if (messages.Any())
                {
                    return Result<bool>.Success(true);
                }

                return Result<bool>.Success(false);
            }

        }
    }
}