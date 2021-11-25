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

namespace VAPI.MediatR.Profiles
{
    public class Get
    {
        public class Query : IRequest<Result<ProfileDto>>
        {
            public string UserId { get; set; }

        }

        public class Handler : IRequestHandler<Query, Result<ProfileDto>>
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

            public async Task<Result<ProfileDto>> Handle(Query request, CancellationToken cancellationToken)
            {
                var profile = await _context.Users
                    .ProjectTo<ProfileDto>(_mapper.ConfigurationProvider, new { CurrentUserId = _userAccessor.GetUserId()})
                    .FirstOrDefaultAsync(u => u.Id == request.UserId);

                return Result<ProfileDto>.Success(profile);
                
            }
        }
    }
}