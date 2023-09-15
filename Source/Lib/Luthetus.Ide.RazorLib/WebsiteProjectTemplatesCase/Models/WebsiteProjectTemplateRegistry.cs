using Luthetus.Common.RazorLib.FileSystem.Classes.LuthetusPath;
using Luthetus.Common.RazorLib.FileSystem.Models;
using System.Collections.Immutable;

namespace Luthetus.Ide.RazorLib.WebsiteProjectTemplatesCase.Models;

public static class WebsiteProjectTemplateRegistry
{
    static WebsiteProjectTemplateRegistry()
    {
        BlazorWasmEmptyProjectTemplate = new ProjectTemplate(
            "Blazor WebAssembly App Empty",
            "blazorwasm-empty",
            "[C#]",
            "Web/Blazor/WebAssembly/PWA/Empty");

        BlazorServerSideEmptyProjectTemplate = new ProjectTemplate(
            "Blazor Server App Empty",
            "blazorserver-empty",
            "[C#]",
            "Web/Blazor/Empty");

        ClassLibProjectTemplate = new ProjectTemplate(
            "Class Library",
            "classlib",
            "[C#],F#,VB",
            "Common/Library");

        RazorClassLibProjectTemplate = new ProjectTemplate(
            "Razor Class Library",
            "razorclasslib",
            "[C#]",
            "Web/Razor/Library/Razor Class Library");

        ConsoleAppProjectTemplate = new ProjectTemplate(
            "Console App",
            "console",
            "[C#],F#,VB",
            "Common/Console");

        XUnitProjectTemplate = new ProjectTemplate(
            "xUnit Test Project",
            "xunit",
            "[C#],F#,VB",
            "Test/xUnit");

        WebsiteProjectTemplatesContainer = new[]
        {
            BlazorWasmEmptyProjectTemplate,
            BlazorServerSideEmptyProjectTemplate,
            ConsoleAppProjectTemplate,
            ClassLibProjectTemplate,
            RazorClassLibProjectTemplate,
            XUnitProjectTemplate,
        }.ToImmutableArray();
    }

    public static ProjectTemplate BlazorWasmEmptyProjectTemplate { get; }
    public static ProjectTemplate BlazorServerSideEmptyProjectTemplate { get; }
    public static ProjectTemplate ClassLibProjectTemplate { get; }
    public static ProjectTemplate RazorClassLibProjectTemplate { get; }
    public static ProjectTemplate ConsoleAppProjectTemplate { get; }
    public static ProjectTemplate XUnitProjectTemplate { get; }

    public static ImmutableArray<ProjectTemplate> WebsiteProjectTemplatesContainer { get; }

    public static async Task HandleNewCSharpProjectAsync(
        string projectTemplateShortName,
        string cSharpProjectAbsolutePathString,
        IFileSystemProvider fileSystemProvider,
        IEnvironmentProvider environmentProvider)
    {
        if (projectTemplateShortName == BlazorWasmEmptyProjectTemplate.ShortName)
            await HandleBlazorWasmEmptyProjectTemplateAsync(cSharpProjectAbsolutePathString, fileSystemProvider, environmentProvider);
        else if (projectTemplateShortName == BlazorServerSideEmptyProjectTemplate.ShortName)
            await HandleBlazorServerSideEmptyProjectTemplateAsync(cSharpProjectAbsolutePathString, fileSystemProvider, environmentProvider);
        else if (projectTemplateShortName == ClassLibProjectTemplate.ShortName)
            await HandleClassLibProjectTemplateAsync(cSharpProjectAbsolutePathString, fileSystemProvider, environmentProvider);
        else if (projectTemplateShortName == RazorClassLibProjectTemplate.ShortName)
            await HandleRazorLibProjectTemplateAsync(cSharpProjectAbsolutePathString, fileSystemProvider, environmentProvider);
        else if (projectTemplateShortName == ConsoleAppProjectTemplate.ShortName)
            await HandleConsoleAppProjectTemplateAsync(cSharpProjectAbsolutePathString, fileSystemProvider, environmentProvider);
        else if (projectTemplateShortName == XUnitProjectTemplate.ShortName)
            await HandleXUnitProjectTemplateAsync(cSharpProjectAbsolutePathString, fileSystemProvider, environmentProvider);
        else
            throw new NotImplementedException($"The {nameof(ProjectTemplate.ShortName)}: '{projectTemplateShortName}' was not recognized.");
    }

