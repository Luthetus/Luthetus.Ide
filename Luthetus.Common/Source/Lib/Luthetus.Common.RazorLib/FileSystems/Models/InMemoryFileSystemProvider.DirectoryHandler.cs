namespace Luthetus.Common.RazorLib.FileSystems.Models;

public partial class InMemoryFileSystemProvider : IFileSystemProvider
{
    private class InMemoryDirectoryHandler : IDirectoryHandler
    {
        private readonly InMemoryFileSystemProvider _inMemoryFileSystemProvider;
        private readonly IEnvironmentProvider _environmentProvider;

        public InMemoryDirectoryHandler(
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

        public async Task CreateDirectoryAsync(
            string absolutePathString,
            CancellationToken cancellationToken = default)
        {
            try
            {
                await _inMemoryFileSystemProvider._modificationSemaphore.WaitAsync();
                await UnsafeCreateDirectoryAsync(absolutePathString, cancellationToken);
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
                await _inMemoryFileSystemProvider._modificationSemaphore.WaitAsync();

                await UnsafeDeleteAsync(
                    absolutePathString,
                    recursive,
                    cancellationToken);
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
                await _inMemoryFileSystemProvider._modificationSemaphore.WaitAsync();

                await UnsafeCopyAsync(
                    sourceAbsoluteFileString,
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
            await _inMemoryFileSystemProvider._modificationSemaphore.WaitAsync();

            return await UnsafeEnumerateFileSystemEntriesAsync(
                absolutePathString,
                cancellationToken);
        }
        
        public Task<bool> UnsafeExistsAsync(
            string absolutePathString,
            CancellationToken cancellationToken = default)
        {
            return Task.FromResult(_inMemoryFileSystemProvider._files.Any(
                f => f.AbsolutePath.FormattedInput == absolutePathString));
        }

        public Task UnsafeCreateDirectoryAsync(
            string absolutePathString,
            CancellationToken cancellationToken = default)
        {
            var existingFile = _inMemoryFileSystemProvider._files.FirstOrDefault(
                f => f.AbsolutePath.FormattedInput == absolutePathString);

            if (existingFile is not null)
                return Task.CompletedTask;

            var absolutePath = new AbsolutePath(
                absolutePathString,
                true,
                _environmentProvider);

            var outDirectory = new InMemoryFile(
                string.Empty,
                absolutePath,
                DateTime.UtcNow);

            _inMemoryFileSystemProvider._files.Add(outDirectory);

            return Task.CompletedTask;
        }

        public async Task UnsafeDeleteAsync(
            string absolutePathString,
            bool recursive,
            CancellationToken cancellationToken = default)
        {
            if (absolutePathString == _environmentProvider.RootDirectoryAbsolutePath.FormattedInput ||
                absolutePathString == _environmentProvider.HomeDirectoryAbsolutePath.FormattedInput)
            {
                return;
            }

            var indexOfExistingFile = _inMemoryFileSystemProvider._files.FindIndex(
                f => f.AbsolutePath.FormattedInput == absolutePathString);

            if (indexOfExistingFile == -1)
                return;

            var childFileBag = _inMemoryFileSystemProvider._files.Where(imf =>
                imf.AbsolutePath.FormattedInput.StartsWith(absolutePathString));

            foreach (var child in childFileBag)
            {
                await _inMemoryFileSystemProvider._file.UnsafeDeleteAsync(
                    child.AbsolutePath.FormattedInput,
                    cancellationToken);
            }

            _inMemoryFileSystemProvider._files.RemoveAt(indexOfExistingFile);
        }

        public async Task UnsafeCopyAsync(
            string sourceAbsolutePathString,
            string destinationAbsolutePathString,
            CancellationToken cancellationToken = default)
        {
            var indexOfExistingFile = _inMemoryFileSystemProvider._files.FindIndex(
                f => f.AbsolutePath.FormattedInput == sourceAbsolutePathString);

            if (indexOfExistingFile == -1)
                return;

            var childFileBag = _inMemoryFileSystemProvider._files.Where(imf =>
                imf.AbsolutePath.FormattedInput.StartsWith(sourceAbsolutePathString));

            var destinationAbsolutePath = new AbsolutePath(
                destinationAbsolutePathString,
                true,
                _environmentProvider);

            var destinationFile = new InMemoryFile(
                string.Empty,
                destinationAbsolutePath,
                DateTime.UtcNow);

            _inMemoryFileSystemProvider._files.Add(destinationFile);

            foreach (var child in childFileBag)
            {
                var destinationChild = _environmentProvider.JoinPaths(
                    destinationAbsolutePathString,
                    child.AbsolutePath.NameWithExtension);

                await _inMemoryFileSystemProvider._file.UnsafeCopyAsync(
                    child.AbsolutePath.FormattedInput,
                    destinationChild,
                    cancellationToken);
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
            if (sourceAbsolutePathString == _environmentProvider.RootDirectoryAbsolutePath.FormattedInput ||
                sourceAbsolutePathString == _environmentProvider.HomeDirectoryAbsolutePath.FormattedInput)
            {
                return;
            }

            var indexOfExistingFile = _inMemoryFileSystemProvider._files.FindIndex(
                f => f.AbsolutePath.FormattedInput == sourceAbsolutePathString);

            if (indexOfExistingFile == -1)
                return;

            var childFileBag = _inMemoryFileSystemProvider._files.Where(imf =>
                imf.AbsolutePath.FormattedInput.StartsWith(sourceAbsolutePathString));

            var destinationAbsolutePath = new AbsolutePath(
                destinationAbsolutePathString,
                true,
                _environmentProvider);

            var destinationFile = new InMemoryFile(
                string.Empty,
                destinationAbsolutePath,
                DateTime.UtcNow);

            _inMemoryFileSystemProvider._files.Add(destinationFile);

            foreach (var child in childFileBag)
            {
                var destinationChild = _environmentProvider.JoinPaths(
                    destinationAbsolutePathString,
                    child.AbsolutePath.NameWithExtension);

                await _inMemoryFileSystemProvider._file.UnsafeMoveAsync(
                    child.AbsolutePath.FormattedInput,
                    destinationChild,
                    cancellationToken);
            }

            _inMemoryFileSystemProvider._files.RemoveAt(indexOfExistingFile);
        }

        public Task<string[]> UnsafeGetDirectoriesAsync(
            string absolutePathString,
            CancellationToken cancellationToken = default)
        {
            var existingFile = _inMemoryFileSystemProvider._files.FirstOrDefault(
                f => f.AbsolutePath.FormattedInput == absolutePathString);

            if (existingFile is null)
                return Task.FromResult(Array.Empty<string>());

            var childrenFromAllGenerationsBag = _inMemoryFileSystemProvider._files.Where(
                f => f.AbsolutePath.FormattedInput.StartsWith(absolutePathString) &&
                     f.AbsolutePath.FormattedInput != absolutePathString)
                .ToArray();

            var directChildren = childrenFromAllGenerationsBag.Where(
                f =>
                {
                    var withoutParentPrefix = new string(
                        f.AbsolutePath.FormattedInput
                            .Skip(absolutePathString.Length)
                            .ToArray());

                    return withoutParentPrefix.EndsWith("/") &&
                           withoutParentPrefix.Count(x => x == '/') == 1;
                })
                .Select(f => f.AbsolutePath.FormattedInput)
                .ToArray();

            return Task.FromResult(directChildren);
        }

        public Task<string[]> UnsafeGetFilesAsync(
            string absolutePathString,
            CancellationToken cancellationToken = default)
        {
            var existingFile = _inMemoryFileSystemProvider._files.FirstOrDefault(
                f => f.AbsolutePath.FormattedInput == absolutePathString);

            if (existingFile is null)
                return Task.FromResult(Array.Empty<string>());

            var childrenFromAllGenerationsBag = _inMemoryFileSystemProvider._files.Where(
                f => f.AbsolutePath.FormattedInput.StartsWith(absolutePathString) &&
                     f.AbsolutePath.FormattedInput != absolutePathString);

            var directChildrenBag = childrenFromAllGenerationsBag.Where(
                f =>
                {
                    var withoutParentPrefix = new string(
                        f.AbsolutePath.FormattedInput
                            .Skip(absolutePathString.Length)
                            .ToArray());

                    return withoutParentPrefix.Count(x => x == '/') == 0;
                })
                .Select(f => f.AbsolutePath.FormattedInput)
                .ToArray();

            return Task.FromResult(directChildrenBag);
        }

        public async Task<IEnumerable<string>> UnsafeEnumerateFileSystemEntriesAsync(
            string absolutePathString,
            CancellationToken cancellationToken = default)
        {
            var directoryBag = await UnsafeGetDirectoriesAsync(
                absolutePathString,
                cancellationToken);

            var fileBag = await UnsafeGetFilesAsync(absolutePathString, cancellationToken);

            return directoryBag.Union(fileBag);
        }
    }
}