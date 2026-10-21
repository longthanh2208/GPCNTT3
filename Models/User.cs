using System;
using System.Collections.Generic;

namespace AssetManagementAPI.Models;

public partial class User
{
    public string Username { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string? FullName { get; set; }
}