    private static async Task HandleBlazorWasmEmptyProjectTemplateAsync(
        string cSharpProjectAbsolutePathString,
        IFileSystemProvider fileSystemProvider,
        IEnvironmentProvider environmentProvider)
    {
        var cSharpProjectAbsolutePath = new AbsolutePath(cSharpProjectAbsolutePathString, false, environmentProvider);
        var parentDirectoryOfProject = cSharpProjectAbsolutePath.ParentDirectory;

        if (parentDirectoryOfProject is null)
            throw new NotImplementedException();

        var parentDirectoryOfProjectAbsolutePathString = parentDirectoryOfProject.FormattedInput;

        // AppCss
        {
            var absolutePath = environmentProvider.JoinPaths(
                parentDirectoryOfProjectAbsolutePathString,
                BlazorWasmEmptyFacts.APP_CSS_RELATIVE_FILE_PATH);

            await fileSystemProvider.File.WriteAllTextAsync(
                absolutePath,
                BlazorWasmEmptyFacts.GetAppCssContents(cSharpProjectAbsolutePath.NameNoExtension));
        }

        // Csproj
        {
            await fileSystemProvider.File.WriteAllTextAsync(
                cSharpProjectAbsolutePathString,
                BlazorWasmEmptyFacts.GetCsprojContents(cSharpProjectAbsolutePath.NameNoExtension));
        }

        // ImportsRazor
        {
            var absolutePath = environmentProvider.JoinPaths(
                parentDirectoryOfProjectAbsolutePathString,
                BlazorWasmEmptyFacts.IMPORTS_RAZOR_RELATIVE_FILE_PATH);

            await fileSystemProvider.File.WriteAllTextAsync(
                absolutePath,
                BlazorWasmEmptyFacts.GetImportsRazorContents(cSharpProjectAbsolutePath.NameNoExtension));
        }

        // IndexHtml
        {
            var absolutePath = environmentProvider.JoinPaths(
                parentDirectoryOfProjectAbsolutePathString,
                BlazorWasmEmptyFacts.INDEX_HTML_RELATIVE_FILE_PATH);

            await fileSystemProvider.File.WriteAllTextAsync(
                absolutePath,
                BlazorWasmEmptyFacts.GetIndexHtmlContents(cSharpProjectAbsolutePath.NameNoExtension));
        }

        // IndexRazor
        {
            var absolutePath = environmentProvider.JoinPaths(
                parentDirectoryOfProjectAbsolutePathString,
                BlazorWasmEmptyFacts.INDEX_RAZOR_RELATIVE_FILE_PATH);

            await fileSystemProvider.File.WriteAllTextAsync(
                absolutePath,
                BlazorWasmEmptyFacts.GetIndexRazorContents(cSharpProjectAbsolutePath.NameNoExtension));
        }

        // LaunchSettingsJson
        {
            var absolutePath = environmentProvider.JoinPaths(
                parentDirectoryOfProjectAbsolutePathString,
                BlazorWasmEmptyFacts.LAUNCH_SETTINGS_JSON_RELATIVE_FILE_PATH);

            await fileSystemProvider.File.WriteAllTextAsync(
                absolutePath,
                BlazorWasmEmptyFacts.GetLaunchSettingsJsonContents(cSharpProjectAbsolutePath.NameNoExtension));
        }

        // MainLayoutRazor
        {
            var absolutePath = environmentProvider.JoinPaths(
                parentDirectoryOfProjectAbsolutePathString,
                BlazorWasmEmptyFacts.MAIN_LAYOUT_RAZOR_RELATIVE_FILE_PATH);

            await fileSystemProvider.File.WriteAllTextAsync(
                absolutePath,
                BlazorWasmEmptyFacts.GetMainLayoutRazorContents(cSharpProjectAbsolutePath.NameNoExtension));
        }

        // ProgramCs
        {
            var absolutePath = environmentProvider.JoinPaths(
                parentDirectoryOfProjectAbsolutePathString,
                BlazorWasmEmptyFacts.PROGRAM_CS_RELATIVE_FILE_PATH);

            await fileSystemProvider.File.WriteAllTextAsync(
                absolutePath,
                BlazorWasmEmptyFacts.GetProgramCsContents(cSharpProjectAbsolutePath.NameNoExtension));
        }
    }

