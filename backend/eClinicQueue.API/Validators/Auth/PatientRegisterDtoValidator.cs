using eClinicQueue.API.Models.Dtos.Auth;
using FluentValidation;
using System.Text.RegularExpressions;

namespace eClinicQueue.API.Validators.Auth;

public class PatientRegisterDtoValidator : AbstractValidator<PatientRegisterDto>
{
    public PatientRegisterDtoValidator()
    {
        RuleFor(x => x.PhoneNumber)
            .NotEmpty().WithMessage("Phone number is required")
            .Matches(new Regex(@"^\+?[1-9]\d{1,14}$")).WithMessage("Phone number must be in a valid international format");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required")
            .MinimumLength(6).WithMessage("Password must be at least 6 characters long")
            .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter")
            .Matches("[0-9]").WithMessage("Password must contain at least one digit")
            .Matches("[!@#$%^&*]").WithMessage("Password must contain at least one special character");

        RuleFor(x => x.FirstName)
            .NotEmpty().WithMessage("First name is required")
            .MaximumLength(50).WithMessage("First name cannot exceed 50 characters");

        RuleFor(x => x.LastName)
            .NotEmpty().WithMessage("Last name is required")
            .MaximumLength(50).WithMessage("Last name cannot exceed 50 characters");

        RuleFor(x => x.DateOfBirth)
            .NotEmpty().WithMessage("Date of birth is required")
            .LessThan(DateTime.Today).WithMessage("Date of birth must be in the past");

        When(x => !string.IsNullOrEmpty(x.Email), () =>
        {
            RuleFor(x => x.Email)
                .EmailAddress().WithMessage("Email must be in a valid format");
        });
    }
}