using System;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using VAPI.Data;
using VAPI.Dto;
using VAPI.Dto.MessageDtos;
using VAPI.Entities;
using VAPI.Extensions;
using VAPI.Helpers;
using VAPI.Interfaces;
using VAPI.MediatR.Messages;

namespace VAPI.SignalR
{
    [Authorize]
    public class MessageHub : Hub
    {
        private readonly IMediator _mediator;
        private readonly DataContext _context;
        private readonly PresenceTracker _presenceTracker;
        private readonly IHubContext<PresenceHub> _presenceHub;
        private readonly IMapper _mapper;
        private readonly IGroupRepository _groupRepository;
        public MessageHub(IMediator mediator, IMapper mapper, IGroupRepository groupRepository,
            DataContext context, IHubContext<PresenceHub> presenceHub, PresenceTracker presenceTracker)
        {
            _groupRepository = groupRepository;
            _mapper = mapper;
            _presenceHub = presenceHub;
            _presenceTracker = presenceTracker;
            _context = context;
            _mediator = mediator;
        }

        public override async Task OnConnectedAsync()
        {
            var httpContext = Context.GetHttpContext();
            var contactId = httpContext.Request.Query["contactId"].ToString();
            var groupName = GetGroupName(Context.User.GetUserId(), contactId);
            await Groups.AddToGroupAsync(Context.ConnectionId, groupName);
            var group = await AddToGroup(groupName);
            await _mediator.Send(new Read.Command { SenderId = contactId});
            await Clients.Group(groupName).SendAsync("UpdatedGroup", group);

        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var group = await RemoveFromMessageGroup();
            await base.OnDisconnectedAsync(exception);
        }

        public async Task SendMessage(CreateMessageDto createMessageDto)
        {
           
            var id = Context.User.GetUserId();

            var sender = await _context.Users.FirstOrDefaultAsync(u => u.Id == id);
            var recipient = await _context.Users.FirstOrDefaultAsync(u => u.Id == createMessageDto.RecipientId);

            var groupName = GetGroupName(sender.Id, recipient.Id);
            var group = await _groupRepository.GetMessageGroup(groupName);

            var guid = await _mediator.Send(new Create.Command { CreateMessageDto = createMessageDto});
            var message = await _context.Messages.FirstOrDefaultAsync(m => m.Id == guid.Value);

            if (group.Connections.Any(x => x.UserId == recipient.Id))
            {
                message.Seen = true;
            }
            else
            {
                var connections = await _presenceTracker.GetConnectionsForUser(recipient.Id);
                if (connections != null)
                {
                    await _presenceHub.Clients.Clients(connections).SendAsync("NewMessageReceived",
                        new { id = sender.Id, displayName = sender.DisplayName, guid = guid.Value });
                }
            }

       
            await Clients.Group(groupName).SendAsync("NewMessage", _mapper.Map<MessageDto>(message));
            
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
            return stringCompare ? $"message-{caller}-{other}" : $"message-{other}-{caller}";
        }
    }
}