    private static async Task HandleBlazorServerSideEmptyProjectTemplateAsync(
        string cSharpProjectAbsolutePathString,
        IFileSystemProvider fileSystemProvider,
        IEnvironmentProvider environmentProvider)
    {
        var cSharpProjectAbsolutePath = new AbsolutePath(cSharpProjectAbsolutePathString, false, environmentProvider);
        var parentDirectoryOfProject = cSharpProjectAbsolutePath.ParentDirectory;

        if (parentDirectoryOfProject is null)
            throw new NotImplementedException();

        var parentDirectoryOfProjectAbsolutePathString = parentDirectoryOfProject.FormattedInput;

        // AppRazor
        {
            var absolutePath = environmentProvider.JoinPaths(
                parentDirectoryOfProjectAbsolutePathString,
                BlazorServerEmptyFacts.APP_RAZOR_RELATIVE_FILE_PATH);

            await fileSystemProvider.File.WriteAllTextAsync(
                absolutePath,
                BlazorServerEmptyFacts.GetAppRazorContents(cSharpProjectAbsolutePath.NameNoExtension));
        }

        // AppSettingsDevelopmentJson
        {
            var absolutePath = environmentProvider.JoinPaths(
                parentDirectoryOfProjectAbsolutePathString,
                BlazorServerEmptyFacts.APP_SETTINGS_DEVELOPMENT_JSON_RELATIVE_FILE_PATH);

            await fileSystemProvider.File.WriteAllTextAsync(
                absolutePath,
                BlazorServerEmptyFacts.GetAppSettingsDevelopmentJsonContents(cSharpProjectAbsolutePath.NameNoExtension));
        }

        // AppSettingsJson
        {
            var absolutePath = environmentProvider.JoinPaths(
                parentDirectoryOfProjectAbsolutePathString,
                BlazorServerEmptyFacts.APP_SETTINGS_JSON_RELATIVE_FILE_PATH);

            await fileSystemProvider.File.WriteAllTextAsync(
                absolutePath,
                BlazorServerEmptyFacts.GetAppSettingsJsonContents(cSharpProjectAbsolutePath.NameNoExtension));
        }

        // Csproj
        {
            await fileSystemProvider.File.WriteAllTextAsync(
                cSharpProjectAbsolutePathString,
                BlazorServerEmptyFacts.GetCsprojContents(cSharpProjectAbsolutePath.NameNoExtension));
        }

        // HostCshtml
        {
            var absolutePath = environmentProvider.JoinPaths(
                parentDirectoryOfProjectAbsolutePathString,
                BlazorServerEmptyFacts.HOST_CSHTML_RELATIVE_FILE_PATH);

            await fileSystemProvider.File.WriteAllTextAsync(
                absolutePath,
                BlazorServerEmptyFacts.GetHostCshtmlContents(cSharpProjectAbsolutePath.NameNoExtension));
        }

        // ImportsRazor
        {
            var absolutePath = environmentProvider.JoinPaths(
                parentDirectoryOfProjectAbsolutePathString,
                BlazorServerEmptyFacts.IMPORTS_RAZOR_RELATIVE_FILE_PATH);

            await fileSystemProvider.File.WriteAllTextAsync(
                absolutePath,
                BlazorServerEmptyFacts.GetImportsRazorContents(cSharpProjectAbsolutePath.NameNoExtension));
        }

        // IndexRazor
        {
            var absolutePath = environmentProvider.JoinPaths(
                parentDirectoryOfProjectAbsolutePathString,
                BlazorServerEmptyFacts.INDEX_RAZOR_RELATIVE_FILE_PATH);

            await fileSystemProvider.File.WriteAllTextAsync(
                absolutePath,
                BlazorServerEmptyFacts.GetIndexRazorContents(cSharpProjectAbsolutePath.NameNoExtension));
        }

        // LaunchSettingsJson
        {
            var absolutePath = environmentProvider.JoinPaths(
                parentDirectoryOfProjectAbsolutePathString,
                BlazorServerEmptyFacts.LAUNCH_SETTINGS_JSON_RELATIVE_FILE_PATH);

            await fileSystemProvider.File.WriteAllTextAsync(
                absolutePath,
                BlazorServerEmptyFacts.GetLaunchSettingsJsonContents(cSharpProjectAbsolutePath.NameNoExtension));
        }

        // MainLayoutRazor
        {
            var absolutePath = environmentProvider.JoinPaths(
                parentDirectoryOfProjectAbsolutePathString,
                BlazorServerEmptyFacts.MAIN_LAYOUT_RAZOR_RELATIVE_FILE_PATH);

            await fileSystemProvider.File.WriteAllTextAsync(
                absolutePath,
                BlazorServerEmptyFacts.GetMainLayoutRazorContents(cSharpProjectAbsolutePath.NameNoExtension));
        }

        // ProgramCs
        {
            var absolutePath = environmentProvider.JoinPaths(
                parentDirectoryOfProjectAbsolutePathString,
                BlazorServerEmptyFacts.PROGRAM_CS_RELATIVE_FILE_PATH);

            await fileSystemProvider.File.WriteAllTextAsync(
                absolutePath,
                BlazorServerEmptyFacts.GetProgramCsContents(cSharpProjectAbsolutePath.NameNoExtension));
        }

        // SiteCss
        {
            var absolutePath = environmentProvider.JoinPaths(
                parentDirectoryOfProjectAbsolutePathString,
                BlazorServerEmptyFacts.SITE_CSS_RELATIVE_FILE_PATH);

            await fileSystemProvider.File.WriteAllTextAsync(
                absolutePath,
                BlazorServerEmptyFacts.GetSiteCssContents(cSharpProjectAbsolutePath.NameNoExtension));
        }
    }

