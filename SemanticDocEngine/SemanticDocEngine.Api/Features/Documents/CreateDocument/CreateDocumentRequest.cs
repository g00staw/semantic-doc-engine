namespace SemanticDocEngine.Api.Features.Documents.CreateDocument;

public record CreateDocumentRequest 
{
    public required string Title { get; init; }
    public required string RawContent { get; init; }
}