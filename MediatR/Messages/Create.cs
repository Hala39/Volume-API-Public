using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using VAPI.Data;
using VAPI.Dto.MessageDtos;
using VAPI.Entities;
using VAPI.Interfaces;
using VAPI.Services;

namespace VAPI.MediatR.Messages
{
    public class Create
    {
        public class Command : IRequest<Result<Guid>>
        {
            public CreateMessageDto CreateMessageDto { get; set; }

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
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == _userAccessor.GetUserId());

                if (user.Id == request.CreateMessageDto.RecipientId) return Result<Guid>.Failure("You cannot chat with yourself");

                var recipient = await _context.Users.FirstOrDefaultAsync(u => u.Id == request.CreateMessageDto.RecipientId);

                if (recipient == null) return Result<Guid>.Failure("Recipient not found");

                var message = new Message
                {
                    Id = Guid.NewGuid(),
                    RecipientId = request.CreateMessageDto.RecipientId,
                    SenderId = _userAccessor.GetUserId(),
                    Content = request.CreateMessageDto.Content,
                    Seen = request.CreateMessageDto.Seen
                };


                await _context.Messages.AddAsync(message);

                var result = await _context.SaveChangesAsync() > 0;

                if (result) return Result<Guid>.Success(message.Id);

                return Result<Guid>.Failure("Unable to send message.");
            }

        }
    }
}