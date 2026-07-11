using FluentValidation;

namespace CampingAI.Application.Commands.Camping.DeleteCamping;
public class DeleteCampingCommandValidator : AbstractValidator<DeleteCampingCommand> {
    public DeleteCampingCommandValidator() {
        RuleFor(x => x.CampingId)
            .NotEqual(Guid.Empty).WithMessage("El CampingId no puede estar vacío.");
    }
}
