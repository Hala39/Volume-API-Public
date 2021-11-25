using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VAPI.Data;
using VAPI.Entities;
using VAPI.Interfaces;

namespace VAPI.MediatR.Notifications
{
    public class Read
    {
        public class Command : IRequest<Result<Unit>>
        {

        }

        public class Handler : IRequestHandler<Command, Result<Unit>>
        {
            private readonly DataContext _context;
            private readonly IUserAccessor _userAccessor;
            private readonly IMapper _mapper;
            public Handler(DataContext context, IUserAccessor userAccessor, IMapper mapper)
            {
                _mapper = mapper;
                _userAccessor = userAccessor;
                _context = context;
            }

            public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
            {
                var notifications = await _context.Notifications
                    .Where(n => n.TargetId == _userAccessor.GetUserId() && n.Seen == false)
                    .ToListAsync();

                if (notifications.Any())
                {
                    foreach (var item in notifications)
                    {
                        item.Seen = true;
                        await _context.SaveChangesAsync();
                    }
                }

                return Result<Unit>.Success(Unit.Value);
            }
        }
    }
}