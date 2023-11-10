using System.Text;

namespace Luthetus.Common.RazorLib.FileSystems.Models;

public partial class InMemoryFileSystemProvider : IFileSystemProvider
{
    private class InMemoryFileHandler : IFileHandler
    {
        private readonly InMemoryFileSystemProvider _inMemoryFileSystemProvider;
        private readonly IEnvironmentProvider _environmentProvider;

        public InMemoryFileHandler(
            InMemoryFileSystemProvider inMemoryFileSystemProvider,
            IEnvironmentProvider environmentProvider)
        {
            _inMemoryFileSystemProvider = inMemoryFileSystemProvider;
            _environmentProvider = environmentProvider;
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
                await _inMemoryFileSystemProvider._modificationSemaphore.WaitAsync();
                await UnsafeDeleteAsync(absolutePathString, cancellationToken);
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
                await _inMemoryFileSystemProvider._modificationSemaphore.WaitAsync();
                
                await UnsafeCopyAsync(
                    sourceAbsolutePathString,
                    destinationAbsolutePathString,
                    cancellationToken);
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
                await _inMemoryFileSystemProvider._modificationSemaphore.WaitAsync();
                
                await UnsafeMoveAsync(
                    sourceAbsolutePathString,
                    destinationAbsolutePathString,
                    cancellationToken);
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
            return await UnsafeGetLastWriteTimeAsync(absolutePathString, cancellationToken);
        }

        public async Task<string> ReadAllTextAsync(
            string absolutePathString,
            CancellationToken cancellationToken = default)
        {
            return await UnsafeReadAllTextAsync(absolutePathString, cancellationToken);
        }

        public async Task WriteAllTextAsync(
            string absolutePathString,
            string contents,
            CancellationToken cancellationToken = default)
        {
            try
            {
                await _inMemoryFileSystemProvider._modificationSemaphore.WaitAsync();
                
                await UnsafeWriteAllTextAsync(
                    absolutePathString,
                    contents,
                    cancellationToken);
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
            return Task.FromResult(_inMemoryFileSystemProvider._files.Any(
                imf => imf.AbsolutePath.Value == absolutePathString));
        }

        public Task UnsafeDeleteAsync(
            string absolutePathString,
            CancellationToken cancellationToken = default)
        {
            var indexOfExistingFile = _inMemoryFileSystemProvider._files.FindIndex(
                f => f.AbsolutePath.Value == absolutePathString);

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
                var indexOfSource = _inMemoryFileSystemProvider._files.FindIndex(
                    f => f.AbsolutePath.Value == sourceAbsolutePathString);

                if (indexOfSource == -1)
                    throw new ApplicationException($"Source file: {sourceAbsolutePathString} was not found.");
            }

            // Destination
            { 
                var indexOfDestination = _inMemoryFileSystemProvider._files.FindIndex(
                    f => f.AbsolutePath.Value == destinationAbsolutePathString);

                if (indexOfDestination != -1)
                    throw new ApplicationException($"A file already exists with the path: {sourceAbsolutePathString}.");
            }

            var contents = await UnsafeReadAllTextAsync(
                sourceAbsolutePathString,
                cancellationToken);

            await UnsafeWriteAllTextAsync(
                destinationAbsolutePathString,
                contents,
                cancellationToken);
        }

        public async Task UnsafeMoveAsync(
            string sourceAbsolutePathString,
            string destinationAbsolutePathString,
            CancellationToken cancellationToken = default)
        {
            await UnsafeCopyAsync(
                sourceAbsolutePathString,
                destinationAbsolutePathString,
                cancellationToken);

            await UnsafeDeleteAsync(sourceAbsolutePathString, cancellationToken);
        }

        public Task<DateTime> UnsafeGetLastWriteTimeAsync(
            string absolutePathString,
            CancellationToken cancellationToken = default)
        {
            var existingFile = _inMemoryFileSystemProvider._files.FirstOrDefault(
                f => f.AbsolutePath.Value == absolutePathString);

            if (existingFile is null)
                return Task.FromResult(default(DateTime));

            return Task.FromResult(existingFile.LastModifiedDateTime);
        }

        public Task<string> UnsafeReadAllTextAsync(
            string absolutePathString,
            CancellationToken cancellationToken = default)
        {
            var existingFile = _inMemoryFileSystemProvider._files.FirstOrDefault(
                f => f.AbsolutePath.Value == absolutePathString);

            if (existingFile is null)
                return Task.FromResult(string.Empty);

            return Task.FromResult(existingFile.Data);
        }

        public Task UnsafeWriteAllTextAsync(
            string absolutePathString,
            string contents,
            CancellationToken cancellationToken = default)
        {
            var indexOfExistingFile = _inMemoryFileSystemProvider._files.FindIndex(
                f => f.AbsolutePath.Value == absolutePathString);

            if (indexOfExistingFile != -1)
            {
                var existingFile = _inMemoryFileSystemProvider._files[indexOfExistingFile];

                _inMemoryFileSystemProvider._files[indexOfExistingFile] = existingFile with
                {
                    Data = contents
                };

                return Task.CompletedTask;
            }

            // Ensure Parent Directories Exist
            {
                var parentDirectoryBag = absolutePathString
                    .Split("/")
                    // The root directory splits into string.Empty
                    .Skip(1)
                    // Skip the file being written to itself
                    .SkipLast(1)
                    .ToArray();

                var directoryPathBuilder = new StringBuilder("/");

                for (int i = 0; i < parentDirectoryBag.Length; i++)
                {
                    directoryPathBuilder.Append(parentDirectoryBag[i]);
                    directoryPathBuilder.Append("/");

                    _inMemoryFileSystemProvider._directory.UnsafeCreateDirectoryAsync(
                        directoryPathBuilder.ToString());
                }
            }

            var absolutePath = new AbsolutePath(
                absolutePathString,
                false,
                _environmentProvider);

            var outFile = new InMemoryFile(
                contents,
                absolutePath,
                DateTime.UtcNow);

            _inMemoryFileSystemProvider._files.Add(outFile);

            return Task.CompletedTask;
        }
    }
}