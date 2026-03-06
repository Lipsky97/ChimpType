using System;
using System.Collections.Generic;

namespace ChimpType.Data;

public partial class User
{
    public Guid Id { get; set; }

    public string Username { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string Name { get; set; } = null!;

    public bool? Active { get; set; }

    public bool? IsAdmin { get; set; }

    public DateTime? CreatedAt { get; set; }

    public string? SessionTokenId { get; set; }

    public DateTime? SessionExpires { get; set; }

    public virtual ICollection<TestsTaken> TestsTakens { get; set; } = new List<TestsTaken>();
}
