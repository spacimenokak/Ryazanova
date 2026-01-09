using FluentValidation;
using projectTest.Models.DTO;

namespace projectTest.Validators;

public class CreateClassDtoValidator : AbstractValidator<CreateClassDto>
{
    public CreateClassDtoValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Name is required")
            .MaximumLength(200).WithMessage("Name must not exceed 200 characters");

        RuleFor(x => x.Description)
            .MaximumLength(1000).WithMessage("Description must not exceed 1000 characters");

        RuleFor(x => x.StartTime)
            .NotEmpty().WithMessage("Start time is required")
            .Must((dto, startTime) => startTime < dto.EndTime)
            .WithMessage("Start time must be before end time");

        RuleFor(x => x.EndTime)
            .NotEmpty().WithMessage("End time is required")
            .Must((dto, endTime) => endTime > dto.StartTime)
            .WithMessage("End time must be after start time");

        RuleFor(x => x.MaxParticipants)
            .GreaterThan(0).WithMessage("Max participants must be greater than 0");

        RuleFor(x => x.Price)
            .GreaterThanOrEqualTo(0).WithMessage("Price must be greater than or equal to 0");

        RuleFor(x => x.InstructorId)
            .NotEmpty().WithMessage("Instructor ID is required");
    }
}
