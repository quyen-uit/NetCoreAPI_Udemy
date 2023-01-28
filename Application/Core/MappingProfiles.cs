using Application.Activities;
using Application.Activities.Dtos;
using Application.Comments;
using AutoMapper;
using Domain;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Core
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            string currentUserName = null;
            CreateMap<Activity, Activity>()
               .ForMember(des => des.IsCancelled, act => act.Ignore());
            CreateMap<Activity, ActivityDto>()
                .ForMember(des => des.HostUserName, act => act.MapFrom(src => src.ActivityAttendees.FirstOrDefault(x => x.IsHost).AppUser.UserName))
                .ForMember(des => des.Attendees, act => act.MapFrom(src => src.ActivityAttendees));

            CreateMap<ActivityAttendee, AttendeeDto>()
                    .ForMember(des => des.DisplayName, act => act.MapFrom(src => src.AppUser.DisplayName))
                    .ForMember(des => des.Bio, act => act.MapFrom(src => src.AppUser.Bio))
                    .ForMember(des => des.UserName, act => act.MapFrom(src => src.AppUser.UserName))
                    .ForMember(des => des.Image, act => act.MapFrom(src => src.AppUser.Photos.FirstOrDefault(x => x.IsMain).Url))
                    .ForMember(des => des.FollowersCount, act => act.MapFrom(src => src.AppUser.Followers.Count))
                    .ForMember(des => des.FollowingsCount, act => act.MapFrom(src => src.AppUser.Followings.Count))
                    .ForMember(des => des.Following, act => act.MapFrom(src => src.AppUser.Followers.Any(x => x.Observer.UserName == currentUserName)));

            CreateMap<AppUser, Profiles.Dtos.Profile>()
                .ForMember(des => des.Image, act => act.MapFrom(src => src.Photos.FirstOrDefault(x => x.IsMain).Url))
                .ForMember(des => des.FollowersCount, act => act.MapFrom(src => src.Followers.Count))
                .ForMember(des => des.FollowingsCount, act => act.MapFrom(src => src.Followings.Count))
                .ForMember(des => des.Following, act => act.MapFrom(src => src.Followers.Any(x => x.Observer.UserName == currentUserName)));

            CreateMap<Comment,CommentDto>()
                .ForMember(des => des.DisplayName, act => act.MapFrom(src => src.Author.DisplayName))
                .ForMember(des => des.UserName, act => act.MapFrom(src => src.Author.UserName))
                .ForMember(des => des.Image, act => act.MapFrom(src => src.Author.Photos.FirstOrDefault(x => x.IsMain).Url));

            CreateMap<ActivityAttendee, Profiles.Dtos.UserActivityDto>()
             .ForMember(des => des.Id, act => act.MapFrom(src => src.ActivityId))
             .ForMember(des => des.Title, act => act.MapFrom(src => src.Activity.Title))
             .ForMember(des => des.Category, act => act.MapFrom(src => src.Activity.Category))
             .ForMember(des => des.Date, act => act.MapFrom(src => src.Activity.Date))
             .ForMember(des => des.HostUserName, act => act.MapFrom(src => src.Activity.ActivityAttendees.FirstOrDefault(x => x.IsHost).AppUser.UserName));


        }

    }
}
