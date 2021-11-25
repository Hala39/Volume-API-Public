using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VAPI.Data;
using VAPI.Entities;
using VAPI.Interfaces;

namespace VAPI.MediatR.Messages
{
    public class Delete
    {
        public class Command : IRequest<Result<Unit>>
        {
            public Guid Id { get; set; }

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
                var message = await _context.Messages.FirstOrDefaultAsync(m => m.Id == request.Id);

                var userId = _userAccessor.GetUserId();

                if (message.RecipientId == userId)
                {
                    message.RecipientDeleted = true;
                }

                if (message.SenderId == userId)
                {
                    message.SenderDeleted = true;
                }

                if (message.SenderDeleted && message.RecipientDeleted)
                {
                    _context.Messages.Remove(message);
                }

                var result = await _context.SaveChangesAsync() > 0;
                
                if (result) return Result<Unit>.Success(Unit.Value);

                return Result<Unit>.Failure("Unable to delete message!");
               
            }
        }


    }
}