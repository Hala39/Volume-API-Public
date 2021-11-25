using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VAPI.Data;
using VAPI.Entities;
using VAPI.Interfaces;

namespace VAPI.MediatR.Notifications
{
    public class ClearAll
    {
        public class Command : IRequest<Result<Unit>>
        {
            public string Predicate { get; set; }
            
            
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
                List<Notification> notifications;

                if (request.Predicate == "activities")
                {
                    notifications = await _context.Notifications
                        .Where(n => n.StimulatorId == _userAccessor.GetUserId()).ToListAsync();

                    foreach (var item in notifications)
                    {
                        item.StimulatorDeleted = true;

                        if (item.TargetDeleted)
                        {
                            _context.Remove(item);
                        }

                        await _context.SaveChangesAsync();

                    }    
                }
                else if (request.Predicate == "notifications")
                {
                    notifications = await _context.Notifications
                    .Where(n => n.TargetId == _userAccessor.GetUserId()).ToListAsync();

                    foreach (var item in notifications)
                    {
                        item.TargetDeleted = true;

                        if (item.StimulatorDeleted)
                        {
                            _context.Remove(item);
                        }

                        await _context.SaveChangesAsync();

                    }
                }    
                
                var result = await _context.SaveChangesAsync() > 0;

                if (result) return Result<Unit>.Success(Unit.Value);

                return Result<Unit>.Failure("Operation Failed");
            }
        }
    }
}