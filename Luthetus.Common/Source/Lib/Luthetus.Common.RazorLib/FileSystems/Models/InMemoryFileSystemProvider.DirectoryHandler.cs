namespace Luthetus.Common.RazorLib.FileSystems.Models;

public partial class InMemoryFileSystemProvider : IFileSystemProvider
{
    public class InMemoryDirectoryHandler : IDirectoryHandler
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

            var absolutePath = new AbsolutePath(
                absolutePathString,
                true,
                _environmentProvider);

            var outDirectory = new InMemoryFile(
                string.Empty,
                absolutePath,
                DateTime.UtcNow,
                true);

            _inMemoryFileSystemProvider._files.Add(outDirectory);

            return Task.CompletedTask;
        }

        public async Task UnsafeDeleteAsync(
            string absolutePathString,
            bool recursive,
            CancellationToken cancellationToken = default)
        {
            if (absolutePathString == _environmentProvider.RootDirectoryAbsolutePath.Value ||
                absolutePathString == _environmentProvider.HomeDirectoryAbsolutePath.Value)
            {
                return;
            }

            var indexOfExistingFile = _inMemoryFileSystemProvider._files.FindIndex(f =>
                f.AbsolutePath.Value == absolutePathString &&
                f.IsDirectory);

            if (indexOfExistingFile == -1)
                return;

            var childFileBag = _inMemoryFileSystemProvider._files.Where(imf =>
                    imf.AbsolutePath.Value.StartsWith(absolutePathString) &&
                    imf.AbsolutePath.Value != absolutePathString)
                .ToArray();

            foreach (var child in childFileBag)
            {
                if (child.IsDirectory)
                {
                    await _inMemoryFileSystemProvider._directory.UnsafeDeleteAsync(
                        child.AbsolutePath.Value,
                        false,
                        cancellationToken);
                }
                else
                {
                    await _inMemoryFileSystemProvider._file.UnsafeDeleteAsync(
                        child.AbsolutePath.Value,
                        cancellationToken);
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

            var sourceAbsolutePath = new AbsolutePath(
                sourceAbsolutePathString,
                true,
                _environmentProvider);

            var childDirectories = (await GetDirectoriesAsync(sourceAbsolutePathString, cancellationToken))
                .Select(x => new AbsolutePath(x, true, _environmentProvider))
                .ToArray();

            var childFiles = (await GetFilesAsync(sourceAbsolutePathString, cancellationToken))
                .Select(x => new AbsolutePath(x, false, _environmentProvider))
                .ToArray();

            var children = childDirectories.Union(childFiles);

            var filePathStrings = children.Select(x => x.Value).ToArray();

            var destinationExists = await UnsafeExistsAsync(destinationAbsolutePathString, cancellationToken);

            if (!destinationExists)
            {
                var destinationAbsolutePath = new AbsolutePath(
                destinationAbsolutePathString,
                true,
                _environmentProvider);

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
                        cancellationToken);
                }
                else
                {
                    await _inMemoryFileSystemProvider._file.UnsafeCopyAsync(
                        child.Value,
                        destinationChild,
                        cancellationToken);
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
            if (sourceAbsolutePathString == _environmentProvider.RootDirectoryAbsolutePath.Value ||
                sourceAbsolutePathString == _environmentProvider.HomeDirectoryAbsolutePath.Value)
            {
                return;
            }

            var indexOfExistingFile = _inMemoryFileSystemProvider._files.FindIndex(f =>
                f.AbsolutePath.Value == sourceAbsolutePathString &&
                f.IsDirectory);

            if (indexOfExistingFile == -1)
                return;

            var childFileBag = _inMemoryFileSystemProvider._files.Where(imf =>
                imf.AbsolutePath.Value.StartsWith(sourceAbsolutePathString));

            var destinationAbsolutePath = new AbsolutePath(
                destinationAbsolutePathString,
                true,
                _environmentProvider);

            var destinationFile = new InMemoryFile(
                string.Empty,
                destinationAbsolutePath,
                DateTime.UtcNow,
                true);

            _inMemoryFileSystemProvider._files.Add(destinationFile);

            foreach (var child in childFileBag)
            {
                var destinationChild = _environmentProvider.JoinPaths(
                    destinationAbsolutePathString,
                    child.AbsolutePath.NameWithExtension);

                if (child.IsDirectory)
                {
                    await _inMemoryFileSystemProvider._directory.UnsafeMoveAsync(
                        child.AbsolutePath.Value,
                        destinationChild,
                        cancellationToken);
                }
                else
                {
                    await _inMemoryFileSystemProvider._file.UnsafeMoveAsync(
                        child.AbsolutePath.Value,
                        destinationChild,
                        cancellationToken);
                }

            }

            _inMemoryFileSystemProvider._files.RemoveAt(indexOfExistingFile);
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

            var childrenFromAllGenerationsBag = _inMemoryFileSystemProvider._files.Where(
                f => f.AbsolutePath.Value.StartsWith(absolutePathString) &&
                     f.AbsolutePath.Value != absolutePathString)
                .ToArray();

            var directChildren = childrenFromAllGenerationsBag.Where(
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

            var childrenFromAllGenerationsBag = _inMemoryFileSystemProvider._files.Where(
                f => f.AbsolutePath.Value.StartsWith(absolutePathString) &&
                     f.AbsolutePath.Value != absolutePathString);

            var directChildrenBag = childrenFromAllGenerationsBag.Where(
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