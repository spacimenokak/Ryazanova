using FluentValidation;
using projectTest.Models.DTO;

namespace projectTest.Validators;

public class CreateBookingDtoValidator : AbstractValidator<CreateBookingDto>
{
    public CreateBookingDtoValidator()
    {
        RuleFor(x => x.ClassId)
            .NotEmpty().WithMessage("Class ID is required");
    }
}
