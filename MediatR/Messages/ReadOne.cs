using System;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VAPI.Data;
using VAPI.Entities;

namespace VAPI.MediatR.Messages
{
    public class ReadOne
    {
        public class Command : IRequest<Result<Unit>>
        {
            public Guid Guid { get; set; }

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
                var message = await 
                    _context.Messages.FirstOrDefaultAsync(m => m.Id == request.Guid);

                message.Seen = true;

                var result = await _context.SaveChangesAsync() > 0;

                if (result) return Result<Unit>.Success(Unit.Value);
                return Result<Unit>.Failure("OPeration Failed");
            }
        }
    }
}