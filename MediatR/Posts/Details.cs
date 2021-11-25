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

namespace VAPI.MediatR.Posts
{
    public class Details
    {
        public class Query : IRequest<Result<PostDto>>
        {
            public int PostId { get; set; }

        }

        public class Handler : IRequestHandler<Query, Result<PostDto>>
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

            public async Task<Result<PostDto>> Handle(Query request, CancellationToken cancellationToken)
            {
                var post = await _context.Posts
                    .Include(p => p.AppUser)
                    .ProjectTo<PostDto>(_mapper.ConfigurationProvider, new { CurrentUserId = _userAccessor.GetUserId()})
                    .FirstOrDefaultAsync(p => p.Id == request.PostId);

                return Result<PostDto>.Success(post);
            }
        }
    }
}