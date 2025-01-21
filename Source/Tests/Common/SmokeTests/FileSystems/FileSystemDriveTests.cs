﻿using Luthetus.Common.RazorLib.FileSystems.Models;
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

                if (absolutePath.RootDrive is null)
                    throw new Exception($"{nameof(absolutePath.RootDrive)} was null, test failed");

                Assert.Equal("C", absolutePath.RootDrive.DriveNameAsIdentifier);
                Assert.Equal("C:", absolutePath.RootDrive.DriveNameAsPath);
            }

            // Directory
            {
                var absolutePath = environmentProvider.AbsolutePathFactory($@"C:{dsc}Homework{dsc}Math{dsc}", true);

                if (absolutePath.RootDrive is null)
                    throw new Exception($"{nameof(absolutePath.RootDrive)} was null, test failed");

                Assert.Equal("C", absolutePath.RootDrive.DriveNameAsIdentifier);
                Assert.Equal("C:", absolutePath.RootDrive.DriveNameAsPath);
            }

            // File
            {
                var absolutePath = environmentProvider.AbsolutePathFactory($@"C:{dsc}Homework{dsc}Math{dsc}addition.txt", false);

                if (absolutePath.RootDrive is null)
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