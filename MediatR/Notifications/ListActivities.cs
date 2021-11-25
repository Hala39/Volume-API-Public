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
    public class ListActivities
    {
        public class Query : IRequest<Result<PagedList<ActivityDto>>>
        {
            public PaginationParams Params { get; set; }
            
            
        }

        public class Handler : IRequestHandler<Query, Result<PagedList<ActivityDto>>>
        {
            private readonly IUserAccessor _userAccessor;
            private readonly DataContext _context;
            private readonly IMapper _mapper;
            public Handler(DataContext context, IUserAccessor userAccessor, IMapper mapper)
            {
                _mapper = mapper;
                _context = context;
                _userAccessor = userAccessor;
            }

            public async Task<Result<PagedList<ActivityDto>>> Handle(Query request, CancellationToken cancellationToken)
            {
                var activities = _context.Notifications
                    .Where(n => n.StimulatorId == _userAccessor.GetUserId()
                        && n.StimulatorDeleted == false)
                    .OrderByDescending(n => n.Date)
                    .ProjectTo<ActivityDto>(_mapper.ConfigurationProvider)
                    .AsQueryable();

                var pagedList = await PagedList<ActivityDto>.CreateAsync(activities, 
                    request.Params.PageNumber, request.Params.PageSize);

                    return Result<PagedList<ActivityDto>>.Success(pagedList);
            }
        }
    }
}