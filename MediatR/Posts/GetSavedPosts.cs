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
    public class GetSavedPosts
    {
        public class Query : IRequest<Result<PagedList<SavedPostDto>>>
        {
            public PaginationParams Params { get; set; }
            
            
        }

        public class Handler : IRequestHandler<Query, Result<PagedList<SavedPostDto>>>
        {
            private readonly IUserAccessor _userAccessor;
            private readonly IMapper _mapper;
            private readonly DataContext _context;
            public Handler(DataContext context, IMapper mapper, IUserAccessor userAccessor)
            {
                _context = context;
                _mapper = mapper;
                _userAccessor = userAccessor;
            }

            public async Task<Result<PagedList<SavedPostDto>>> Handle(Query request, CancellationToken cancellationToken)
            {
                var savedPosts = _context.SavedPosts.Where(sp => sp.SaverId == _userAccessor.GetUserId())
                    .OrderByDescending(sp => sp.SavedAt)
                    .ProjectTo<SavedPostDto>(_mapper.ConfigurationProvider,
                                new { CurrentUserId = _userAccessor.GetUserId()})
                    .AsNoTracking()
                    .AsQueryable();


                var pagedList = await PagedList<SavedPostDto>.CreateAsync(savedPosts, 
                    request.Params.PageNumber, request.Params.PageSize);

                return Result<PagedList<SavedPostDto>>.Success(pagedList);

            }
        }
    }
}