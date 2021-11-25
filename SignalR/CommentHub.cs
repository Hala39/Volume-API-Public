using System;
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
using VAPI.MediatR.Comments;


namespace VAPI.SignalR
{
    [Authorize]
    public class CommentHub : Hub
    {
        private readonly Mediator _mediator;
        private readonly PresenceTracker _presenceTracker;
        private readonly DataContext _context;
        private readonly IGroupRepository _groupRepository;
        private readonly IHubContext<PresenceHub> _presenceHub;

        public CommentHub(Mediator mediator, PresenceTracker presenceTracker, DataContext context,
        IGroupRepository groupRepository, IHubContext<PresenceHub> presenceHub)
        {
            _presenceHub = presenceHub;
            _groupRepository = groupRepository;
            _context = context;
            _presenceTracker = presenceTracker;
            _mediator = mediator;
        }

        public async Task AddComment(Create.Command command)
        {
            var comment = await _mediator.Send(command);
            var id = Context.User.GetUserId();
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
            var post = await _context.Posts.FirstOrDefaultAsync(p => p.Id == command.PostId);
            var postOwnerId = post.AppUserId;
            var notificationDto = new CreateNotificationDto
            {
                Stimulation = Stimulation.Comment,
                TargetId = postOwnerId,
                Path = command.PostId
            };

            var guid = await _mediator.Send(new VAPI.MediatR.Notifications.Create.Command { CreateNotificationDto = notificationDto});

            await Clients.Group(command.PostId.ToString())
                .SendAsync("ReceiveComment", comment.Value);

            var groupName = GetGroupName(command.PostId.ToString());
            var group = await _groupRepository.GetMessageGroup(groupName);

            if (!group.Connections.Any(x => x.UserId == postOwnerId)) 
            {
                var connections = await _presenceTracker.GetConnectionsForUser(postOwnerId);

                if (connections != null)
                {
                    await _presenceHub.Clients.Clients(connections).SendAsync("NewComment",
                        new { postId = command.PostId, displayName = user.DisplayName, notificationId = guid.Value});
                }
            }
            
            
        }

        public override async Task OnConnectedAsync()
        {
            var httpContext = Context.GetHttpContext();
            var postId = httpContext.Request.Query["postId"];
            var groupName = GetGroupName(postId);
            var group = await AddToGroup(groupName);
            await Groups.AddToGroupAsync(Context.ConnectionId, postId);
        }

        public override async Task OnDisconnectedAsync(Exception exception)
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

        private string GetGroupName(string postId)
        {
            return $"comment-{postId}";
        }

        
    }
}