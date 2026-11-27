using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;

using TimeTrackingDemo.Data;

namespace TimeTrackingDemo.Controllers;

[ApiController]
[Route("timeentries")]
public class TimeEntriesController(
    TimeTrackingDemoContext db,
    IDistributedCache cache,
    IConfiguration configuration) : ControllerBase
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

        var entries = await db.TimeEntries
            .OrderBy(t => t.WorkDate)
            .Select(t => new
            {
                t.Employee.FirstName,
                t.Employee.LastName,
                Department = t.Employee.Department.Name,
                Location = t.Employee.Department.Location,
                Project = t.Project.Name,
                t.WorkDate,
                t.HoursWorked
            })
            .ToListAsync(cancellationToken);

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