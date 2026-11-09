using System.ComponentModel.DataAnnotations;

namespace SmartHome.REST.Models;

public sealed class PaginationQuery : IValidatableObject
{
    [Range(1, int.MaxValue)]
    public int? Page { get; init; }

    [Range(1, 100)]
    public int? Amount { get; init; }

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (Page.HasValue != Amount.HasValue)
        {
            yield return new ValidationResult("Specify both page and amount.",
                [nameof(Page), nameof(Amount)]);
        }
        else if (Page is int page && Amount is int amount && (page - 1L) * amount > int.MaxValue)
        {
            yield return new ValidationResult("The requested page offset is too large.", [nameof(Page)]);
        }
    }
}
