using Luthetus.Common.RazorLib.FileSystems.Models;
using Microsoft.Extensions.DependencyInjection;

namespace Luthetus.Common.Tests.Basis.FileSystems.Models;

/// <summary>
/// <see cref="FileSystemDrive"/>
/// </summary>
public class FileSystemDriveTests
{
    /// <summary>
    /// <see cref="FileSystemDrive(string, IEnvironmentProvider)"/>
    /// <br/>----<br/>
    /// <see cref="FileSystemDrive.DriveNameAsIdentifier"/>
    /// <see cref="FileSystemDrive.DriveNameAsPath"/>
    /// <see cref="FileSystemDrive.EnvironmentProvider"/>
    /// </summary>
    [Fact]
    public void Constructor()
    {
        FileSystemsTestsHelper.InitializeFileSystemsTests(
            out InMemoryEnvironmentProvider environmentProvider,
            out InMemoryFileSystemProvider fileSystemProvider,
            out ServiceProvider serviceProvider);

        var directorySeparatorCharBag = new[]
        {
            environmentProvider.DirectorySeparatorChar,
            environmentProvider.AltDirectorySeparatorChar,
        };

        foreach (var dsc in directorySeparatorCharBag)
        {
            // Root
            {
                var absolutePath = new AbsolutePath($@"C:{dsc}", true, environmentProvider);

                if (absolutePath.RootDrive is null)
                    throw new Exception($"{nameof(absolutePath.RootDrive)} was null, test failed");

                Assert.Equal("C", absolutePath.RootDrive.DriveNameAsIdentifier);
                Assert.Equal("C:", absolutePath.RootDrive.DriveNameAsPath);
            }

            // Directory
            {
                var absolutePath = new AbsolutePath($@"C:{dsc}Homework{dsc}Math{dsc}", true, environmentProvider);

                if (absolutePath.RootDrive is null)
                    throw new Exception($"{nameof(absolutePath.RootDrive)} was null, test failed");

                Assert.Equal("C", absolutePath.RootDrive.DriveNameAsIdentifier);
                Assert.Equal("C:", absolutePath.RootDrive.DriveNameAsPath);
            }

            // File
            {
                var absolutePath = new AbsolutePath($@"C:{dsc}Homework{dsc}Math{dsc}addition.txt", false, environmentProvider);

                if (absolutePath.RootDrive is null)
                    throw new Exception($"{nameof(absolutePath.RootDrive)} was null, test failed");

                Assert.Equal("C", absolutePath.RootDrive.DriveNameAsIdentifier);
                Assert.Equal("C:", absolutePath.RootDrive.DriveNameAsPath);
            }

            // No drive provided
            {
                var absolutePath = new AbsolutePath($@"{dsc}Homework{dsc}Math{dsc}addition.txt", false, environmentProvider);

                Assert.Null(absolutePath.RootDrive);
            }
        }
    }
}