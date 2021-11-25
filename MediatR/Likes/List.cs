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
using VAPI.Helpers;
using VAPI.Interfaces;

namespace VAPI.MediatR.Likes
{
    public class List
    {
        public class Query : IRequest<Result<PagedList<AppUserDto>>>
        {
            public int PostId { get; set; }
            public PaginationParams Params { get; set; }

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
                var likers = _context.Users
                    .Where(u => u.LikedPosts.Any(l => l.PostId == request.PostId))
                    .ProjectTo<AppUserDto>(_mapper.ConfigurationProvider,
                                new { CurrentUserId = _userAccessor.GetUserId()})
                    .AsQueryable();

                var pagedList = await PagedList<AppUserDto>.CreateAsync(likers,
                    request.Params.PageNumber, request.Params.PageSize);

                return Result<PagedList<AppUserDto>>.Success(pagedList);
            }
        }
    }
}