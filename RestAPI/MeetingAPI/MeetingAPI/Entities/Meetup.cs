using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MeetingAPI.Entities
{
    public class Meetup
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Organizer { get; set; }
        public DateTime Date { get; set; }
        public bool IsPrivate { get; set; }

        public virtual Location Location { get; set; }

        public virtual List<Lecture> Lectures { get; set; }

        public int? CreateById { get; set; }
        public User CreateBy { get; set; }
    }
}
