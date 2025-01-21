using Luthetus.Common.RazorLib.FileSystems.Models;
using Microsoft.Extensions.DependencyInjection;
using static Luthetus.Common.RazorLib.FileSystems.Models.InMemoryFileSystemProvider;
using static Luthetus.Common.Tests.Basis.FileSystems.FileSystemsTestsHelper;

namespace Luthetus.Common.Tests.Basis.FileSystems.Models;

/// <summary>
/// <see cref="InMemoryFileSystemProvider.File"/>
/// </summary>
public class InMemoryFileHandlerTests
{
    [Fact]
    public void Constructor()
    {
        /*
        public InMemoryFileHandler(
            InMemoryFileSystemProvider inMemoryFileSystemProvider, IEnvironmentProvider environmentProvider)
         */

        InitializeFileSystemsTests(
            out InMemoryEnvironmentProvider environmentProvider,
            out InMemoryFileSystemProvider fileSystemProvider,
            out ServiceProvider serviceProvider);

        // This assertion presumes that FileSystemsTestsHelper.InitializeFileSystemsTests
        // is returning as an out variable, an instance of InMemoryFileHandler
        Assert.IsType<InMemoryFileHandler>(fileSystemProvider.File);
    }

    [Fact]
    public async Task ExistsAsync()
    {
        /*
        public Task<bool> ExistsAsync(
            string absolutePathString, CancellationToken cancellationToken = default)
         */

        InitializeFileSystemsTests(
            out InMemoryEnvironmentProvider environmentProvider,
            out InMemoryFileSystemProvider fileSystemProvider,
            out ServiceProvider serviceProvider);

        Assert.True(await fileSystemProvider.File.ExistsAsync(WellKnownPaths.Files.AdditionTxt));
        Assert.False(await fileSystemProvider.File.ExistsAsync(WellKnownPaths.Files.NonExistingFile));

        // This is false because, 'WellKnownPaths.Directories.Biology' is a directory
        Assert.False(await fileSystemProvider.File.ExistsAsync(WellKnownPaths.Directories.Biology));
        Assert.False(await fileSystemProvider.File.ExistsAsync(WellKnownPaths.Directories.NonExistingDirectory));
    }

    [Fact]
    public async Task DeleteAsync()
    {
        /*
        public async Task DeleteAsync(
            string absolutePathString, CancellationToken cancellationToken = default)
         */

        InitializeFileSystemsTests(
            out InMemoryEnvironmentProvider environmentProvider,
            out InMemoryFileSystemProvider fileSystemProvider,
            out ServiceProvider serviceProvider);

        Assert.True(await fileSystemProvider.File.ExistsAsync(WellKnownPaths.Files.AdditionTxt));

        await fileSystemProvider.File.DeleteAsync(WellKnownPaths.Files.AdditionTxt);

        Assert.False(await fileSystemProvider.File.ExistsAsync(WellKnownPaths.Files.AdditionTxt));
    }

    [Fact]
    public async Task CopyAsync()
    {
        /*
        public async Task CopyAsync(
            string sourceAbsolutePathString, string destinationAbsolutePathString, CancellationToken cancellationToken = default)
         */

        InitializeFileSystemsTests(
            out InMemoryEnvironmentProvider environmentProvider,
            out InMemoryFileSystemProvider fileSystemProvider,
            out ServiceProvider serviceProvider);

        Assert.True(await fileSystemProvider.File.ExistsAsync(WellKnownPaths.Files.AdditionTxt));
        Assert.False(await fileSystemProvider.File.ExistsAsync(WellKnownPaths.Files.NonExistingFile));

        // This directory should be created when CopyAsync sees the parent directory does not exist
        Assert.False(await fileSystemProvider.Directory.ExistsAsync(WellKnownPaths.Directories.NonExistingDirectory));

        await fileSystemProvider.File.CopyAsync(WellKnownPaths.Files.AdditionTxt, WellKnownPaths.Files.NonExistingFile);

        // This directory should be created when CopyAsync sees the parent directory does not exist
        Assert.True(await fileSystemProvider.Directory.ExistsAsync(WellKnownPaths.Directories.NonExistingDirectory));

        Assert.True(await fileSystemProvider.File.ExistsAsync(WellKnownPaths.Files.NonExistingFile));

        // Ensure the source still exists
        Assert.True(await fileSystemProvider.File.ExistsAsync(WellKnownPaths.Files.AdditionTxt));

        Assert.Equal(
            await fileSystemProvider.File.ReadAllTextAsync(WellKnownPaths.Files.AdditionTxt),
            await fileSystemProvider.File.ReadAllTextAsync(WellKnownPaths.Files.NonExistingFile));
    }

