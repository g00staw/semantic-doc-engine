using SemanticDocEngine.Api.Domain;
using SemanticDocEngine.Api.Infrastructure.Common;
using SemanticDocEngine.Api.Infrastructure.Database;

namespace SemanticDocEngine.Api.Features.Documents.CreateDocument;

public sealed class CreateDocumentHandler
{
    private readonly AppDbContext _dbContext;

    public CreateDocumentHandler(AppDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<Guid>> HandleAsync(
        CreateDocumentRequest request,
        CancellationToken cancellationToken)
    {
        var document = new Document
        {
            Id = Guid.NewGuid(),
            Title = request.Title,
            RawContent = request.RawContent,
            Status = DocumentStatus.Pending,
            CreatedAt = DateTime.UtcNow
        };

        _dbContext.Documents.Add(document);

        await _dbContext.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(document.Id);
    }
}