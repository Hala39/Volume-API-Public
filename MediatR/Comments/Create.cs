using System;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VAPI.Data;
using VAPI.Dto;
using VAPI.Entities;
using VAPI.Interfaces;

namespace VAPI.MediatR.Comments
{
    public class Create
    {
        public class Command : IRequest<Result<CommentDto>>
        {
            public int PostId { get; set; }
            public string Content { get; set; }

        }

        public class Handler : IRequestHandler<Command, Result<CommentDto>>
        {
            private readonly IUserAccessor _userAccessor;
            private readonly DataContext _context;
            private readonly IMapper _mapper;
            public Handler(DataContext context, IUserAccessor userAccessor, IMapper mapper)
            {
                _mapper = mapper;
                _context = context;
                _userAccessor = userAccessor;
            }

            public async Task<Result<CommentDto>> Handle(Command request, CancellationToken cancellationToken)
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == _userAccessor.GetUserId());

                var post = await _context.Posts.FirstOrDefaultAsync(p => p.Id == request.PostId);

                var comment = new Comment
                {
                    Id = Guid.NewGuid(),
                    AppUser = user,
                    Post = post,
                    PostId = post.Id,
                    Content = request.Content,
                };

                await _context.Comments.AddAsync(comment);

                var commentDto = _mapper.Map<CommentDto>(comment);

                var result = await _context.SaveChangesAsync() > 0;

                if (result) return Result<CommentDto>.Success(commentDto);

                return Result<CommentDto>.Failure("Unable to add comment.");

            }
        }
    }
}