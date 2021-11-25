using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VAPI.Data;
using VAPI.Entities;
using VAPI.Interfaces;

namespace VAPI.MediatR.Posts
{
    public class SavePostToggle
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

                var savedPost = await _context.SavedPosts.FirstOrDefaultAsync(sp => sp.SaverId == user.Id && sp.PostId == post.Id);

                bool isSaved;

                if (savedPost != null) 
                {
                    user.SavedPosts.Remove(savedPost);
                    isSaved = false;
                }
                else
                {
                    savedPost = new SavedPost
                    {
                        Saver = user,
                        SaverId = user.Id,
                        Post = post,
                        PostId = post.Id
                    };
                    
                    user.SavedPosts.Add(savedPost);
                    isSaved = true;
                }
                
                var result = await _context.SaveChangesAsync() > 0;

                if (result) return Result<bool>.Success(isSaved);

                return Result<bool>.Failure("Opeartion Failed");
            }
        }
    }
}