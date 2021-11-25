using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using MediatR;
using Microsoft.EntityFrameworkCore;
using VAPI.Data;
using VAPI.Entities;
using VAPI.Interfaces;

namespace VAPI.MediatR.Following
{
    public class FollowToggle
    {
        public class Command : IRequest<Result<bool>>
        {
            public string TargetId { get; set; }

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
                var observer = await _context.Users.FirstOrDefaultAsync(u => u.Id == _userAccessor.GetUserId());
                var target = await _context.Users.FirstOrDefaultAsync(u => u.Id == request.TargetId);

                var following = await _context.UserFollowings
                    .FirstOrDefaultAsync(uf => uf.ObserverId == _userAccessor.GetUserId() 
                        && uf.TargetId == request.TargetId);

                bool isFollowing;
                if (following == null)
                {
                    following = new UserFollowing
                    {
                        Target = target,
                        Observer = observer
                    };

                    isFollowing = true;
                    await _context.UserFollowings.AddAsync(following);
                }
                else 
                {
                    isFollowing = false;
                    _context.UserFollowings.Remove(following);
                }

                var result = await _context.SaveChangesAsync() > 0;

                if (result) return Result<bool>.Success(isFollowing);

                return Result<bool>.Failure("Operation failed.");
            }
        }
    }
}