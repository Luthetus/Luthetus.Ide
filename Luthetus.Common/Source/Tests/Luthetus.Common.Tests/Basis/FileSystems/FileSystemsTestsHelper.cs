using Luthetus.Common.RazorLib.FileSystems.Models;
using Microsoft.Extensions.DependencyInjection;

namespace Luthetus.Common.Tests.Basis.FileSystems;

public class FileSystemsTestsHelper
{
    /// <param name="inMemoryEnvironmentProvider">
    /// Register <see cref="IEnvironmentProvider"/> service to be <see cref="InMemoryEnvironmentProvider"/>,
    /// but keep the out variable with the concrete type. This provides clarity that
    /// the unit test won't create side effects in one's true filesystem,
    /// while still allowing the use of the dependency injected interface.
    /// </param>
    /// <param name="inMemoryFileSystemProvider">
    /// Register <see cref="IFileSystemProvider"/> service to be <see cref="InMemoryFileSystemProvider"/>,
    /// but keep the out variable with the concrete type. This provides clarity that
    /// the unit test won't create side effects in one's true filesystem,
    /// while still allowing the use of the dependency injected interface.
    /// </param>
    /// <summary>
    /// The files shown in the 'tree' below will be written to the in-memory filesystem.
    /// ---------------------------------------------<br/>
    /// Root<br/>
    /// ∙└───Homework<br/>
    /// ∙∙∙∙∙∙├───Math<br/>
    /// ∙∙∙∙∙∙│∙∙∙∙├───addition.txt<br/>
    /// ∙∙∙∙∙∙│∙∙∙∙└───subtraction.txt<br/>
    /// ∙∙∙∙∙∙│<br/>
    /// ∙∙∙∙∙∙└───Biology<br/>
    /// ∙∙∙∙∙∙∙∙∙∙∙├───nervousSystem.txt<br/>
    /// ∙∙∙∙∙∙∙∙∙∙∙└───skeletalSystem.txt<br/>
    /// </summary>
    public static void InitializeFileSystemsTests(
        out InMemoryEnvironmentProvider inMemoryEnvironmentProvider,
        out InMemoryFileSystemProvider inMemoryFileSystemProvider,
        out ServiceProvider serviceProvider)
    {
        // Cannot provide out variable to a lambda, so make local 'temporary' variables.
        //
        // There are other ways to achieve the result, but I want to
        // write out explicitly both 'new' expressions for anxiety's sake.
        //
        // I don't ever want a test somehow running on someone's true filesystem
        // and this explit 'new' helps me sleep at night.
        var tempInMemoryEnvironmentProvider = inMemoryEnvironmentProvider = new InMemoryEnvironmentProvider();
        var tempInMemoryFileSystemProvider = inMemoryFileSystemProvider = new InMemoryFileSystemProvider(tempInMemoryEnvironmentProvider);

        var services = new ServiceCollection()
            .AddScoped<IEnvironmentProvider>(sp => tempInMemoryEnvironmentProvider)
            .AddScoped<IFileSystemProvider>(sp => tempInMemoryFileSystemProvider);

        serviceProvider = services.BuildServiceProvider();

        WriteToInMemoryFileSystem(
            tempInMemoryEnvironmentProvider,
            tempInMemoryFileSystemProvider);
    }

    private static void WriteToInMemoryFileSystem(
        InMemoryEnvironmentProvider inMemoryEnvironmentProvider,
        InMemoryFileSystemProvider inMemoryFileSystemProvider)
    {
        var dsc = inMemoryEnvironmentProvider.DirectorySeparatorChar;

        inMemoryFileSystemProvider.File.WriteAllTextAsync(
            $"{dsc}Homework{dsc}Math{dsc}addition.txt",
            "3 + 7 = 10");

        inMemoryFileSystemProvider.File.WriteAllTextAsync(
            $"{dsc}Homework{dsc}Math{dsc}subtraction.txt",
            "10 - 3 = 7");

        inMemoryFileSystemProvider.File.WriteAllTextAsync(
            $"{dsc}Homework{dsc}Biology{dsc}nervousSystem.txt",
            "The nervous system is...");

        inMemoryFileSystemProvider.File.WriteAllTextAsync(
            $"{dsc}Homework{dsc}Biology{dsc}skeletalSystem.txt",
            "The skeletal system is...");
    }

    public class WellKnownPaths
    {
        public class Directories
        {
            public const string Root = "/";
            public const string Homework = "/Homework/";
            public const string Math = "/Homework/Math/";
            public const string Biology = "/Homework/Biology/";

            public const string NonExistingDirectory = "/Homework/Hamburger/";
        }

        public class Files
        {
            public const string AdditionTxt = "/Homework/Math/addition.txt";
            public const string SubtractionTxt = "/Homework/Math/subtraction.txt";
            public const string NervousSystemTxt = "/Homework/Biology/nervousSystem.txt";
            public const string SkeletalSystemTxt = "/Homework/Biology/skeletalSystem.txt";

            public const string NonExistingFile = "/Homework/Hamburger/recipe.txt";
        }
    }
}
