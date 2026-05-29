using System.ComponentModel.DataAnnotations;

namespace QuestionService.Validators;

public class TagListAttribute(int min, int max) : ValidationAttribute
{
    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        if (value is not List<string> tags)
            return new ValidationResult($"{validationContext.DisplayName} is required.");

        if (tags.Count < min || tags.Count > max)
            return new ValidationResult($"{validationContext.DisplayName} must have between {min} and {max} tags.");

        return ValidationResult.Success;
    }
}