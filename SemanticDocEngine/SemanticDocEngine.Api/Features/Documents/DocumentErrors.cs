using SemanticDocEngine.Api.Infrastructure.Common;

namespace SemanticDocEngine.Api.Features.Documents;

public static class DocumentErrors
{
    public static Error NotFound(Guid documentId) => new(
        Code: "Documents.NotFound",
        Description: $"Document with ID '{documentId}' was not found.",
        Type: ErrorType.NotFound);

    public static Error Validation(string description) => new(
        Code: "Documents.Validation",
        Description: description,
        Type: ErrorType.Validation);
}