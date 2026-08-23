namespace SemanticDocEngine.Api.Features.Documents;

public class Document
{
    public required Guid Id { get; init; }
    public required string Title { get; set; }
    public required DateTime CreatedAt { get; init; }
    public DocumentStatus Status { get; set; }
}