    private static async Task HandleClassLibProjectTemplateAsync(
        string cSharpProjectAbsolutePathString,
        IFileSystemProvider fileSystemProvider,
        IEnvironmentProvider environmentProvider)
    {
        var cSharpProjectAbsolutePath = new AbsolutePath(cSharpProjectAbsolutePathString, false, environmentProvider);
        var parentDirectoryOfProject = cSharpProjectAbsolutePath.ParentDirectory;

        if (parentDirectoryOfProject is null)
            throw new NotImplementedException();

        var parentDirectoryOfProjectAbsolutePathString = parentDirectoryOfProject.FormattedInput;

        // Class1Cs
        {
            var absolutePath = environmentProvider.JoinPaths(
                parentDirectoryOfProjectAbsolutePathString,
                ClassLibFacts.CLASS_1_CS_RELATIVE_FILE_PATH);

            await fileSystemProvider.File.WriteAllTextAsync(
                absolutePath,
                ClassLibFacts.GetClass1CsContents(cSharpProjectAbsolutePath.NameNoExtension));
        }

        // Csproj
        {
            await fileSystemProvider.File.WriteAllTextAsync(
                cSharpProjectAbsolutePathString,
                ClassLibFacts.GetCsprojContents(cSharpProjectAbsolutePath.NameNoExtension));
        }
    }

