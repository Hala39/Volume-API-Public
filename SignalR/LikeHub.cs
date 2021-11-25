using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using VAPI.Data;
using VAPI.Dto.NotificationsDto;
using VAPI.Entities;
using VAPI.Extensions;
using VAPI.Interfaces;
using VAPI.MediatR.Likes;
using VAPI.MediatR.Notifications;

namespace VAPI.SignalR
{
    [Authorize]
    public class LikeHub : Hub
    {
        private readonly Mediator _mediator;
        private readonly DataContext _context;
        private readonly IGroupRepository _groupRepository;
        private readonly PresenceTracker _presenceTracker;
        private readonly IHubContext<PresenceHub> _presenceHub;
        public LikeHub(Mediator mediator, DataContext context, IGroupRepository groupRepository, 
        IHubContext<PresenceHub> presenceHub,
        PresenceTracker presenceTracker)
        {
            _presenceHub = presenceHub;
            _presenceTracker = presenceTracker;
            _groupRepository = groupRepository;
            _context = context;
            _mediator = mediator;
        }

          public override async Task OnConnectedAsync()
        {
            var httpContext = Context.GetHttpContext();
            var postId = httpContext.Request.Query["postId"].ToString();
            var post = await _context.Posts.FirstOrDefaultAsync(p => p.Id == int.Parse(postId));
            var postOwnerId = post.AppUserId;

            var id = Context.User.GetUserId();

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);

            var groupName = GetGroupName(Context.User.GetUserId(), postOwnerId);
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            var group = await AddToGroup(groupName);
            
            var createNotificationDto = new CreateNotificationDto {
                Stimulation = Stimulation.Like,
                TargetId = post.AppUserId,
                Path = int.Parse(postId)
            };

            var guid = await _mediator.Send(new Create.Command { CreateNotificationDto = createNotificationDto});

            if (postOwnerId != id && !group.Connections.Any(x => x.UserId == postOwnerId))
            {
                var connections = await _presenceTracker.GetConnectionsForUser(postOwnerId);
                if (connections != null)
                {
                    await _presenceHub.Clients.Clients(connections).SendAsync("NewLike",
                        new { postId = postId, displayName = user.DisplayName, notificationId = guid.Value });
                }
            }

        }

        public override async Task OnDisconnectedAsync(System.Exception exception)
        {
            var group = await RemoveFromMessageGroup();
            await base.OnDisconnectedAsync(exception);
        }

        private async Task<Group> AddToGroup(string groupName)
        {
            var group = await _groupRepository.GetMessageGroup(groupName);

            var connection = new Connection(Context.ConnectionId, Context.User.GetUserId());

            if (group == null)
            {
                group = new Group(groupName);
                _groupRepository.AddGroup(group);
            }

            group.Connections.Add(connection);

            if (await _groupRepository.SaveAllAsync()) return group;

            throw new HubException("Failed to join group");
        }

        private async Task<Group> RemoveFromMessageGroup()
        {
            var group = await _groupRepository.GetGroupForConnection(Context.ConnectionId);
            var connection = group.Connections.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId);
            _groupRepository.RemoveConnection(connection);
            if (await _groupRepository.SaveAllAsync()) return group;

            throw new HubException("Failed to remove from group");
        }

        private string GetGroupName(string caller, string other)
        {
            var stringCompare = string.CompareOrdinal(caller, other) < 0;
            return stringCompare ? $"like-{caller}-{other}" : $"like-{other}-{caller}";
        }
    }
}