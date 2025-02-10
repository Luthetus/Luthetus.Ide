using Fluxor;
using Luthetus.Common.RazorLib.ComponentRenderers.Models;
using Luthetus.Common.RazorLib.Notifications.Models;

namespace Luthetus.Common.RazorLib.FileSystems.Models;

public class LocalFileHandler : IFileHandler
{
    private const bool IS_DIRECTORY_RESPONSE = true;

    private readonly IEnvironmentProvider _environmentProvider;
    private readonly ICommonComponentRenderers _commonComponentRenderers;
    private readonly INotificationService _notificationService;

    public LocalFileHandler(
        IEnvironmentProvider environmentProvider,
        ICommonComponentRenderers commonComponentRenderers,
        INotificationService notificationService)
    {
        _environmentProvider = environmentProvider;
        _commonComponentRenderers = commonComponentRenderers;
        _notificationService = notificationService;
    }

    public Task<bool> ExistsAsync(
        string absolutePathString,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(
            File.Exists(absolutePathString));
    }

    public Task DeleteAsync(
        string absolutePathString,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _environmentProvider.AssertDeletionPermitted(absolutePathString, IS_DIRECTORY_RESPONSE);
            File.Delete(absolutePathString);
        }
        catch (Exception exception)
        {
            NotifyUserOfException(exception);
            throw;
        }

        return Task.CompletedTask;
    }

    public Task CopyAsync(
        string sourceAbsolutePathString,
        string destinationAbsolutePathString,
        CancellationToken cancellationToken = default)
    {
        File.Copy(
            sourceAbsolutePathString,
            destinationAbsolutePathString);

        _environmentProvider.DeletionPermittedRegister(
            new SimplePath(destinationAbsolutePathString, IS_DIRECTORY_RESPONSE));

        return Task.CompletedTask;
    }

    public async Task MoveAsync(
        string sourceAbsolutePathString,
        string destinationAbsolutePathString,
        CancellationToken cancellationToken = default)
    {
        try
        {
            _environmentProvider.AssertDeletionPermitted(sourceAbsolutePathString, IS_DIRECTORY_RESPONSE);

            if (await ExistsAsync(destinationAbsolutePathString).ConfigureAwait(false))
                _environmentProvider.AssertDeletionPermitted(destinationAbsolutePathString, IS_DIRECTORY_RESPONSE);

            File.Move(
                sourceAbsolutePathString,
                destinationAbsolutePathString);

            _environmentProvider.DeletionPermittedRegister(
                new SimplePath(destinationAbsolutePathString, IS_DIRECTORY_RESPONSE));
        }
        catch (Exception exception)
        {
            NotifyUserOfException(exception);
            throw;
        }
    }

    public Task<DateTime> GetLastWriteTimeAsync(
        string absolutePathString,
        CancellationToken cancellationToken = default)
    {
        return Task.FromResult(
            File.GetLastWriteTime(
                absolutePathString));
    }

    public async Task<string> ReadAllTextAsync(
        string absolutePathString,
        CancellationToken cancellationToken = default)
    {
        return await File.ReadAllTextAsync(
                absolutePathString,
                cancellationToken)
			.ConfigureAwait(false);
    }

    public async Task WriteAllTextAsync(
        string absolutePathString,
        string contents,
        CancellationToken cancellationToken = default)
    {
        try
        {
            if (await ExistsAsync(absolutePathString, cancellationToken).ConfigureAwait(false))
                _environmentProvider.AssertDeletionPermitted(absolutePathString, IS_DIRECTORY_RESPONSE);

            await File.WriteAllTextAsync(
                    absolutePathString,
                    contents,
                    cancellationToken)
				.ConfigureAwait(false);

            _environmentProvider.DeletionPermittedRegister(
                new SimplePath(absolutePathString, IS_DIRECTORY_RESPONSE));
        }
        catch (Exception exception)
        {
            NotifyUserOfException(exception);
            throw;
        }
    }

    private void NotifyUserOfException(Exception exception)
    {
        var title = "FILESYSTEM ERROR";

        if (exception.Message.StartsWith(PermittanceChecker.ERROR_PREFIX))
            title = PermittanceChecker.ERROR_PREFIX;

        NotificationHelper.DispatchError(
            title,
            exception.ToString(),
            _commonComponentRenderers,
            _notificationService,
            TimeSpan.FromSeconds(10));
    }
}