    [Fact]
    public async Task MoveAsync()
    {
        /*
        public async Task MoveAsync(
            string sourceAbsolutePathString, string destinationAbsolutePathString, CancellationToken cancellationToken = default)
         */

        InitializeFileSystemsTests(
            out InMemoryEnvironmentProvider environmentProvider,
            out InMemoryFileSystemProvider fileSystemProvider,
            out ServiceProvider serviceProvider);

        Assert.True(await fileSystemProvider.File.ExistsAsync(WellKnownPaths.Files.AdditionTxt));
        Assert.False(await fileSystemProvider.File.ExistsAsync(WellKnownPaths.Files.NonExistingFile));

        // This directory should be created when CopyAsync sees the parent directory does not exist
        Assert.False(await fileSystemProvider.Directory.ExistsAsync(WellKnownPaths.Directories.NonExistingDirectory));

        var textFromAdditionTxt = await fileSystemProvider.File.ReadAllTextAsync(WellKnownPaths.Files.AdditionTxt);

        await fileSystemProvider.File.MoveAsync(WellKnownPaths.Files.AdditionTxt, WellKnownPaths.Files.NonExistingFile);

        // This directory should be created when CopyAsync sees the parent directory does not exist
        Assert.True(await fileSystemProvider.Directory.ExistsAsync(WellKnownPaths.Directories.NonExistingDirectory));

        Assert.True(await fileSystemProvider.File.ExistsAsync(WellKnownPaths.Files.NonExistingFile));

        // Ensure the source NO longer exists
        Assert.False(await fileSystemProvider.File.ExistsAsync(WellKnownPaths.Files.AdditionTxt));

        Assert.Equal(
            textFromAdditionTxt,
            await fileSystemProvider.File.ReadAllTextAsync(WellKnownPaths.Files.NonExistingFile));
    }

    [Fact]
    public async Task GetLastWriteTimeAsync()
    {
        /*
        public async Task<DateTime> GetLastWriteTimeAsync(
            string absolutePathString, CancellationToken cancellationToken = default)
         */

        InitializeFileSystemsTests(
            out InMemoryEnvironmentProvider environmentProvider,
            out InMemoryFileSystemProvider fileSystemProvider,
            out ServiceProvider serviceProvider);

        Assert.True(await fileSystemProvider.File.ExistsAsync(WellKnownPaths.Files.AdditionTxt));

        var lastWriteTime = await fileSystemProvider.File.GetLastWriteTimeAsync(WellKnownPaths.Files.AdditionTxt);

        Assert.NotEqual(DateTime.MinValue, lastWriteTime);
        Assert.NotEqual(DateTime.MaxValue, lastWriteTime);
    }

    [Fact]
    public async Task ReadAllTextAsync()
    {
        /*
        public async Task<string> ReadAllTextAsync(
            string absolutePathString, CancellationToken cancellationToken = default)
         */

        InitializeFileSystemsTests(
            out InMemoryEnvironmentProvider environmentProvider,
            out InMemoryFileSystemProvider fileSystemProvider,
            out ServiceProvider serviceProvider);

        Assert.Equal(
            "3 + 7 = 10",
            await fileSystemProvider.File.ReadAllTextAsync(WellKnownPaths.Files.AdditionTxt));
    }

    [Fact]
    public async Task WriteAllTextAsync()
    {
        /*
        public async Task WriteAllTextAsync(
            string absolutePathString, string contents, CancellationToken cancellationToken = default)
         */

        InitializeFileSystemsTests(
            out InMemoryEnvironmentProvider environmentProvider,
            out InMemoryFileSystemProvider fileSystemProvider,
            out ServiceProvider serviceProvider);

        Assert.Equal(
            "3 + 7 = 10",
            await fileSystemProvider.File.ReadAllTextAsync(WellKnownPaths.Files.AdditionTxt));

        var newText = "4 + 4 = 8";

        await fileSystemProvider.File.WriteAllTextAsync(WellKnownPaths.Files.AdditionTxt, newText);

        Assert.Equal(
            newText,
            await fileSystemProvider.File.ReadAllTextAsync(WellKnownPaths.Files.AdditionTxt));
    }
}