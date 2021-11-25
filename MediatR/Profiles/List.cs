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

namespace VAPI.MediatR.Profiles
{
    public class List
    {
        public class Query : IRequest<Result<PagedList<AppUserDto>>>
        {
            public UserParams Params { get; set; }

        }

        public class Handler : IRequestHandler<Query, Result<PagedList<AppUserDto>>>
        {
            private readonly IMapper _mapper;
            private readonly DataContext _context;
            private readonly IUserAccessor _userAccessor;
            public Handler(DataContext context, IMapper mapper, IUserAccessor userAccessor)
            {
                _userAccessor = userAccessor;
                _context = context;
                _mapper = mapper;
            }

            public async Task<Result<PagedList<AppUserDto>>> Handle(Query request, CancellationToken cancellationToken)
            {
                var userId = _userAccessor.GetUserId();
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
                
                var query = _context.Users.Where(u => u.Id != userId).AsQueryable();

                var keyword = request.Params.Keyword;

                if (request.Params.Suggest == true) 
                {   
                    query = query.Where(u => !u.Followers.Any(f => f.ObserverId == userId))
                        .OrderBy(u => u.Title.Contains(user.Title)).ThenBy(u => u.Hometown.Contains(user.Hometown))
                        .OrderBy(u => u.Followers.Count);
                }

                if (keyword != null)
                {
                    query = query.Where(u => u.DisplayName.ToLower().Contains(keyword.ToLower()) 
                    || u.Title.ToLower().Contains(keyword.ToLower()) 
                    || u.UserName.ToLower().Contains(keyword.ToLower()));

                }

                var users = query
                    .OrderBy(u => u.Followers.Any(f => f.ObserverId == userId))
                    .OrderBy(u => u.Followers.Any(f => f.TargetId == userId))
                    .OrderBy(u => u.Hometown.Contains(user.Hometown))
                    .OrderBy(u => u.Title.Contains(user.Title))
                    .ProjectTo<AppUserDto>(_mapper.ConfigurationProvider, new { CurrentUserId = userId });

                var pagedList = await PagedList<AppUserDto>.CreateAsync(users, 
                    request.Params.PageNumber, request.Params.PageSize);

                return Result<PagedList<AppUserDto>>.Success(pagedList);
            }
        }
    }
}