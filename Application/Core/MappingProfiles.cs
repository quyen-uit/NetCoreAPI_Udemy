using Application.Activities;
using AutoMapper;
using Domain;
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
            CreateMap<Activity, Activity>();
            CreateMap<Activity, ActivityDto>()
                .ForMember(des => des.HostUserName, act => act.MapFrom(src => src.ActivityAttendees.FirstOrDefault(x => x.IsHost).AppUser.UserName))
                .ForMember(des => des.Attendees, act => act.MapFrom(src => src.ActivityAttendees));

            CreateMap<ActivityAttendee, AttendeeDto>()
                    .ForMember(des => des.Displayname, act => act.MapFrom(src => src.AppUser.DisplayName))
                    .ForMember(des => des.Bio, act => act.MapFrom(src => src.AppUser.Bio))
                    .ForMember(des => des.UserName, act => act.MapFrom(src => src.AppUser.UserName))
                    .ForMember(des => des.Image, act => act.MapFrom(src => src.AppUser.Photos.FirstOrDefault(x => x.IsMain).Url));

            CreateMap<AppUser, Profiles.Profile>()
                .ForMember(des => des.Image, act => act.MapFrom(src => src.Photos.FirstOrDefault(x => x.IsMain).Url));
        }

    }
}
