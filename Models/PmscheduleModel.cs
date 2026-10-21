using System;
using System.Collections.Generic;

namespace AssetManagementAPI.Models;

public partial class PmscheduleModel
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public int? PmscheduleTypeId { get; set; }

    public virtual PmscheduleType? PmscheduleType { get; set; }
}
