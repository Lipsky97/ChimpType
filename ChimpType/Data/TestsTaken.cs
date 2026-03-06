using System;
using System.Collections.Generic;

namespace ChimpType.Data;

/// <summary>
/// List of all typing tests taken
/// </summary>
public partial class TestsTaken
{
    public Guid Id { get; set; }

    public DateTime? TakenOn { get; set; }

    public Guid? UserId { get; set; }

    public int? Wpm { get; set; }

    public decimal? Accuracy { get; set; }

    public string? TestType { get; set; }

    public int? TotalTime { get; set; }

    public int? CorrectCharacters { get; set; }

    public int? MissedCharacters { get; set; }

    public int? ExtraCharacters { get; set; }

    public int? WrongCharacters { get; set; }

    public int? MistakesNumber { get; set; }

    public virtual User? User { get; set; }
}
