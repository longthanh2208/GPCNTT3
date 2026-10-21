using System;
using System.Collections.Generic;

namespace AssetManagementAPI.Models;

public partial class Asset
{
    public int Id { get; set; }

    public string AssetSn { get; set; } = null!;

    public string AssetName { get; set; } = null!;

    public int? DepartmentLocationId { get; set; }

    public int? EmployeeId { get; set; }

    public int? AssetGroupId { get; set; }

    public string? Description { get; set; }

    public DateOnly? WarrantyDate { get; set; }

    public virtual ICollection<AssetOdometer> AssetOdometers { get; set; } = new List<AssetOdometer>();

    public virtual ICollection<Pmtask> Pmtasks { get; set; } = new List<Pmtask>();
}
