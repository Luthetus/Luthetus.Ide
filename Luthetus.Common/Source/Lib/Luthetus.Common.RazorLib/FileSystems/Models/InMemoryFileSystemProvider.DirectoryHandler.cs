using Fluxor;
using Luthetus.Common.RazorLib.ComponentRenderers.Models;
using Luthetus.Common.RazorLib.Notifications.Models;

namespace Luthetus.Common.RazorLib.FileSystems.Models;

public partial class InMemoryFileSystemProvider : IFileSystemProvider
{
    public class InMemoryDirectoryHandler : IDirectoryHandler
    {
        private const bool IS_DIRECTORY_RESPONSE = true;

        private readonly InMemoryFileSystemProvider _inMemoryFileSystemProvider;
        private readonly IEnvironmentProvider _environmentProvider;
        private readonly ILuthetusCommonComponentRenderers _commonComponentRenderers;
        private readonly IDispatcher _dispatcher;

        public InMemoryDirectoryHandler(
            InMemoryFileSystemProvider inMemoryFileSystemProvider,
            IEnvironmentProvider environmentProvider,
            ILuthetusCommonComponentRenderers commonComponentRenderers,
            IDispatcher dispatcher)
        {
            _inMemoryFileSystemProvider = inMemoryFileSystemProvider;
            _environmentProvider = environmentProvider;
            _commonComponentRenderers = commonComponentRenderers;
            _dispatcher = dispatcher;
        }

        public Task<bool> ExistsAsync(
            string absolutePathString,
            CancellationToken cancellationToken = default)
        {
            return UnsafeExistsAsync(absolutePathString, cancellationToken);
        }

        public async Task CreateDirectoryAsync(
            string absolutePathString,
            CancellationToken cancellationToken = default)
        {
            try
            {
                await _inMemoryFileSystemProvider._modificationSemaphore.WaitAsync().ConfigureAwait(false);
                await UnsafeCreateDirectoryAsync(absolutePathString, cancellationToken).ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                NotifyUserOfException(exception);
                throw;
            }
            finally
            {
                _inMemoryFileSystemProvider._modificationSemaphore.Release();
            }
        }

        public async Task DeleteAsync(
            string absolutePathString,
            bool recursive,
            CancellationToken cancellationToken = default)
        {
            try
            {
                await _inMemoryFileSystemProvider._modificationSemaphore.WaitAsync().ConfigureAwait(false);

                await UnsafeDeleteAsync(
                        absolutePathString,
                        recursive,
                        cancellationToken)
                    .ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                NotifyUserOfException(exception);
                throw;
            }
            finally
            {
                _inMemoryFileSystemProvider._modificationSemaphore.Release();
            }
        }

        public async Task CopyAsync(
            string sourceAbsoluteFileString,
            string destinationAbsolutePathString,
            CancellationToken cancellationToken = default)
        {
            try
            {
                await _inMemoryFileSystemProvider._modificationSemaphore.WaitAsync().ConfigureAwait(false);

                await UnsafeCopyAsync(
                        sourceAbsoluteFileString,
                        destinationAbsolutePathString,
                        cancellationToken)
					.ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                NotifyUserOfException(exception);
                throw;
            }
            finally
            {
                _inMemoryFileSystemProvider._modificationSemaphore.Release();
            }
        }
        
        public async Task MoveAsync(
            string sourceAbsolutePathString,
            string destinationAbsolutePathString,
            CancellationToken cancellationToken = default)
        {
            try
            {
                await _inMemoryFileSystemProvider._modificationSemaphore.WaitAsync().ConfigureAwait(false);

                await UnsafeMoveAsync(
                        sourceAbsolutePathString,
                        destinationAbsolutePathString,
                        cancellationToken)
					.ConfigureAwait(false);
            }
            catch (Exception exception)
            {
                NotifyUserOfException(exception);
                throw;
            }
            finally
            {
                _inMemoryFileSystemProvider._modificationSemaphore.Release();
            }
        }

        public Task<string[]> GetDirectoriesAsync(
            string absolutePathString,
            CancellationToken cancellationToken = default)
        {
            return UnsafeGetDirectoriesAsync(absolutePathString, cancellationToken);
        }

        public Task<string[]> GetFilesAsync(
            string absolutePathString,
            CancellationToken cancellationToken = default)
        {
            return UnsafeGetFilesAsync(absolutePathString, cancellationToken);
        }

