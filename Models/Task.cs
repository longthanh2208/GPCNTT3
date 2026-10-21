using System;
using System.Collections.Generic;

namespace AssetManagementAPI.Models;

public partial class Task
{
    public int Id { get; set; }

    public string Name { get; set; } = null!;

    public virtual ICollection<Pmtask> Pmtasks { get; set; } = new List<Pmtask>();
}
