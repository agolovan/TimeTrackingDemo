using System;
using System.Collections.Generic;

namespace TimeTrackingDemo.Models;

public partial class Employee
{
    public int Id { get; set; }

    public string FirstName { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string? Email { get; set; }

    public int DepartmentId { get; set; }

    public DateOnly HireDate { get; set; }

    public virtual Department Department { get; set; } = null!;

    public virtual ICollection<TimeEntry> TimeEntries { get; set; } = new List<TimeEntry>();
}
