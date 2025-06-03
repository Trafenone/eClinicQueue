using eClinicQueue.API.Models.Dtos.Auth;
using FluentValidation;
using System.Text.RegularExpressions;

namespace eClinicQueue.API.Validators.Auth;

public class PhoneLoginDtoValidator : AbstractValidator<PhoneLoginDto>
{
    public PhoneLoginDtoValidator()
    {
        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("Phone number is required")
            .Matches(new Regex(@"^\+?[1-9]\d{1,14}$")).WithMessage("Phone number must be in a valid international format");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required")
            .MinimumLength(6).WithMessage("Password must be at least 6 characters long");
    }
}