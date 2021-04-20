using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace MeetingAPI.Models
{
    public class LectureDto
    {
        [Required]
        [MinLength(3)]
        public string Author { get; set; }
        [Required]
        [MinLength(3)]
        public string Topic { get; set; }
        public string Description { get; set; }
    }
}
