using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TimeTrackingDemo.Data;

namespace TimeTrackingDemo.Controllers;

[ApiController]
[Route("departments")]
public class DepartmentsController(TimeTrackingDemoContext db) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetDepartments()
    {
        var departments = await db.Departments
            .OrderBy(d => d.Name)
            .Select(d => new { d.Id, d.Name, d.Description, d.Location })
            .ToListAsync();

        return Ok(departments);
    }
}

