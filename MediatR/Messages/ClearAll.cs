using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using VAPI.Data;
using VAPI.Entities;
using VAPI.Interfaces;

namespace VAPI.MediatR.Messages
{
    public class ClearAll
    {
        public class Command : IRequest<Result<Unit>>
        {
            public string ContactId { get; set; }


        }

        public class Handler : IRequestHandler<Command, Result<Unit>>
        {
            private readonly IUserAccessor _userAccessor;
            private readonly DataContext _context;
            private readonly Mediator _mediator;
            public Handler(DataContext context, IUserAccessor userAccessor, Mediator mediator)
            {
                _mediator = mediator;
                _context = context;
                _userAccessor = userAccessor;
            }

            public async Task<Result<Unit>> Handle(Command request, CancellationToken cancellationToken)
            {
                var currentUserId = _userAccessor.GetUserId();
                var thread = await _mediator.Send(new Thread.Query { ContactId = request.ContactId});

                var messages = thread.Value;

                foreach (var message in messages)
                {
                    if (message.SenderId == currentUserId && message.SenderDeleted == false) 
                    {
                        message.SenderDeleted = true;
                    }

                    else if (message.RecipientId == currentUserId && message.RecipientDeleted == false) 
                    {
                        message.RecipientDeleted = true;
                    }
                }

                var result = await _context.SaveChangesAsync() > 0;

                if (result) return Result<Unit>.Success(Unit.Value);

                return Result<Unit>.Failure("Operation Failed");
            }
        }
    }
}