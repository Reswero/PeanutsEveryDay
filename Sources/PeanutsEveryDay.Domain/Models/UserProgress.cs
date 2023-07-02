﻿namespace PeanutsEveryDay.Domain.Models;

public class UserProgress
{
    public required long UserId { get; init; }
    public required int TotalComicsWatched { get; init; }
}
