using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VAPI.Data;
using VAPI.Dto.SearchDto;
using VAPI.Entities;
using VAPI.Helpers;
using VAPI.Interfaces;

namespace VAPI.MediatR.Search
{
    public class GetSearchKeywords
    {
        public class Query : IRequest<Result<PagedList<SearchOperationDto>>>
        {
            public PaginationParams Params { get; set; }
        }

        public class Handler : IRequestHandler<Query, Result<PagedList<SearchOperationDto>>>
        {
            private readonly DataContext _context;
            private readonly IUserAccessor _userAccessor;
            private readonly IMapper _mapper;
            public Handler(DataContext context, IUserAccessor userAccessor, IMapper mapper)
            {
                _mapper = mapper;
                _userAccessor = userAccessor;
                _context = context;
            }

            public async Task<Result<PagedList<SearchOperationDto>>> Handle(Query request, CancellationToken cancellationToken)
            {

                var operations = 
                    _context.SearchOperations.Where(so => so.AppUserId == _userAccessor.GetUserId())
                    .OrderByDescending(so => so.Date)
                    .ProjectTo<SearchOperationDto>(_mapper.ConfigurationProvider)
                    .AsQueryable();

                var pagedList = await PagedList<SearchOperationDto>.CreateAsync(operations, request.Params.PageNumber, request.Params.PageSize);

                return Result<PagedList<SearchOperationDto>>.Success(pagedList);
            }
        }
    }
}