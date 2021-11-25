using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VAPI.Data;
using VAPI.Dto.NotificationsDto;
using VAPI.Entities;
using VAPI.Helpers;
using VAPI.Interfaces;

namespace VAPI.MediatR.Notifications
{
    public class List
    {
        public class Query : IRequest<Result<PagedList<NotificationDto>>>
        {
            public PaginationParams Params { get; set; }
            public string Predicate { get; set; }
            
        }

        public class Handler : IRequestHandler<Query, Result<PagedList<NotificationDto>>>
        {
            private readonly DataContext _context;
            private readonly IUserAccessor _userAccessor;
            private readonly IMapper _mapper;
            private readonly Mediator _mediator;
            public Handler(DataContext context, IUserAccessor userAccessor, IMapper mapper, Mediator mediator)
            {
                _mediator = mediator;
                _mapper = mapper;
                _userAccessor = userAccessor;
                _context = context;

            }
            public async Task<Result<PagedList<NotificationDto>>> Handle(Query request, CancellationToken cancellationToken)
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == _userAccessor.GetUserId());
                var notifications =  _context.Notifications
                            .Where(n => n.TargetId == user.Id && n.TargetDeleted == false)
                            .OrderByDescending(n => n.Date)
                            .ProjectTo<NotificationDto>(_mapper.ConfigurationProvider)
                            .AsNoTracking()
                            .AsQueryable();

                var pagedList = await PagedList<NotificationDto>.CreateAsync(notifications, 
                    request.Params.PageNumber, request.Params.PageSize);

                return Result<PagedList<NotificationDto>>.Success(pagedList);

            }
        }
    }
}