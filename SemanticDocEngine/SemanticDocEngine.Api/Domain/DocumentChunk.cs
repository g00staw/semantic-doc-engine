namespace SemanticDocEngine.Api.Features.Documents;

using Pgvector;

public class DocumentChunk
{
    public required Guid Id { get; init; }
    public required Guid DocumentId { get; init; }
    public string Content { get; set; } = string.Empty;
    public Vector? Embedding { get; set; }
}