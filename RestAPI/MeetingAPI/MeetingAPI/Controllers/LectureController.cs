using AutoMapper;
using MeetingAPI.Entities;
using MeetingAPI.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MeetingAPI.Controllers
{
    [Route("api/meetup/{meetupName}/lecture")]
    public class LectureController : ControllerBase
    {
        private readonly MeetupContext _meetupContext;
        private readonly IMapper _mapper;
        private readonly ILogger _logger;
        public LectureController(MeetupContext meetupContext, IMapper mapper, ILogger<LectureController> logger)
        {
            _meetupContext = meetupContext;
            _mapper = mapper;
            _logger = logger;
        }
        [HttpGet]
        public ActionResult<List<LectureDto>> Get(string meetupName) 
        {
            var meetup = _meetupContext.Meetups
                .Include(m => m.Lectures)
                .FirstOrDefault(m => m.Name.Replace(" ", "-").ToLower() == meetupName.ToLower());
            if (meetup == null) 
            {
                return NotFound();
            }
            var lecture = _mapper.Map<List<LectureDto>>(meetup.Lectures);

            // Write Log
            _logger.LogWarning($"Get List Lecture successfully");
            
            return Ok(lecture);
        }

        [HttpPost]
        public ActionResult Post(string meetupName, [FromBody] LectureDto lectureDto) 
        {
            var meetup = _meetupContext.Meetups
                .Include(i => i.Lectures)
                .FirstOrDefault(m => m.Name.Replace(" ", "-").ToLower() == meetupName.ToLower());
            if (meetup == null) 
            {
                return NotFound();
            }

            if (!ModelState.IsValid) 
            {
                return BadRequest(ModelState);
            }
            var lecture = _mapper.Map<Lecture>(lectureDto);
            meetup.Lectures.Add(lecture);
            _meetupContext.SaveChanges();

            return Created($"api/meetup/{meetupName}", null);
        }

        [HttpDelete]
        public ActionResult Delete(string meetupName) 
        {
            var meetup = _meetupContext.Meetups
                .Include(m => m.Lectures)
                .FirstOrDefault(m => m.Name.Replace(" ", "-").ToLower() == meetupName.ToLower());
            if (meetup == null) 
            {
                return NotFound();
            }
            _meetupContext.Lectures.RemoveRange(meetup.Lectures);
            _meetupContext.SaveChanges();

            return NoContent();
        }

        [HttpDelete("{id}")]
        public ActionResult Delete(string meetupName, int id)
        {
            var meetup = _meetupContext.Meetups
                .Include(m => m.Lectures)
                .FirstOrDefault(m => m.Name.Replace(" ", "-").ToLower() == meetupName.ToLower());
            if (meetup == null)
            {
                return NotFound();
            }

            var leture = meetup.Lectures.FirstOrDefault(m => m.Id == id);
            if (leture == null)
            {
                return NotFound();
            }

            _meetupContext.Lectures.Remove(leture);
            _meetupContext.SaveChanges();

            return NoContent();
        }
    }
}
