using System;
using System.Collections.Generic;

namespace AssetManagementAPI.Models;

public partial class Pmtask
{
    public int Id { get; set; }

    public int? AssetId { get; set; }

    public int? TaskId { get; set; }

    public int? PmscheduleTypeId { get; set; }

    public DateOnly? ScheduleDate { get; set; }

    public decimal? ScheduleKilometer { get; set; }

    public bool? TaskDone { get; set; }

    public virtual Asset? Asset { get; set; }

    public virtual PmscheduleType? PmscheduleType { get; set; }

    public virtual Task? Task { get; set; }
}
