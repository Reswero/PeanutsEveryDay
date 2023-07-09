namespace PeanutsEveryDay.Application.Modules.Services;

/// <summary>
/// Service for loading and saving comics
/// </summary>
public interface IComicsLoaderService
{
    /// <summary>
    /// Starts loading comics
    /// </summary>
    /// <param name="executionDuration">Working duration</param>
    public Task LoadAsync(TimeSpan? executionDuration = null, CancellationToken cancellationToken = default);
}
