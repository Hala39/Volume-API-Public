using System;
using System.Linq;
using System.Threading.Tasks;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using VAPI.Data;
using VAPI.Entities;
using VAPI.Extensions;
using VAPI.Interfaces;
using VAPI.MediatR.Messages;
using VAPI.MediatR.Notifications;

namespace VAPI.SignalR
{
    [Authorize]
    public class PresenceHub : Hub
    {
        private readonly PresenceTracker _presenceTracker;
        private readonly IGroupRepository _groupRepository;
        private readonly Mediator _mediator;
        private readonly DataContext _context;

        public PresenceHub(PresenceTracker presenceTracker, 
        IGroupRepository groupRepository,
        Mediator mediator, DataContext context)
        {
            _context = context;
            _mediator = mediator;
            _groupRepository = groupRepository;
            _presenceTracker = presenceTracker;
        }

        public override async Task OnConnectedAsync()
        {
            var isOnline = await _presenceTracker.UserConnected(Context.User.GetUserId(), Context.ConnectionId);

            if (isOnline)
                await Clients.Others.SendAsync("UserIsOnline", Context.User.GetUserId());

            var onlineUsers = await _presenceTracker.GetOnlineUsers();
            await Clients.Caller.SendAsync("GetOnlineUsers", onlineUsers);

            var notifications = await _mediator.Send(new MediatR.Notifications.Check.Query());

            await Clients.Caller.SendAsync("CheckNotifications", notifications.Value);

            var contacts = await _mediator.Send(new MediatR.Messages.Check.Query());

            await Clients.Caller.SendAsync("CheckMessages", contacts.Value);
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var isOffline = await _presenceTracker.UserDisconnected(Context.User.GetUserId(), Context.ConnectionId);

            if (isOffline)
                await Clients.Others.SendAsync("UserIsOffline", Context.User.GetUserId());

            await base.OnDisconnectedAsync(exception);
        }

    }
}

