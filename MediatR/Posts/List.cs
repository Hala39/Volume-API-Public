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
using VAPI.Helpers;
using VAPI.Interfaces;

namespace VAPI.MediatR.Posts
{
    public class List
    {
        public class Query : IRequest<Result<PagedList<PostDto>>> 
        { 
            public PaginationParams Params { get; set; }
            
        }

        public class Handler : IRequestHandler<Query, Result<PagedList<PostDto>>>
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

            public async Task<Result<PagedList<PostDto>>> Handle(Query request, CancellationToken cancellationToken)
            {
                var userId = _userAccessor.GetUserId();

                var posts = _context.Posts
                    .Include(p => p.File)
                    .Include(p => p.AppUser)
                    .OrderBy(p => p.AppUser.Followers.Any(f => f.ObserverId == userId))
                    .OrderByDescending(p => p.Date)
                    .ProjectTo<PostDto>(_mapper.ConfigurationProvider, new { CurrentUserId = _userAccessor.GetUserId() })
                    .AsQueryable();

                var pagedList = await PagedList<PostDto>.CreateAsync(posts, 
                    request.Params.PageNumber, request.Params.PageSize);
                return Result<PagedList<PostDto>>.Success(pagedList);
            }
        }
    }
}