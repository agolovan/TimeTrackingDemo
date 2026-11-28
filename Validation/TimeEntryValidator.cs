using System.ComponentModel.DataAnnotations;
using TimeTrackingDemo.Models;

namespace TimeTrackingDemo.Validation;

public sealed class TimeEntryValidator
{
    public IReadOnlyList<ValidationResult> Validate(TimeEntry timeEntry)
    {
        var validationResults = new List<ValidationResult>();
        var validationContext = new ValidationContext(timeEntry);

        System.ComponentModel.DataAnnotations.Validator.TryValidateObject(
            timeEntry,
            validationContext,
            validationResults,
            validateAllProperties: true);

        return validationResults;
    }
}
