using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using VAPI.Data;
using VAPI.Dto;
using VAPI.Entities;
using VAPI.Helpers;
using VAPI.Interfaces;

namespace VAPI.MediatR.Profiles
{
    public class GetPosts
    {
        public class Query : IRequest<Result<PagedList<PostDto>>>
        {
            public PaginationParams Params { get; set; }
            public string UserId { get; set; }

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
                var posts = _context.Posts
                    .Where(p => p.AppUserId == request.UserId)
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