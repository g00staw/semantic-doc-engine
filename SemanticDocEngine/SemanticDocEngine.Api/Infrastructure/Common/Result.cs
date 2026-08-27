using System.Runtime.InteropServices.JavaScript;

namespace SemanticDocEngine.Api.Infrastructure.Common;

public sealed class Result<T>
{
    private readonly T? _value;
    private readonly Error? _error;
    
    private Result(T? value)
    {
        IsSuccesss = true;
        _value = value;
    }

    private Result(Error error)
    {
        ArgumentNullException.ThrowIfNull(error);
        
        IsSuccesss = false;
        _error = error;
    }
    
    public bool IsSuccesss { get; }
    public bool IsFailure => !IsSuccesss;
    
    public T Value => IsSuccesss
        ? _value!
        : throw new InvalidOperationException("A failed result does not have a value.");
    
    public Error Error => IsFailure
        ? _error!
        : throw new InvalidOperationException("A successful result does not have an error.");
    
    public static Result<T> Success(T value) => new(value);
    public static Result<T> Failure(Error error) => new(error);
    
}