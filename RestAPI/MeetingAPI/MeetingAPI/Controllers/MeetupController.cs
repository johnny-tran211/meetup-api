using AutoMapper;
using MeetingAPI.Authorization;
using MeetingAPI.Entities;
using MeetingAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using MeetingAPI.Filters;
using System.Linq.Expressions;

namespace MeetingAPI.Controllers
{
    [Route("api/meetup")]
    [Authorize]
    [ServiceFilter(typeof(TimeTrackFilter))]
    public class MeetupController : ControllerBase
    {
        private readonly MeetupContext _meetupContext;
        private readonly IMapper _mapper;
        private readonly IAuthorizationService _authorizationService;

        public MeetupController(MeetupContext meetupContext, IMapper mapper, IAuthorizationService authorizationService)
        {
            _meetupContext = meetupContext;
            _mapper = mapper;
            _authorizationService = authorizationService;
        }
        [HttpGet]
        [AllowAnonymous]
        public ActionResult<List<MeetupDetailsDto>> GetAll([FromQuery] MeetupQuery query)
        {
            if (!ModelState.IsValid) 
            {
                return BadRequest(ModelState);
            }

            var baseQuery = _meetupContext.Meetups
                .Include(m => m.Location)
                .Include(m => m.Lectures)
                .Where(m => query.SearchPhrase == null || 
                            (m.Organizer.ToLower().Contains(query.SearchPhrase.ToLower()) || 
                             m.Name.ToLower().Contains(query.SearchPhrase.ToLower())));

            if (!string.IsNullOrEmpty(query.SortBy)) 
            {
                var propertySelectors = new Dictionary<string, Expression<Func<Meetup, object>>>()
                {
                    { nameof(Meetup.Name), meetup => meetup.Name },
                    { nameof(Meetup.Date), meetup => meetup.Date },
                    { nameof(Meetup.Organizer), meetup => meetup.Organizer }
                };

                var propertySelector = propertySelectors[query.SortBy];

                baseQuery = query.SortDirection == SortDirection.ASC
                    ? baseQuery.OrderBy(propertySelector)
                    : baseQuery.OrderByDescending(propertySelector);
            }


            var meetups = baseQuery
                .Skip(query.PageSize * (query.PageNumber - 1 ))
                .Take(query.PageSize)
                .ToList();

            var itemsCount = baseQuery.Count();

            var meetupDtos = _mapper.Map<List<MeetupDetailsDto>>(meetups);

            var result = new PageResult<MeetupDetailsDto>(meetupDtos, itemsCount, query.PageSize, query.PageNumber);

            return Ok(result);
        }
        [HttpGet("{name}")]
        //[Authorize(Policy = "HasNationality")]
        //[Authorize(Policy = "AtLeast18")]
        [NationalityFilter("German")]
        public ActionResult<MeetupDetailsDto> Get([FromRoute]string name) 
        {
            var meetup = _meetupContext.Meetups
                .Include(i => i.Location)
                .Include(m => m.Lectures)
                .FirstOrDefault(m => m.Name.Replace(" ", "-").ToLower() == name.ToLower());
            if (meetup == null) 
            {
                return NotFound();
            }
            var meetupDto = _mapper.Map<MeetupDetailsDto>(meetup);
            return Ok(meetupDto);
        }
        [HttpPost]
        public ActionResult Post([FromBody] MeetupDto meetupDto) 
        {
            if (!ModelState.IsValid) 
            {
                return BadRequest(ModelState);
            }
            var meetup = _mapper.Map<Meetup>(meetupDto);

            var userId = User.FindFirst(c => c.Type == ClaimTypes.NameIdentifier).Value;

            meetup.CreateById = int.Parse(userId);

            _meetupContext.Meetups.Add(meetup);
            _meetupContext.SaveChanges();

            var key = meetup.Name.Replace(" ", "-").ToLower();

            return Created("api/meetup/" + key, null);
        }
        [HttpPut("{name}")]
        public ActionResult Put(string name, [FromBody] MeetupDto meetupDto) 
        {
            var meetup = _meetupContext.Meetups
                .FirstOrDefault(m => m.Name.Replace(" ", "-").ToLower() == name.ToLower());
            if (meetup == null) 
            {
                return NotFound();
            }

            var authorizationResult = _authorizationService.AuthorizeAsync(User, meetup, new ResourceOperationRequirement(OperationType.Update)).Result;
            if (!authorizationResult.Succeeded) 
            {
                return Forbid();
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            meetup.Name = meetupDto.Name;
            meetup.Organizer = meetupDto.Organizer;
            meetup.Date = meetupDto.Date;
            meetup.IsPrivate = meetupDto.IsPrivate;

            _meetupContext.SaveChanges();

            return NoContent();
        }

        [HttpDelete("{name}")]
        public ActionResult Delete(string name) 
        {
            var meetup = _meetupContext.Meetups
                .FirstOrDefault(m => m.Name.Replace(" ", "-").ToLower() == name.ToLower());
            if (meetup == null) 
            {
                return NotFound();
            }
            var authorizationResult = _authorizationService.AuthorizeAsync(User, meetup, new ResourceOperationRequirement(OperationType.Delete)).Result;
            if (!authorizationResult.Succeeded)
            {
                return Forbid();
            }
            _meetupContext.Remove(meetup);
            _meetupContext.SaveChanges();

            return NoContent();
        }
    }
}