        public async Task<IEnumerable<string>> EnumerateFileSystemEntriesAsync(
            string absolutePathString,
            CancellationToken cancellationToken = default)
        {
            return await UnsafeEnumerateFileSystemEntriesAsync(
                    absolutePathString,
                    cancellationToken)
				.ConfigureAwait(false);
        }
        
        public Task<bool> UnsafeExistsAsync(
            string absolutePathString,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(_inMemoryFileSystemProvider._files.Any(f =>
                f.AbsolutePath.Value == absolutePathString &&
                f.IsDirectory));
        }

        public Task UnsafeCreateDirectoryAsync(
            string absolutePathString,
            CancellationToken cancellationToken = default)
        {
            var existingFile = _inMemoryFileSystemProvider._files.FirstOrDefault(f =>
                f.AbsolutePath.Value == absolutePathString &&
                f.IsDirectory);

            if (existingFile is not null)
                return Task.CompletedTask;

            var absolutePath = _environmentProvider.AbsolutePathFactory(absolutePathString, true);

            var outDirectory = new InMemoryFile(
                string.Empty,
                absolutePath,
                DateTime.UtcNow,
                true);

            _inMemoryFileSystemProvider._files.Add(outDirectory);

            _environmentProvider.DeletionPermittedRegister(new SimplePath(
                absolutePathString,
                IS_DIRECTORY_RESPONSE));

            return Task.CompletedTask;
        }

        public async Task UnsafeDeleteAsync(
            string absolutePathString,
            bool recursive,
            CancellationToken cancellationToken = default)
        {
            _environmentProvider.AssertDeletionPermitted(absolutePathString, IS_DIRECTORY_RESPONSE);

            var indexOfExistingFile = _inMemoryFileSystemProvider._files.FindIndex(f =>
                f.AbsolutePath.Value == absolutePathString &&
                f.IsDirectory);

            if (indexOfExistingFile == -1)
                return;

            var childFileList = _inMemoryFileSystemProvider._files.Where(imf =>
                    imf.AbsolutePath.Value.StartsWith(absolutePathString) &&
                    imf.AbsolutePath.Value != absolutePathString)
                .ToArray();

            foreach (var child in childFileList)
            {
                if (child.IsDirectory)
                {
                    await _inMemoryFileSystemProvider._directory.UnsafeDeleteAsync(
                            child.AbsolutePath.Value,
                            false,
                            cancellationToken)
						.ConfigureAwait(false);
                }
                else
                {
                    await _inMemoryFileSystemProvider._file.UnsafeDeleteAsync(
                            child.AbsolutePath.Value,
                            cancellationToken)
						.ConfigureAwait(false);
                }
            }

            _inMemoryFileSystemProvider._files.RemoveAt(indexOfExistingFile);
        }

        public async Task UnsafeCopyAsync(
            string sourceAbsolutePathString,
            string destinationAbsolutePathString,
            CancellationToken cancellationToken = default)
        {
            var indexOfExistingFile = _inMemoryFileSystemProvider._files.FindIndex(f =>
                f.AbsolutePath.Value == sourceAbsolutePathString &&
                f.IsDirectory);

            if (indexOfExistingFile == -1)
                return;

            var sourceAbsolutePath = _environmentProvider.AbsolutePathFactory(sourceAbsolutePathString, true);

            var childDirectories = (await GetDirectoriesAsync(sourceAbsolutePathString, cancellationToken).ConfigureAwait(false))
                .Select(x => _environmentProvider.AbsolutePathFactory(x, true))
                .ToArray();

            var childFiles = (await GetFilesAsync(sourceAbsolutePathString, cancellationToken).ConfigureAwait(false))
                .Select(x => _environmentProvider.AbsolutePathFactory(x, false))
                .ToArray();

            var children = childDirectories.Union(childFiles);

            var destinationExists = await UnsafeExistsAsync(destinationAbsolutePathString, cancellationToken).ConfigureAwait(false);

            if (destinationExists)
            {
                _environmentProvider.AssertDeletionPermitted(destinationAbsolutePathString, IS_DIRECTORY_RESPONSE);
            }
            else
            {
                var destinationAbsolutePath = _environmentProvider.AbsolutePathFactory(
                    destinationAbsolutePathString,
                    true);

                var destinationFile = new InMemoryFile(
                    string.Empty,
                    destinationAbsolutePath,
                    DateTime.UtcNow,
                    true);

                _inMemoryFileSystemProvider._files.Add(destinationFile);
            }

            foreach (var child in children)
            {
                var innerDestinationPath = _environmentProvider.JoinPaths(
                    destinationAbsolutePathString,
                    sourceAbsolutePath.NameWithExtension);

                var destinationChild = _environmentProvider.JoinPaths(
                    innerDestinationPath,
                    child.NameWithExtension);

                if (child.IsDirectory)
                {
                    await _inMemoryFileSystemProvider._directory.UnsafeCopyAsync(
                            child.Value,
                            innerDestinationPath,
                            cancellationToken)
						.ConfigureAwait(false);
                }
                else
                {
                    await _inMemoryFileSystemProvider._file.UnsafeCopyAsync(
                            child.Value,
                            destinationChild,
                            cancellationToken)
						.ConfigureAwait(false);
                }
            }
        }

