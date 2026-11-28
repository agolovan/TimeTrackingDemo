using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;

using TimeTrackingDemo.Data;
using TimeTrackingDemo.Validation;

namespace TimeTrackingDemo.Controllers;

[ApiController]
[Route("timeentries")]
public class TimeEntriesController(
    TimeTrackingDemoContext db,
    IDistributedCache cache,
    IConfiguration configuration,
    TimeEntryValidator validator) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetTimeEntries(CancellationToken cancellationToken)
    {
        var cacheKey = configuration["CacheKeys:TimeEntries"]
            ?? throw new InvalidOperationException(
                "Cache key 'CacheKeys:TimeEntries' was not found.");

        var cachedJson = await cache.GetStringAsync(cacheKey, cancellationToken);

        if (cachedJson is not null)
        {
            return Content(cachedJson, "application/json");
        }

        var timeEntries = await db.TimeEntries
            .AsNoTracking()
            .Include(t => t.Employee)
                .ThenInclude(e => e.Department)
            .Include(t => t.Project)
            .OrderBy(t => t.WorkDate)
            .ToListAsync(cancellationToken);

        foreach (var timeEntry in timeEntries)
        {
            var validationErrors = validator.Validate(timeEntry);
            if (validationErrors.Count > 0)
            {
                return BadRequest(new
                {
                    message = $"Time entry id = {timeEntry.Id} failed validation.",
                    errors = validationErrors
                        .Select(error => error.ErrorMessage)
                        .ToArray()
                });
            }
        }

        var entries = timeEntries.Select(t => new
        {
            t.Employee.FirstName,
            t.Employee.LastName,
            Department = t.Employee.Department.Name,
            Location = t.Employee.Department.Location,
            Project = t.Project.Name,
            t.WorkDate,
            t.HoursWorked
        });

        var json = JsonSerializer.Serialize(entries, JsonSerializerOptions.Web);

        await cache.SetStringAsync(
            cacheKey,
            json,
            new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(30)
            },
            cancellationToken);

        return Content(json, "application/json");
    }
}