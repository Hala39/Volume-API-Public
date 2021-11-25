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
    public class FollowHub : Hub
    {
        private readonly Mediator _mediator;
        private readonly DataContext _context;
        private readonly IGroupRepository _groupRepository;
        private readonly PresenceTracker _presenceTracker;
        private readonly IHubContext<PresenceHub> _presenceHub;
        public FollowHub(Mediator mediator, DataContext context, IGroupRepository groupRepository, 
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
            var targetId = httpContext.Request.Query["targetId"].ToString();
            var target = await _context.Users.FirstOrDefaultAsync(p => p.Id == targetId);

            var observerId = Context.User.GetUserId();
            var observer = await _context.Users.FirstOrDefaultAsync(u => u.Id == observerId);

            var groupName = GetGroupName(observerId, targetId);
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            var group = await AddToGroup(groupName);

            var createNotificationDto = new CreateNotificationDto {
                Stimulation = Stimulation.Follow,
                TargetId = targetId
            };

            var guid = await _mediator.Send(new Create.Command { CreateNotificationDto = createNotificationDto});

            if (!group.Connections.Any(x => x.UserId == targetId))
            {
                var connections = await _presenceTracker.GetConnectionsForUser(targetId);
                if (connections != null)
                {
                    await _presenceHub.Clients.Clients(connections).SendAsync("NewFollower",
                        new { observerId = observerId, displayName = observer.DisplayName, notificationId = guid.Value });
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
            return stringCompare ? $"follow-{caller}-{other}" : $"follow-{other}-{caller}";
        }
    }
}