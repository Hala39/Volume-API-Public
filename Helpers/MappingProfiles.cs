using System;
using System.Linq;
using AutoMapper;
using VAPI.Dto;
using VAPI.Dto.AccountDtos;
using VAPI.Dto.NotificationsDto;
using VAPI.Dto.SearchDto;
using VAPI.Entities;

namespace VAPI.Helpers
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            string CurrentUserId = null;
            
            CreateMap<AppUser, AppUserDto>()
                .ForMember(s => s.LastMessageSent, 
                    o => o.MapFrom(d => d.MessagesSent
                        .Where(m => m.RecipientId == CurrentUserId && m.RecipientDeleted == false)
                        .OrderByDescending(m => m.SentAt)
                        .FirstOrDefault()))
                .ForMember(s => s.LastMessageReceived, 
                    o => o.MapFrom(d => d.MessagesReceived
                        .Where(m => m.SenderId == CurrentUserId && m.SenderDeleted == false)
                        .OrderByDescending(m => m.SentAt)
                        .FirstOrDefault()))
                .ForMember(s => s.IsFollowing,
                    o => o.MapFrom(d => d.Followers.Any(f => f.ObserverId == CurrentUserId)));

            CreateMap<UserDto, AppUser>();

            CreateMap<Post, PostDto>()
                .ForMember(s => s.LikesCount, o => o.MapFrom(d => d.Likes.Count))
                .ForMember(s => s.CommentsCount, o => o.MapFrom(d => d.Comments.Count))
                .ForMember(s => s.IsLikedByUser, o => o.MapFrom(d => d.Likes.Any(l => l.SourceId == CurrentUserId)))
                .ForMember(p => p.IsFollowing, 
                    o => o.MapFrom(d => d.AppUser.Followers.Any(f => f.ObserverId == CurrentUserId)))
                .ForMember(p => p.IsSavedByUser, o => o.MapFrom(d => d.SavedPosts.Any(p => p.SaverId == CurrentUserId)));

            CreateMap<SavedPost, SavedPostDto>()
                .ForMember(s => s.Description, o => o.MapFrom(d => d.Post.Description))
                .ForMember(s => s.File, o => o.MapFrom(d => d.Post.File))
                .ForMember(s => s.Id, o => o.MapFrom(d => d.PostId))
                .ForMember(s => s.OwnerDisplayName, o => o.MapFrom(d => d.Saver.DisplayName));

            CreateMap<AppUser, ProfileDto>()
                .ForMember(s => s.IsFollowing, 
                    o => o.MapFrom(d => d.Followers.Any(f => f.ObserverId == CurrentUserId)));

            CreateMap<File, FileDto>();

            CreateMap<SearchOperation, SearchOperationDto>();

            CreateMap<Message, MessageDto>()
                .ForMember(s => s.SenderDisplayName, o => o.MapFrom(d => d.Sender.DisplayName));
            CreateMap<Comment, CommentDto>();

            CreateMap<Notification, NotificationDto>()
                .ForMember(s => s.StimulatorName, o => o.MapFrom(d => d.Stimulator.DisplayName));  
            
            CreateMap<Notification, ActivityDto>()
                .ForMember(s => s.TargetName, o => o.MapFrom(d => d.Target.DisplayName));

            CreateMap<DateTime, DateTime>().ConvertUsing(d => DateTime.SpecifyKind(d, DateTimeKind.Utc));

        }
    }
}