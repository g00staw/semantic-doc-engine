using SemanticDocEngine.Api.Infrastructure.Common;

namespace SemanticDocEngine.Api.Features.Documents.CreateDocument;

public static class CreateDocumentEndpoint
{
    public static IEndpointRouteBuilder MapCreateDocumentEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/documents", HandleAsync)
            .WithName("CreateDocument")
            .AddEndpointFilter<ValidationFilter<CreateDocumentRequest>>()
            .Produces<CreateDocumentResponse>(StatusCodes.Status201Created)
            .Produces(StatusCodes.Status400BadRequest);
        
        return app; 
    }

    private static async Task<IResult> HandleAsync(
        CreateDocumentRequest request,
        CreateDocumentHandler handler,
        CancellationToken cancellationToken)
    {
        var result = await handler.HandleAsync(
            request, 
            cancellationToken);
        
        if (result.IsFailure)
        {
            return MapError(result.Error);
        }
        
        var response = new CreateDocumentResponse(result.Value);
        return Results.Created($"/api/documents/{response.Id}", response);
    }
    
    private static IResult MapError(Error error)
    {
        var response = new ErrorResponse(
            error.Code, 
            error.Description);
            
        return error.Type switch
        {
            ErrorType.Validation => Results.BadRequest(response),
            ErrorType.NotFound => Results.NotFound(response),
            ErrorType.Conflict => Results.Conflict(response),
            _ => Results.Problem(
                statusCode: StatusCodes.Status500InternalServerError,
                title: error.Code,
                detail: error.Description)
        };
    }

}