using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VAPI.Data;
using VAPI.Dto.NotificationsDto;
using VAPI.Entities;
using VAPI.Interfaces;

namespace VAPI.MediatR.Notifications
{
    public class Create
    {
        public class Command : IRequest<Result<Guid>>
        {
            public CreateNotificationDto CreateNotificationDto { get; set; }

        }

        public class Handler : IRequestHandler<Command, Result<Guid>>
        {
            private readonly DataContext _context;
            private readonly IUserAccessor _userAccessor;
            public Handler(DataContext context, IUserAccessor userAccessor)
            {
                _userAccessor = userAccessor;
                _context = context;

            }

            public async Task<Result<Guid>> Handle(Command request, CancellationToken cancellationToken)
            {
                var targetId = request.CreateNotificationDto.TargetId;
                var stimulatorId = _userAccessor.GetUserId();

                if (targetId == stimulatorId) 
                {
                   return Result<Guid>.Success(Guid.Empty);
                }
                
                var notification = new Notification
                {
                    Id = Guid.NewGuid(),
                    TargetId = targetId,
                    Target = await _context.Users.FirstOrDefaultAsync(u => u.Id == targetId),
                    StimulatorId = stimulatorId,
                    Stimulator = await _context.Users.FirstOrDefaultAsync(u => u.Id == stimulatorId),
                    Stimulation = request.CreateNotificationDto.Stimulation,
                    Path = request.CreateNotificationDto.Path
                };

                await _context.Notifications.AddAsync(notification);

                var result = await _context.SaveChangesAsync() > 0;

                if (result) return Result<Guid>.Success(notification.Id);

                return Result<Guid>.Failure("Operation Failed.");
            }
        }
    }
}