    private static async Task HandleRazorLibProjectTemplateAsync(
        string cSharpProjectAbsolutePathString,
        IFileSystemProvider fileSystemProvider,
        IEnvironmentProvider environmentProvider)
    {
        var cSharpProjectAbsolutePath = new AbsolutePath(cSharpProjectAbsolutePathString, false, environmentProvider);
        var parentDirectoryOfProject = cSharpProjectAbsolutePath.ParentDirectory;

        if (parentDirectoryOfProject is null)
            throw new NotImplementedException();

        var parentDirectoryOfProjectAbsolutePathString = parentDirectoryOfProject.FormattedInput;

        // Component1Razor
        {
            var absolutePath = environmentProvider.JoinPaths(
                parentDirectoryOfProjectAbsolutePathString,
                RazorClassLibFacts.COMPONENT_1_RAZOR_RELATIVE_FILE_PATH);

            await fileSystemProvider.File.WriteAllTextAsync(
                absolutePath,
                RazorClassLibFacts.GetComponent1RazorContents(cSharpProjectAbsolutePath.NameNoExtension));
        }

        // Component1RazorCss
        {
            var absolutePath = environmentProvider.JoinPaths(
                parentDirectoryOfProjectAbsolutePathString,
                RazorClassLibFacts.COMPONENT_1_RAZOR_CSS_RELATIVE_FILE_PATH);

            await fileSystemProvider.File.WriteAllTextAsync(
                absolutePath,
                RazorClassLibFacts.GetComponent1RazorCssContents(cSharpProjectAbsolutePath.NameNoExtension));
        }

        // Csproj
        {
            await fileSystemProvider.File.WriteAllTextAsync(
                cSharpProjectAbsolutePathString,
                RazorClassLibFacts.GetCsprojContents(cSharpProjectAbsolutePath.NameNoExtension));
        }

        // ExampleJsInteropCs
        {
            var absolutePath = environmentProvider.JoinPaths(
                parentDirectoryOfProjectAbsolutePathString,
                RazorClassLibFacts.EXAMPLE_JS_INTEROP_CS_RELATIVE_FILE_PATH);

            await fileSystemProvider.File.WriteAllTextAsync(
                absolutePath,
                RazorClassLibFacts.GetExampleJsInteropCsContents(cSharpProjectAbsolutePath.NameNoExtension));
        }

        // ExampleJsInteropJs
        {
            var absolutePath = environmentProvider.JoinPaths(
                parentDirectoryOfProjectAbsolutePathString,
                RazorClassLibFacts.EXAMPLE_JS_INTEROP_JS_RELATIVE_FILE_PATH);

            await fileSystemProvider.File.WriteAllTextAsync(
                absolutePath,
                RazorClassLibFacts.GetExampleJsInteropJsContents(cSharpProjectAbsolutePath.NameNoExtension));
        }

        // ImportsRazor
        {
            var absolutePath = environmentProvider.JoinPaths(
                parentDirectoryOfProjectAbsolutePathString,
                RazorClassLibFacts.IMPORTS_RAZOR_RELATIVE_FILE_PATH);

            await fileSystemProvider.File.WriteAllTextAsync(
                absolutePath,
                RazorClassLibFacts.GetImportsRazorContents(cSharpProjectAbsolutePath.NameNoExtension));
        }
    }

    private static async Task HandleConsoleAppProjectTemplateAsync(
        string cSharpProjectAbsolutePathString,
        IFileSystemProvider fileSystemProvider,
        IEnvironmentProvider environmentProvider)
    {
        var cSharpProjectAbsolutePath = new AbsolutePath(cSharpProjectAbsolutePathString, false, environmentProvider);
        var parentDirectoryOfProject = cSharpProjectAbsolutePath.ParentDirectory;

        if (parentDirectoryOfProject is null)
            throw new NotImplementedException();

        var parentDirectoryOfProjectAbsolutePathString = parentDirectoryOfProject.FormattedInput;

        // ProgramCs
        {
            var absolutePath = environmentProvider.JoinPaths(
                parentDirectoryOfProjectAbsolutePathString,
                ConsoleAppFacts.PROGRAM_CS_RELATIVE_FILE_PATH);

            await fileSystemProvider.File.WriteAllTextAsync(
                absolutePath,
                ConsoleAppFacts.GetProgramCsContents(cSharpProjectAbsolutePath.NameNoExtension));
        }

        // Csproj
        {
            await fileSystemProvider.File.WriteAllTextAsync(
                cSharpProjectAbsolutePathString,
                ConsoleAppFacts.GetCsprojContents(cSharpProjectAbsolutePath.NameNoExtension));
        }
    }

