using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VAPI.Data;
using VAPI.Dto;
using VAPI.Entities;
using VAPI.Extensions;
using VAPI.Helpers;
using VAPI.Interfaces;

namespace VAPI.MediatR.Messages
{
    public class Thread
    {
        public class Query : IRequest<Result<PagedList<MessageDto>>>
        {
            public string ContactId { get; set; }
            public PaginationParams Params { get; set; }
            public string Keyword { get; set; }
            
        }

        public class Handler : IRequestHandler<Query, Result<PagedList<MessageDto>>>
        {
            private readonly IMapper _mapper;
            private readonly DataContext _context;
            private readonly IUserAccessor _userAccessor;
            private readonly Mediator _mediator;
            public Handler(DataContext context, IMapper mapper, 
            Mediator mediator, IUserAccessor userAccessor)
            {
                _mediator = mediator;
                _userAccessor = userAccessor;
                _context = context;
                _mapper = mapper;
            }

            public async Task<Result<PagedList<MessageDto>>> Handle(Query request, CancellationToken cancellationToken)
            {
                var userId = _userAccessor.GetUserId();

                await _mediator.Send(new Read.Command { SenderId = request.ContactId});

                var messages = _context.Messages
                    .Where(m => m.RecipientId == userId && m.RecipientDeleted == false
                        && m.SenderId == request.ContactId
                        || m.RecipientId == request.ContactId
                        && m.SenderId == userId && m.SenderDeleted == false
                    )
                    .OrderByDescending(m => m.SentAt)
                    .ProjectTo<MessageDto>(_mapper.ConfigurationProvider)
                    .AsNoTracking()
                    .AsQueryable();

                if(!string.IsNullOrEmpty(request.Keyword))
                {
                    messages = messages.Where(m => m.Content.ToLower().Contains(request.Keyword.ToLower()));
                }

                var pagedList = await PagedList<MessageDto>.CreateAsync(messages, request.Params.PageNumber, request.Params.PageSize);
                
                return Result<PagedList<MessageDto>>.Success(pagedList);
            }
        }
    }
}