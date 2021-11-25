using System;
using System.Linq;
using VAPI.Data;
using VAPI.Entities;

namespace VAPI.Extensions
{
    public static class QueryableExtensions
    {
        public static IQueryable<Message> MarkUnreadAsRead(this IQueryable<Message> query, string currentUserId, DataContext context)
        {
            var unreadMessages = query.Where(m => m.Seen == false
                && m.RecipientId == currentUserId);

            if (unreadMessages.Any())
            {
                foreach (var message in unreadMessages)
                {
                    message.Seen = true;
                }
            }

            return query;
        }
    }
}