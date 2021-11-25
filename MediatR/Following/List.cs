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
using VAPI.Interfaces;

namespace VAPI.MediatR.Following
{
    public class List
    {
        public class Query : IRequest<Result<List<AppUserDto>>>
        {
            public string Predicate { get; set; }
            public string UserId { get; set; }

        }

        public class Handler : IRequestHandler<Query, Result<List<AppUserDto>>>
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

            public async Task<Result<List<AppUserDto>>> Handle(Query request, CancellationToken cancellationToken)
            {
                var users = new List<AppUserDto>();

                switch (request.Predicate)
                {
                    case "followers":
                        users = await _context.UserFollowings.Where(uf => uf.TargetId == request.UserId)
                            .Select(f => f.Observer)
                            .ProjectTo<AppUserDto>(_mapper.ConfigurationProvider, 
                                new { CurrentUserId = _userAccessor.GetUserId()})
                            .AsNoTracking()
                            .ToListAsync();
                        break;

                    case "followings":
                        users = await _context.UserFollowings.Where(uf => uf.ObserverId == request.UserId)
                            .Select(f => f.Target)
                            .ProjectTo<AppUserDto>(_mapper.ConfigurationProvider, 
                                new { CurrentUserId = _userAccessor.GetUserId()})
                            .AsNoTracking()
                            .ToListAsync();
                        break;
                }

                return Result<List<AppUserDto>>.Success(users);

            }
        }
    }
}