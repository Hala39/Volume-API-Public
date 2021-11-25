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

namespace VAPI.MediatR.Profiles
{
    public class GetPhotos
    {
        public class Query : IRequest<Result<PagedList<FileDto>>>
        {
            public string UserId { get; set; }
            public PaginationParams Params { get; set; }
            
        }

        public class Handler : IRequestHandler<Query, Result<PagedList<FileDto>>>
        {
            private readonly IMapper _mapper;
            private readonly DataContext _context;
            public Handler(DataContext context, IMapper mapper)
            {
                _context = context;
                _mapper = mapper;
            }

            public async Task<Result<PagedList<FileDto>>> Handle(Query request, CancellationToken cancellationToken)
            {
                var photos = _context.Files
                    .Where(f => f.AppUserId == request.UserId)
                    .OrderByDescending(f => f.Date)
                    .ProjectTo<FileDto>(_mapper.ConfigurationProvider)
                    .AsQueryable();

                var pagedList = await PagedList<FileDto>.CreateAsync(photos, 
                    request.Params.PageNumber, request.Params.PageSize);

                return Result<PagedList<FileDto>>.Success(pagedList);
            }
        }
    }
}