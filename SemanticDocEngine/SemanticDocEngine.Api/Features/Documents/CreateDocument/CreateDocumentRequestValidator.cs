using FluentValidation;

namespace SemanticDocEngine.Api.Features.Documents.CreateDocument;

public class CreateDocumentRequestValidator : AbstractValidator<CreateDocumentRequest>
{
    public CreateDocumentRequestValidator()
    {
        RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.")
            .MinimumLength(3).WithMessage("Title must be at least 3 characters long.")
            .MaximumLength(200).WithMessage("Title must not exceed 200 characters.");

        RuleFor(x => x.RawContent)
            .NotEmpty().WithMessage("Raw content is required.")
            .MinimumLength(50).WithMessage("Content must be at least 50 characters.")
            .MaximumLength(100_000).WithMessage("Content must not exceed 100,000 characters.");

    }
}