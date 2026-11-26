using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TimeTrackingDemo.Data;

namespace TimeTrackingDemo.Controllers;

[ApiController]
[Route("timeentries")]
public class TimeEntriesController(TimeTrackingDemoContext db) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetTimeEntries()
    {
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
            .ToListAsync();

        return Ok(entries);
    }
}
