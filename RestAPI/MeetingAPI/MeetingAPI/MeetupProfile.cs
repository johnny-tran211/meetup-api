using AutoMapper;
using MeetingAPI.Entities;
using MeetingAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MeetingAPI
{
    public class MeetupProfile : Profile
    {
        public MeetupProfile()
        {
            CreateMap<Meetup, MeetupDetailsDto>()
                .ForMember(m => m.City, map => map.MapFrom(meetup => meetup.Location.City))
                .ForMember(m => m.Street, map => map.MapFrom(meetup => meetup.Location.Street))
                .ForMember(m => m.PostCode, map => map.MapFrom(meetup => meetup.Location.PostCode));

            CreateMap<MeetupDto, Meetup>();

            CreateMap<LectureDto, Lecture>()
                .ReverseMap();
        }
    }
}
