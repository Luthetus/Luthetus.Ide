using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using Fluxor;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.Installations.Models;
using Luthetus.Common.RazorLib.Misc;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Common.RazorLib.Exceptions;
using Luthetus.TextEditor.RazorLib.Installations.Models;
using Luthetus.Ide.RazorLib.Installations.Models;

using Luthetus.Extensions.Config.Installations.Models;

namespace Luthetus.Extensions.DotNet.Tests.Basis;

public class ExtensionsDotNetTestBase
{
    protected static void Test_RegisterServices(out ServiceProvider serviceProvider)
    {
        var backgroundTaskService = new BackgroundTaskServiceSynchronous();
        
        var hostingInformation = new LuthetusHostingInformation(
        	LuthetusHostingKind.UnitTestingSynchronous,
        	backgroundTaskService);

        var serviceCollection = new ServiceCollection()
            .AddScoped<IJSRuntime, DoNothingJsRuntime>()
            .AddLuthetusIdeRazorLibServices(hostingInformation)
            .AddLuthetusConfigServices(hostingInformation)
            .AddFluxor(options => options.ScanAssemblies(
                typeof(LuthetusCommonConfig).Assembly,
                typeof(LuthetusTextEditorConfig).Assembly,
                typeof(LuthetusIdeConfig).Assembly,
                typeof(Luthetus.Extensions.DotNet.Installations.Models.ServiceCollectionExtensions).Assembly));

        serviceProvider = serviceCollection.BuildServiceProvider();

        var store = serviceProvider.GetRequiredService<IStore>();
        store.InitializeAsync().Wait();

        var environmentProvider = serviceProvider.GetRequiredService<IEnvironmentProvider>();
        var fileSystemProvider = serviceProvider.GetRequiredService<IFileSystemProvider>();

        if (environmentProvider.GetType() != typeof(InMemoryEnvironmentProvider))
        {
            throw new LuthetusCommonException($"When unit testing one must use the {nameof(InMemoryEnvironmentProvider)} " +
                $"implementation of {nameof(IEnvironmentProvider)}. This will avoid any side effects from running the tests.");
        }

        if (fileSystemProvider.GetType() != typeof(InMemoryFileSystemProvider))
        {
            throw new LuthetusCommonException($"When unit testing one must use the {nameof(InMemoryFileSystemProvider)} " +
                $"implementation of {nameof(IFileSystemProvider)} This will avoid any side effects from running the tests.");
        }
    }

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
    protected static void Test_CreateFileSystem(IServiceProvider serviceProvider)
    {
        // Cast as the in-memory provider to ensure the actual filesystem will not be used.
        InMemoryEnvironmentProvider environmentProvider = 
            (InMemoryEnvironmentProvider)serviceProvider.GetRequiredService<IEnvironmentProvider>();

        // Cast as the in-memory provider to ensure the actual filesystem will not be used.
        InMemoryFileSystemProvider fileSystemProvider =
            (InMemoryFileSystemProvider)serviceProvider.GetRequiredService<IFileSystemProvider>();

        WriteToInMemoryFileSystem(
            environmentProvider,
            fileSystemProvider);
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

    protected class Test_WellKnownPaths
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
