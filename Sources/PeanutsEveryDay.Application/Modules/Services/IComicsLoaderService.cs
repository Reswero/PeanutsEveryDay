namespace PeanutsEveryDay.Application.Modules.Services;

public interface IComicsLoaderService
{
    public Task LoadAsync(CancellationToken cancellationToken = default);
}
