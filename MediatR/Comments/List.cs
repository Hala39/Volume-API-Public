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

namespace VAPI.MediatR.Comments
{
    public class List
    {
        public class Query : IRequest<Result<PagedList<CommentDto>>>
        {
            public int PostId { get; set; }
            public PaginationParams Params { get; set; }
        }

        public class Handler : IRequestHandler<Query, Result<PagedList<CommentDto>>>
        {
            private readonly DataContext _context;
            private readonly IMapper _mapper;
            public Handler(DataContext context, IMapper mapper)
            {
                _mapper = mapper;
                _context = context;
            }

            public async Task<Result<PagedList<CommentDto>>> Handle(Query request, CancellationToken cancellationToken)
            {
                var comments = _context.Comments
                    .Where(c => c.PostId == request.PostId)
                    .OrderByDescending(c => c.Date)
                    .ProjectTo<CommentDto>(_mapper.ConfigurationProvider)
                    .AsQueryable();

                var pagedList = await PagedList<CommentDto>.CreateAsync(comments, request.Params.PageNumber, request.Params.PageSize);    
                return Result<PagedList<CommentDto>>.Success(pagedList);
            }
        }
    }
}