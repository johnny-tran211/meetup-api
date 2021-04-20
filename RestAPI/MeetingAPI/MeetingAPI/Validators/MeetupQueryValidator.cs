using FluentValidation;
using MeetingAPI.Entities;
using MeetingAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MeetingAPI.Validators
{
    public class MeetupQueryValidator : AbstractValidator<MeetupQuery>
    {
        private int[] allowedPageSizes = new[] { 5, 15, 20 };
        private string[] allowedSortingByColumnNames = { nameof(Meetup.Name), nameof(Meetup.Date), nameof(Meetup.Organizer) };
        public MeetupQueryValidator()
        {
            RuleFor(x => x.PageNumber).GreaterThanOrEqualTo(1);
            RuleFor(x => x.PageSize).Custom((value, context) =>
            {
                if (!allowedPageSizes.Contains(value)) 
                {
                    context.AddFailure("PageSize", $"Page size must be in {string.Join(",", allowedPageSizes)}");
                }
            });

            RuleFor(x => x.SortBy)
                .Must(value => string.IsNullOrEmpty(value) || allowedSortingByColumnNames.Contains(value))
                .WithMessage($"Sort by is optional, or it has to be in ({string.Join(",", allowedSortingByColumnNames)})");
        }
    }
}
