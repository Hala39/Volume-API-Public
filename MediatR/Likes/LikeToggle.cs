using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VAPI.Data;
using VAPI.Entities;
using VAPI.Interfaces;

namespace VAPI.MediatR.Likes
{
    public class LikeToggle
    {
        public class Command : IRequest<Result<bool>>
        {
            public int PostId { get; set; }

        }

        public class Handler : IRequestHandler<Command, Result<bool>>
        {
            private readonly IUserAccessor _userAccessor;
            private readonly DataContext _context;
            public Handler(DataContext context, IUserAccessor userAccessor)
            {
                _context = context;
                _userAccessor = userAccessor;
            }

            public async Task<Result<bool>> Handle(Command request, CancellationToken cancellationToken)
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == _userAccessor.GetUserId());

                var post = await _context.Posts.FirstOrDefaultAsync(p => p.Id == request.PostId);

                var like = await _context.UserLikes.FirstOrDefaultAsync(ul => ul.SourceId == user.Id && ul.PostId == post.Id);

                bool isLiked;

                if (like == null)
                {
                    like = new UserLike
                    {
                        Post = post,
                        PostId = request.PostId,
                        Source = user,
                        SourceId = user.Id
                    };

                    isLiked = true;
                    await _context.UserLikes.AddAsync(like);
                } 
                else
                {
                    isLiked = false;
                    _context.UserLikes.Remove(like);
                }

                var result = await _context.SaveChangesAsync() > 0;

                if (result) return Result<bool>.Success(isLiked);

                return Result<bool>.Failure("Unable to add like.");
            }
        }
    }
}