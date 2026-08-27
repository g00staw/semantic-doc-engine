namespace SemanticDocEngine.Api.Infrastructure.Common;

public sealed record Error(
    string Code,
    string Description,
    ErrorType Type);

public enum ErrorType
{
    Validation,
    NotFound,
    Conflict,
    Forbidden,
    Unauthorized,
    Failure
}