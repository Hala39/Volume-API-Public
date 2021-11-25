using MediatR;
using VAPI.Helpers;
using VAPI.Dto;
using VAPI.Entities;
using System.Threading.Tasks;
using System.Threading;
using VAPI.Data;
using AutoMapper;
using VAPI.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using AutoMapper.QueryableExtensions;
using System.Collections.Generic;

namespace VAPI.MediatR.Messages
{
    public class Contact
    {
        public class Query : IRequest<Result<PagedList<AppUserDto>>>
        {
            public UserParams Params { get; set; }
            
        }

        public class Handler : IRequestHandler<Query, Result<PagedList<AppUserDto>>>
        {
            private readonly DataContext _context;
            private readonly IMapper _mapper;
            private readonly IUserAccessor _userAccessor;

            public Handler(DataContext context, IMapper mapper, IUserAccessor userAccessor)
            {
                _userAccessor = userAccessor;
                _mapper = mapper;
                _context = context;
            }

            public async Task<Result<PagedList<AppUserDto>>> Handle(Query request, CancellationToken cancellationToken)
            {
                var userId = _userAccessor.GetUserId();

                var contacts = _context.Users
                    .Include(u => u.MessagesReceived)
                    .Include(u => u.MessagesSent)
                    .Where(u => (u.MessagesReceived.Any(m => m.SenderId == userId)
                    || (u.MessagesSent.Any(m => m.RecipientId == userId))))
                    .OrderBy(u => u.MessagesSent.Any(m => m.RecipientId == userId && m.Seen == false))
                    .ProjectTo<AppUserDto>(_mapper.ConfigurationProvider, new { CurrentUserId = userId })
                    .AsNoTracking();
                    
                if (!string.IsNullOrEmpty(request.Params.Keyword)) 
                {
                    contacts = contacts.Where(c => c.DisplayName.ToLower().Contains(request.Params.Keyword.ToLower()));
                }

                var pagedList = await PagedList<AppUserDto>.CreateAsync(contacts, 
                    request.Params.PageNumber, request.Params.PageSize);

                return Result<PagedList<AppUserDto>>.Success(pagedList);
            }
        }
    }
}