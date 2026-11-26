using System;
using System.Collections.Generic;

namespace TimeTrackingDemo.Models;

public partial class TimeEntry
{
    public int Id { get; set; }

    public int EmployeeId { get; set; }

    public int ProjectId { get; set; }

    public DateOnly WorkDate { get; set; }

    public decimal HoursWorked { get; set; }

    public string? Description { get; set; }

    public virtual Employee Employee { get; set; } = null!;

    public virtual Project Project { get; set; } = null!;
}
