using System;
using System.Collections.Generic;

namespace AssetManagementAPI.Models;

public partial class AssetOdometer
{
    public int Id { get; set; }

    public int? AssetId { get; set; }

    public DateOnly ReadDate { get; set; }

    public decimal OdometerAmount { get; set; }

    public virtual Asset? Asset { get; set; }
}
