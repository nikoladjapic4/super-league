using FluentValidation;
using SuperLeague.DTOs.Team;

namespace SuperLeague.Validators.Team
{
    public class CreateTeamDtoValidator : AbstractValidator<CreateTeamDto>
    {
        public CreateTeamDtoValidator()
        {
            // TeamName validacija
            RuleFor(x => x.TeamName)
                .NotEmpty()
                    .WithMessage("Ime tima je obavezno")
                .MaximumLength(100)
                    .WithMessage("Ime tima ne sme biti duže od 100 karaktera")
                .MinimumLength(2)
                    .WithMessage("Ime tima mora imati najmanje 2 karaktera")
                .Matches(@"^[a-zA-Z0-9\s\-]+$")
                    .WithMessage("Ime tima može sadržati samo slova, brojeve, razmake i crtice");

            // City validacija
            RuleFor(x => x.City)
                .NotEmpty()
                    .WithMessage("Grad je obavezan")
                .MaximumLength(50)
                    .WithMessage("Grad ne sme biti duži od 50 karaktera");

            // Stadium validacija
            RuleFor(x => x.Stadium)
                .NotEmpty()
                    .WithMessage("Stadion je obavezan")
                .MaximumLength(100)
                    .WithMessage("Stadion ne sme biti duži od 100 karaktera");

            // DateOfFoundation validacija
            RuleFor(x => x.DateOfFoundation)
                .NotEmpty()
                    .WithMessage("Datum osnivanja je obavezan")
                .LessThanOrEqualTo(DateTime.UtcNow)
                    .WithMessage("Datum osnivanja ne može biti u budućnosti")
                .GreaterThanOrEqualTo(new DateTime(1800, 1, 1))
                    .WithMessage("Datum osnivanja mora biti posle 1800. godine");
        }
    }
}