using Fluxor;
using Luthetus.Common.RazorLib.ComponentRenderers.Models;
using Luthetus.Common.RazorLib.Notifications.Models;
using System.Text;

namespace Luthetus.Common.RazorLib.FileSystems.Models;

public partial class InMemoryFileSystemProvider : IFileSystemProvider
{
    public class InMemoryFileHandler : IFileHandler
    {
        private const bool IS_DIRECTORY_RESPONSE = false;

        private readonly InMemoryFileSystemProvider _inMemoryFileSystemProvider;
        private readonly IEnvironmentProvider _environmentProvider;
        private readonly ILuthetusCommonComponentRenderers _commonComponentRenderers;
        private readonly IDispatcher _dispatcher;

        public InMemoryFileHandler(
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

        public async Task DeleteAsync(
            string absolutePathString,
            CancellationToken cancellationToken = default)
        {
            try
            {
                await _inMemoryFileSystemProvider._modificationSemaphore.WaitAsync().ConfigureAwait(false);
                await UnsafeDeleteAsync(absolutePathString, cancellationToken).ConfigureAwait(false);
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
            string sourceAbsolutePathString,
            string destinationAbsolutePathString,
            CancellationToken cancellationToken = default)
        {
            try
            {
                await _inMemoryFileSystemProvider._modificationSemaphore.WaitAsync().ConfigureAwait(false);
                
                await UnsafeCopyAsync(
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

        public async Task<DateTime> GetLastWriteTimeAsync(
            string absolutePathString,
            CancellationToken cancellationToken = default)
        {
            return await UnsafeGetLastWriteTimeAsync(absolutePathString, cancellationToken).ConfigureAwait(false);
        }

        public async Task<string> ReadAllTextAsync(
            string absolutePathString,
            CancellationToken cancellationToken = default)
        {
            return await UnsafeReadAllTextAsync(absolutePathString, cancellationToken).ConfigureAwait(false);
        }

        public async Task WriteAllTextAsync(
            string absolutePathString,
            string contents,
            CancellationToken cancellationToken = default)
        {
            try
            {
                await _inMemoryFileSystemProvider._modificationSemaphore.WaitAsync().ConfigureAwait(false);
                
                await UnsafeWriteAllTextAsync(
                        absolutePathString,
                        contents,
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
        
        public Task<bool> UnsafeExistsAsync(
            string absolutePathString,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(_inMemoryFileSystemProvider._files.Any(imf =>
                imf.AbsolutePath.Value == absolutePathString &&
                !imf.IsDirectory));
        }

        public Task UnsafeDeleteAsync(
            string absolutePathString,
            CancellationToken cancellationToken = default)
        {
            _environmentProvider.AssertDeletionPermitted(absolutePathString, IS_DIRECTORY_RESPONSE);

            var indexOfExistingFile = _inMemoryFileSystemProvider._files.FindIndex(f =>
                f.AbsolutePath.Value == absolutePathString &&
                !f.IsDirectory);

            if (indexOfExistingFile == -1)
                return Task.CompletedTask;

            _inMemoryFileSystemProvider._files.RemoveAt(indexOfExistingFile);

            return Task.CompletedTask;
        }

        public async Task UnsafeCopyAsync(
            string sourceAbsolutePathString,
            string destinationAbsolutePathString,
            CancellationToken cancellationToken = default)
        {
            // Source
            {
                var indexOfSource = _inMemoryFileSystemProvider._files.FindIndex(f =>
                    f.AbsolutePath.Value == sourceAbsolutePathString &&
                    !f.IsDirectory);

                if (indexOfSource == -1)
                    throw new ApplicationException($"Source file: {sourceAbsolutePathString} was not found.");
            }

            // Destination
            { 
                var indexOfDestination = _inMemoryFileSystemProvider._files.FindIndex(f =>
                    f.AbsolutePath.Value == destinationAbsolutePathString &&
                    !f.IsDirectory);

                if (indexOfDestination != -1)
                    _environmentProvider.AssertDeletionPermitted(destinationAbsolutePathString, IS_DIRECTORY_RESPONSE);
            }

            var contents = await UnsafeReadAllTextAsync(
                    sourceAbsolutePathString,
                    cancellationToken)
				.ConfigureAwait(false);

            await UnsafeWriteAllTextAsync(
                    destinationAbsolutePathString,
                    contents,
                    cancellationToken)
				.ConfigureAwait(false);
        }

        public async Task UnsafeMoveAsync(
            string sourceAbsolutePathString,
            string destinationAbsolutePathString,
            CancellationToken cancellationToken = default)
        {
            _environmentProvider.AssertDeletionPermitted(sourceAbsolutePathString, IS_DIRECTORY_RESPONSE);

            if (await ExistsAsync(destinationAbsolutePathString).ConfigureAwait(false))
                _environmentProvider.AssertDeletionPermitted(destinationAbsolutePathString, IS_DIRECTORY_RESPONSE);

            await UnsafeCopyAsync(
                    sourceAbsolutePathString,
                    destinationAbsolutePathString,
                    cancellationToken)
                .ConfigureAwait(false);

            await UnsafeDeleteAsync(sourceAbsolutePathString, cancellationToken).ConfigureAwait(false);
        }

        public Task<DateTime> UnsafeGetLastWriteTimeAsync(
            string absolutePathString,
            CancellationToken cancellationToken = default)
        {
            var existingFile = _inMemoryFileSystemProvider._files.FirstOrDefault(f =>
                f.AbsolutePath.Value == absolutePathString &&
                !f.IsDirectory);

            if (existingFile is null)
                return Task.FromResult(default(DateTime));

            return Task.FromResult(existingFile.LastModifiedDateTime);
        }

        public Task<string> UnsafeReadAllTextAsync(
            string absolutePathString,
            CancellationToken cancellationToken = default)
        {
            var existingFile = _inMemoryFileSystemProvider._files.FirstOrDefault(f =>
                f.AbsolutePath.Value == absolutePathString &&
                !f.IsDirectory);

            if (existingFile is null)
                return Task.FromResult(string.Empty);

            return Task.FromResult(existingFile.Data);
        }

        public Task UnsafeWriteAllTextAsync(
            string absolutePathString,
            string contents,
            CancellationToken cancellationToken = default)
        {
            var indexOfExistingFile = _inMemoryFileSystemProvider._files.FindIndex(f =>
                f.AbsolutePath.Value == absolutePathString &&
                !f.IsDirectory);

            if (indexOfExistingFile != -1)
            {
                _environmentProvider.AssertDeletionPermitted(absolutePathString, IS_DIRECTORY_RESPONSE);

                var existingFile = _inMemoryFileSystemProvider._files[indexOfExistingFile];

                _inMemoryFileSystemProvider._files[indexOfExistingFile] = existingFile with
                {
                    Data = contents
                };

                return Task.CompletedTask;
            }

            // Ensure Parent Directories Exist
            {
                var parentDirectoryList = absolutePathString
                    .Split("/")
                    // The root directory splits into string.Empty
                    .Skip(1)
                    // Skip the file being written to itself
                    .SkipLast(1)
                    .ToArray();

                var directoryPathBuilder = new StringBuilder("/");

                for (int i = 0; i < parentDirectoryList.Length; i++)
                {
                    directoryPathBuilder.Append(parentDirectoryList[i]);
                    directoryPathBuilder.Append("/");

                    _inMemoryFileSystemProvider._directory.UnsafeCreateDirectoryAsync(
                        directoryPathBuilder.ToString());
                }
            }

            var absolutePath = _environmentProvider.AbsolutePathFactory(
                absolutePathString,
                false);

            var outFile = new InMemoryFile(
                contents,
                absolutePath,
                DateTime.UtcNow,
                false);

            _inMemoryFileSystemProvider._files.Add(outFile);

            _environmentProvider.DeletionPermittedRegister(
                new SimplePath(absolutePathString, IS_DIRECTORY_RESPONSE));

            return Task.CompletedTask;
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