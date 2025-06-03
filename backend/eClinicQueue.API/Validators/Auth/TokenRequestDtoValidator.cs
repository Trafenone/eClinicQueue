using eClinicQueue.API.Models.Dtos.Auth;
using FluentValidation;

namespace eClinicQueue.API.Validators.Auth;

public class TokenRequestDtoValidator : AbstractValidator<TokenRequestDto>
{
    public TokenRequestDtoValidator()
    {
        RuleFor(x => x.Token)
            .NotEmpty().WithMessage("Token is required");

        RuleFor(x => x.RefreshToken)
            .NotEmpty().WithMessage("Refresh token is required");
    }
}