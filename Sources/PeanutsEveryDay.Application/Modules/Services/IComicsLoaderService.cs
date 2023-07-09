namespace PeanutsEveryDay.Application.Modules.Services;

/// <summary>
/// Service for loading and saving comics
/// </summary>
public interface IComicsLoaderService
{
    /// <summary>
    /// Starts loading comics
    /// </summary>
    public Task LoadAsync(CancellationToken cancellationToken = default);
}
