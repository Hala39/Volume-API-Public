using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VAPI.Data;
using VAPI.Entities;

namespace VAPI.MediatR.Notifications
{
    public class ReadOne
    {
        public class Command : IRequest<Result<bool>>
        {
            public Guid Id { get; set; }


        }

        public class Handler : IRequestHandler<Command, Result<bool>>
        {
            private readonly DataContext _context;
            public Handler(DataContext context)
            {
                _context = context;
            }

            public async Task<Result<bool>> Handle(Command request, CancellationToken cancellationToken)
            {
                var notification = await 
                    _context.Notifications.FirstOrDefaultAsync(m => m.Id == request.Id);

                notification.Seen = true;

                await _context.SaveChangesAsync();

                return Result<bool>.Success(notification.Seen);

            }
        }
    }
}