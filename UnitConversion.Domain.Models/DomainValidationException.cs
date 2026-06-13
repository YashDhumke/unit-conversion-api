namespace UnitConversion.Domain.Models;

public class DomainValidationException : Exception
{
    public DomainValidationException(IReadOnlyCollection<DomainValidationResult> validationResults)
        : base(validationResults.FirstOrDefault()?.Message ?? "One or more validation errors occurred.")
    {
        ValidationResults = validationResults;
    }

    public IReadOnlyCollection<DomainValidationResult> ValidationResults { get; }
}
