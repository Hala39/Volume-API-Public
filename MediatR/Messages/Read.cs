using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VAPI.Data;
using VAPI.Entities;
using VAPI.Interfaces;

namespace VAPI.MediatR.Messages
{
    public class Read
    {
        public class Command : IRequest<Result<Unit>>
        {
            public string SenderId { get; set; }
            
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
                var messages = await _context.Messages
                    .Where(n => n.RecipientId == _userAccessor.GetUserId() 
                    && n.SenderId == request.SenderId
                    && n.Seen == false)
                    .ToListAsync();

                if (messages.Any())
                {
                    foreach (var item in messages)
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