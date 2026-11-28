using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace TimeTrackingDemo.Models;

public partial class TimeEntry
{
    public int Id { get; set; }

    [Range(1, int.MaxValue)]
    public int EmployeeId { get; set; }

    [Range(1, int.MaxValue)]
    public int ProjectId { get; set; }

    public DateOnly WorkDate { get; set; }

    [Range(typeof(decimal), "0.01", "5.00")]
    public decimal HoursWorked { get; set; }

    [StringLength(500)]
    public string? Description { get; set; }

    public virtual Employee Employee { get; set; } = null!;

    public virtual Project Project { get; set; } = null!;
}
