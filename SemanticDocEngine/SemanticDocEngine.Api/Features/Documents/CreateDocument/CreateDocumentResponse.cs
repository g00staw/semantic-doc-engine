namespace SemanticDocEngine.Api.Features.Documents.CreateDocument;

public sealed record CreateDocumentResponse(Guid Id);

public sealed record ErrorResponse(
    string Code,
    string Message);