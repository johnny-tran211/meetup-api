using MeetingAPI.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MeetingAPI
{
    public class MeetupSeeder
    {
        private readonly MeetupContext _meetupContext;
        public MeetupSeeder(MeetupContext meetupContext)
        {
            _meetupContext = meetupContext;
        }
        public void Seed() {
            if (_meetupContext.Database.CanConnect()) {
                if (!_meetupContext.Meetups.Any()) {
                    InsertSampleData();
                }
            }
        }

        private void InsertSampleData()
        {
            var meetups = new List<Meetup>
            {
                new Meetup
                {
                    Name = "Web summit",
                    Date = DateTime.Now.AddDays(7),
                    IsPrivate = false,
                    Organizer = "Mircosoft",
                    Location = new Location
                    { 
                        City = "HCM",
                        Street = "Cach Mang Thang 8",
                        PostCode = "71234"
                    },
                    Lectures = new List<Lecture>
                    {
                        new Lecture
                        { 
                            Author = "Duy Hoang",
                            Topic = "Modern browsers",
                            Description = "Deep live into V8"
                        }
                    }
                },
                new Meetup
                {
                    Name = "4Devs",
                    Date = DateTime.Now.AddDays(7),
                    IsPrivate = false,
                    Organizer = "KGD",
                    Location = new Location
                    {
                        City = "HCM",
                        Street = "Thong nhat",
                        PostCode = "70007"
                    },
                    Lectures = new List<Lecture>
                    {
                        new Lecture
                        {
                            Author = "Hoang Tran",
                            Topic = "react.js",
                            Description = "new technical"
                        },
                        new Lecture
                        {
                            Author = "Johnny",
                            Topic = "angular.js",
                            Description = "new frontend technical"
                        }
                    }
                }
            };
            _meetupContext.AddRange(meetups);
            _meetupContext.SaveChanges();
        }
    }
}
