using Microsoft.Extensions.DependencyInjection;
using Luthetus.Common.RazorLib.FileSystems.Models;

namespace Luthetus.Common.Tests.SmokeTests.FileSystems;

public class FileSystemDriveTests
{
    [Fact]
    public void Constructor()
    {
        FileSystemsTestsHelper.InitializeFileSystemsTests(
            out InMemoryEnvironmentProvider environmentProvider,
            out InMemoryFileSystemProvider fileSystemProvider,
            out ServiceProvider serviceProvider);

        var directorySeparatorCharList = new[]
        {
            environmentProvider.DirectorySeparatorChar,
            environmentProvider.AltDirectorySeparatorChar,
        };

        foreach (var dsc in directorySeparatorCharList)
        {
            // Root
            {
                var absolutePath = environmentProvider.AbsolutePathFactory($@"C:{dsc}", true);

                if (absolutePath.RootDrive.DriveNameAsIdentifier is null)
                    throw new Exception($"{nameof(absolutePath.RootDrive)} was null, test failed");

                Assert.Equal("C", absolutePath.RootDrive.DriveNameAsIdentifier);
                Assert.Equal("C:", absolutePath.RootDrive.DriveNameAsPath);
            }

            // Directory
            {
                var absolutePath = environmentProvider.AbsolutePathFactory($@"C:{dsc}Homework{dsc}Math{dsc}", true);

                if (absolutePath.RootDrive.DriveNameAsIdentifier is null)
                    throw new Exception($"{nameof(absolutePath.RootDrive)} was null, test failed");

                Assert.Equal("C", absolutePath.RootDrive.DriveNameAsIdentifier);
                Assert.Equal("C:", absolutePath.RootDrive.DriveNameAsPath);
            }

            // File
            {
                var absolutePath = environmentProvider.AbsolutePathFactory($@"C:{dsc}Homework{dsc}Math{dsc}addition.txt", false);

                if (absolutePath.RootDrive.DriveNameAsIdentifier is null)
                    throw new Exception($"{nameof(absolutePath.RootDrive)} was null, test failed");

                Assert.Equal("C", absolutePath.RootDrive.DriveNameAsIdentifier);
                Assert.Equal("C:", absolutePath.RootDrive.DriveNameAsPath);
            }

            // No drive provided
            {
                var absolutePath = environmentProvider.AbsolutePathFactory($@"{dsc}Homework{dsc}Math{dsc}addition.txt", false);

                Assert.Null(absolutePath.RootDrive);
            }
        }
    }
}