        /// <summary>
        /// destinationAbsolutePathString refers to the newly created dir not the parent dir which will contain it
        /// </summary>
        public async Task UnsafeMoveAsync(
            string sourceAbsolutePathString,
            string destinationAbsolutePathString,
            CancellationToken cancellationToken = default)
        {
            _environmentProvider.AssertDeletionPermitted(sourceAbsolutePathString, IS_DIRECTORY_RESPONSE);

            var indexOfExistingFile = _inMemoryFileSystemProvider._files.FindIndex(f =>
                f.AbsolutePath.Value == sourceAbsolutePathString &&
                f.IsDirectory);

            if (indexOfExistingFile == -1)
                return;

            if (await ExistsAsync(destinationAbsolutePathString).ConfigureAwait(false))
                _environmentProvider.AssertDeletionPermitted(destinationAbsolutePathString, IS_DIRECTORY_RESPONSE);

            await UnsafeCopyAsync(sourceAbsolutePathString, destinationAbsolutePathString, cancellationToken).ConfigureAwait(false);
            await UnsafeDeleteAsync(sourceAbsolutePathString, true, cancellationToken).ConfigureAwait(false);
        }

        public Task<string[]> UnsafeGetDirectoriesAsync(
            string absolutePathString,
            CancellationToken cancellationToken = default)
        {
            var existingFile = _inMemoryFileSystemProvider._files.FirstOrDefault(f =>
                f.AbsolutePath.Value == absolutePathString &&
                f.IsDirectory);

            if (existingFile is null)
                return Task.FromResult(Array.Empty<string>());

            var childrenFromAllGenerationsList = _inMemoryFileSystemProvider._files.Where(
                f => f.AbsolutePath.Value.StartsWith(absolutePathString) &&
                     f.AbsolutePath.Value != absolutePathString)
                .ToArray();

            var directChildren = childrenFromAllGenerationsList.Where(
                f =>
                {
                    var withoutParentPrefix = new string(
                        f.AbsolutePath.Value
                            .Skip(absolutePathString.Length)
                            .ToArray());

                    return withoutParentPrefix.EndsWith("/") &&
                           withoutParentPrefix.Count(x => x == '/') == 1;
                })
                .Select(f => f.AbsolutePath.Value)
                .ToArray();

            return Task.FromResult(directChildren);
        }

        public Task<string[]> UnsafeGetFilesAsync(
            string absolutePathString,
            CancellationToken cancellationToken = default)
        {
            var existingFile = _inMemoryFileSystemProvider._files.FirstOrDefault(f =>
                f.AbsolutePath.Value == absolutePathString &&
                f.IsDirectory);

            if (existingFile is null)
                return Task.FromResult(Array.Empty<string>());

            var childrenFromAllGenerationsList = _inMemoryFileSystemProvider._files.Where(
                f => f.AbsolutePath.Value.StartsWith(absolutePathString) &&
                     f.AbsolutePath.Value != absolutePathString);

            var directChildrenList = childrenFromAllGenerationsList.Where(
                f =>
                {
                    var withoutParentPrefix = new string(
                        f.AbsolutePath.Value
                            .Skip(absolutePathString.Length)
                            .ToArray());

                    return withoutParentPrefix.Count(x => x == '/') == 0;
                })
                .Select(f => f.AbsolutePath.Value)
                .ToArray();

            return Task.FromResult(directChildrenList);
        }

        public async Task<IEnumerable<string>> UnsafeEnumerateFileSystemEntriesAsync(
            string absolutePathString,
            CancellationToken cancellationToken = default)
        {
            var directoryList = await UnsafeGetDirectoriesAsync(
                    absolutePathString,
                    cancellationToken)
				.ConfigureAwait(false);

            var fileList = await UnsafeGetFilesAsync(absolutePathString, cancellationToken).ConfigureAwait(false);

            return directoryList.Union(fileList);
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
                _dispatcher,
                TimeSpan.FromSeconds(10));
        }
    }
}