    private static async Task HandleXUnitProjectTemplateAsync(
        string cSharpProjectAbsolutePathString,
        IFileSystemProvider fileSystemProvider,
        IEnvironmentProvider environmentProvider)
    {
        var cSharpProjectAbsolutePath = new AbsolutePath(cSharpProjectAbsolutePathString, false, environmentProvider);
        var parentDirectoryOfProject = cSharpProjectAbsolutePath.ParentDirectory;

        if (parentDirectoryOfProject is null)
            throw new NotImplementedException();

        var parentDirectoryOfProjectAbsolutePathString = parentDirectoryOfProject.FormattedInput;

        // Csproj
        {
            await fileSystemProvider.File.WriteAllTextAsync(
                cSharpProjectAbsolutePathString,
                XUnitFacts.GetCsprojContents(cSharpProjectAbsolutePath.NameNoExtension));
        }

        // UnitTest1Cs
        {
            var absolutePath = environmentProvider.JoinPaths(
                parentDirectoryOfProjectAbsolutePathString,
                XUnitFacts.UNIT_TEST_1_CS_RELATIVE_FILE_PATH);

            await fileSystemProvider.File.WriteAllTextAsync(
                absolutePath,
                XUnitFacts.GetUnitTest1CsContents(cSharpProjectAbsolutePath.NameNoExtension));
        }

        // UsingsCs
        {
            var absolutePath = environmentProvider.JoinPaths(
                parentDirectoryOfProjectAbsolutePathString,
                XUnitFacts.USINGS_CS_RELATIVE_FILE_PATH);

            await fileSystemProvider.File.WriteAllTextAsync(
                absolutePath,
                XUnitFacts.GetUsingsCsContents(cSharpProjectAbsolutePath.NameNoExtension));
        }
    }

    public static Guid GetProjectTypeGuid(string projectTemplateShortName)
    {
        // I'm not going to DRY up the string "FAE04EC0-301F-11D3-BF4B-00C04F79EFBC"
        // for now, because I don't fully understand its purpose.

        if (projectTemplateShortName == BlazorWasmEmptyProjectTemplate.ShortName)
            return Guid.Parse("FAE04EC0-301F-11D3-BF4B-00C04F79EFBC");
        else if (projectTemplateShortName == BlazorServerSideEmptyProjectTemplate.ShortName)
            return Guid.Parse("FAE04EC0-301F-11D3-BF4B-00C04F79EFBC");
        else if (projectTemplateShortName == ClassLibProjectTemplate.ShortName)
            return Guid.Parse("FAE04EC0-301F-11D3-BF4B-00C04F79EFBC");
        else if (projectTemplateShortName == RazorClassLibProjectTemplate.ShortName)
            return Guid.Parse("FAE04EC0-301F-11D3-BF4B-00C04F79EFBC");
        else if (projectTemplateShortName == ConsoleAppProjectTemplate.ShortName)
            return Guid.Parse("FAE04EC0-301F-11D3-BF4B-00C04F79EFBC");
        else if (projectTemplateShortName == XUnitProjectTemplate.ShortName)
            return Guid.Parse("FAE04EC0-301F-11D3-BF4B-00C04F79EFBC");
        else
            throw new NotImplementedException($"The {nameof(ProjectTemplate.ShortName)}: '{projectTemplateShortName}' was not recognized.");
    }


}
