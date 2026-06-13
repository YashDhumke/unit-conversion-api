namespace UnitConversion.Domain.Models;

public class DomainValidationResult
{
    public bool Success { get; init; }
    public string Message { get; init; } = string.Empty;
    public string? Source { get; init; }

    public static DomainValidationResult Failure(string message, string? source = null)
    {
        return new DomainValidationResult
        {
            Success = false,
            Message = message,
            Source = source
        };
    }
}
