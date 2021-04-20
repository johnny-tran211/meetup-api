using FluentValidation;
using MeetingAPI.Entities;
using MeetingAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MeetingAPI.Validators
{
    public class RegisterUserValidator : AbstractValidator<RegisterUserDto>
    {
        public RegisterUserValidator(MeetupContext meetupContext)
        {
            RuleFor(x => x.Email).NotEmpty();
            RuleFor(x => x.Password).NotEmpty();
            RuleFor(x => x.ConfirmPassword).NotEmpty();
            RuleFor(x => x.Email).Custom((value, context) => {
                var userAlreadyExist = meetupContext.Users.Any(u => u.Email == value);
                if (userAlreadyExist) 
                {
                    context.AddFailure("Email", "That email address is taken");
                }
            });

            RuleFor(x => x.Password).MinimumLength(6);
            RuleFor(x => x.Password).Equal(x => x.ConfirmPassword).WithMessage("Password must be equal to Confirm Password");
        }
    }
}
