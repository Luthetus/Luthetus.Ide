using Luthetus.Common.RazorLib.FileSystems.Models;
using System.Collections.Immutable;

namespace Luthetus.Ide.RazorLib.Websites.ProjectTemplates.Models;

public static class WebsiteProjectTemplateFacts
{
    static WebsiteProjectTemplateFacts()
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
            await HandleBlazorWasmEmptyProjectTemplateAsync(cSharpProjectAbsolutePathString, fileSystemProvider, environmentProvider)
                .ConfigureAwait(false);
        else if (projectTemplateShortName == BlazorServerSideEmptyProjectTemplate.ShortName)
            await HandleBlazorServerSideEmptyProjectTemplateAsync(cSharpProjectAbsolutePathString, fileSystemProvider, environmentProvider)
                .ConfigureAwait(false);
        else if (projectTemplateShortName == ClassLibProjectTemplate.ShortName)
            await HandleClassLibProjectTemplateAsync(cSharpProjectAbsolutePathString, fileSystemProvider, environmentProvider)
                .ConfigureAwait(false);
        else if (projectTemplateShortName == RazorClassLibProjectTemplate.ShortName)
            await HandleRazorLibProjectTemplateAsync(cSharpProjectAbsolutePathString, fileSystemProvider, environmentProvider)
                .ConfigureAwait(false);
        else if (projectTemplateShortName == ConsoleAppProjectTemplate.ShortName)
            await HandleConsoleAppProjectTemplateAsync(cSharpProjectAbsolutePathString, fileSystemProvider, environmentProvider)
                .ConfigureAwait(false);
        else if (projectTemplateShortName == XUnitProjectTemplate.ShortName)
            await HandleXUnitProjectTemplateAsync(cSharpProjectAbsolutePathString, fileSystemProvider, environmentProvider)
                .ConfigureAwait(false);
        else
            throw new NotImplementedException($"The {nameof(ProjectTemplate.ShortName)}: '{projectTemplateShortName}' was not recognized.");
    }

    private static async Task HandleBlazorWasmEmptyProjectTemplateAsync(
        string cSharpProjectAbsolutePathString,
        IFileSystemProvider fileSystemProvider,
        IEnvironmentProvider environmentProvider)
    {
        var cSharpProjectAbsolutePath = environmentProvider.AbsolutePathFactory(cSharpProjectAbsolutePathString, false);
        var parentDirectoryOfProject = cSharpProjectAbsolutePath.ParentDirectory;

        if (parentDirectoryOfProject is null)
            throw new NotImplementedException();

        var parentDirectoryOfProjectAbsolutePath = parentDirectoryOfProject;

        // AppCss
        {
            var absolutePath = environmentProvider.JoinPaths(
                parentDirectoryOfProjectAbsolutePath.Value,
                BlazorWasmEmptyFacts.APP_CSS_RELATIVE_FILE_PATH);

            await fileSystemProvider.File.WriteAllTextAsync(
                    absolutePath,
                    BlazorWasmEmptyFacts.GetAppCssContents(cSharpProjectAbsolutePath.NameNoExtension))
                .ConfigureAwait(false);
        }

        // Csproj
        {
            await fileSystemProvider.File.WriteAllTextAsync(
                    cSharpProjectAbsolutePathString,
                    BlazorWasmEmptyFacts.GetCsprojContents(cSharpProjectAbsolutePath.NameNoExtension))
                .ConfigureAwait(false);
        }

        // ImportsRazor
        {
            var absolutePath = environmentProvider.JoinPaths(
                parentDirectoryOfProjectAbsolutePath.Value,
                BlazorWasmEmptyFacts.IMPORTS_RAZOR_RELATIVE_FILE_PATH);

            await fileSystemProvider.File.WriteAllTextAsync(
                    absolutePath,
                    BlazorWasmEmptyFacts.GetImportsRazorContents(cSharpProjectAbsolutePath.NameNoExtension))
                .ConfigureAwait(false);
        }

        // IndexHtml
        {
            var absolutePath = environmentProvider.JoinPaths(
                parentDirectoryOfProjectAbsolutePath.Value,
                BlazorWasmEmptyFacts.INDEX_HTML_RELATIVE_FILE_PATH);

            await fileSystemProvider.File.WriteAllTextAsync(
                    absolutePath,
                    BlazorWasmEmptyFacts.GetIndexHtmlContents(cSharpProjectAbsolutePath.NameNoExtension))
                .ConfigureAwait(false);
        }

        // IndexRazor
        {
            var absolutePath = environmentProvider.JoinPaths(
                parentDirectoryOfProjectAbsolutePath.Value,
                BlazorWasmEmptyFacts.INDEX_RAZOR_RELATIVE_FILE_PATH);

            await fileSystemProvider.File.WriteAllTextAsync(
                    absolutePath,
                    BlazorWasmEmptyFacts.GetIndexRazorContents(cSharpProjectAbsolutePath.NameNoExtension))
                .ConfigureAwait(false);
        }

        // LaunchSettingsJson
        {
            var absolutePath = environmentProvider.JoinPaths(
                parentDirectoryOfProjectAbsolutePath.Value,
                BlazorWasmEmptyFacts.LAUNCH_SETTINGS_JSON_RELATIVE_FILE_PATH);

            await fileSystemProvider.File.WriteAllTextAsync(
                    absolutePath,
                    BlazorWasmEmptyFacts.GetLaunchSettingsJsonContents(cSharpProjectAbsolutePath.NameNoExtension))
                .ConfigureAwait(false);
        }

        // MainLayoutRazor
        {
            var absolutePath = environmentProvider.JoinPaths(
                parentDirectoryOfProjectAbsolutePath.Value,
                BlazorWasmEmptyFacts.MAIN_LAYOUT_RAZOR_RELATIVE_FILE_PATH);

            await fileSystemProvider.File.WriteAllTextAsync(
                    absolutePath,
                    BlazorWasmEmptyFacts.GetMainLayoutRazorContents(cSharpProjectAbsolutePath.NameNoExtension))
                .ConfigureAwait(false);
        }

        // ProgramCs
        {
            var absolutePath = environmentProvider.JoinPaths(
                parentDirectoryOfProjectAbsolutePath.Value,
                BlazorWasmEmptyFacts.PROGRAM_CS_RELATIVE_FILE_PATH);

            await fileSystemProvider.File.WriteAllTextAsync(
                    absolutePath,
                    BlazorWasmEmptyFacts.GetProgramCsContents(cSharpProjectAbsolutePath.NameNoExtension))
                .ConfigureAwait(false);
        }
    }

    private static async Task HandleBlazorServerSideEmptyProjectTemplateAsync(
        string cSharpProjectAbsolutePathString,
        IFileSystemProvider fileSystemProvider,
        IEnvironmentProvider environmentProvider)
    {
        var cSharpProjectAbsolutePath = environmentProvider.AbsolutePathFactory(cSharpProjectAbsolutePathString, false);
        var parentDirectoryOfProject = cSharpProjectAbsolutePath.ParentDirectory;

        if (parentDirectoryOfProject is null)
            throw new NotImplementedException();

        var ancestorDirectory = parentDirectoryOfProject;

        // AppRazor
        {
            var absolutePath = environmentProvider.JoinPaths(
                ancestorDirectory.Value,
                BlazorServerEmptyFacts.APP_RAZOR_RELATIVE_FILE_PATH);

            await fileSystemProvider.File.WriteAllTextAsync(
                    absolutePath,
                    BlazorServerEmptyFacts.GetAppRazorContents(cSharpProjectAbsolutePath.NameNoExtension))
                .ConfigureAwait(false);
        }

        // AppSettingsDevelopmentJson
        {
            var absolutePath = environmentProvider.JoinPaths(
                ancestorDirectory.Value,
                BlazorServerEmptyFacts.APP_SETTINGS_DEVELOPMENT_JSON_RELATIVE_FILE_PATH);

            await fileSystemProvider.File.WriteAllTextAsync(
                    absolutePath,
                    BlazorServerEmptyFacts.GetAppSettingsDevelopmentJsonContents(cSharpProjectAbsolutePath.NameNoExtension))
                .ConfigureAwait(false);
        }

        // AppSettingsJson
        {
            var absolutePath = environmentProvider.JoinPaths(
                ancestorDirectory.Value,
                BlazorServerEmptyFacts.APP_SETTINGS_JSON_RELATIVE_FILE_PATH);

            await fileSystemProvider.File.WriteAllTextAsync(
                    absolutePath,
                    BlazorServerEmptyFacts.GetAppSettingsJsonContents(cSharpProjectAbsolutePath.NameNoExtension))
                .ConfigureAwait(false);
        }

        // Csproj
        {
            await fileSystemProvider.File.WriteAllTextAsync(
                    cSharpProjectAbsolutePathString,
                    BlazorServerEmptyFacts.GetCsprojContents(cSharpProjectAbsolutePath.NameNoExtension))
                .ConfigureAwait(false);
        }

        // HostCshtml
        {
            var absolutePath = environmentProvider.JoinPaths(
                ancestorDirectory.Value,
                BlazorServerEmptyFacts.HOST_CSHTML_RELATIVE_FILE_PATH);

            await fileSystemProvider.File.WriteAllTextAsync(
                    absolutePath,
                    BlazorServerEmptyFacts.GetHostCshtmlContents(cSharpProjectAbsolutePath.NameNoExtension))
                .ConfigureAwait(false);
        }

        // ImportsRazor
        {
            var absolutePath = environmentProvider.JoinPaths(
                ancestorDirectory.Value,
                BlazorServerEmptyFacts.IMPORTS_RAZOR_RELATIVE_FILE_PATH);

            await fileSystemProvider.File.WriteAllTextAsync(
                    absolutePath,
                    BlazorServerEmptyFacts.GetImportsRazorContents(cSharpProjectAbsolutePath.NameNoExtension))
                .ConfigureAwait(false);
        }

        // IndexRazor
        {
            var absolutePath = environmentProvider.JoinPaths(
                ancestorDirectory.Value,
                BlazorServerEmptyFacts.INDEX_RAZOR_RELATIVE_FILE_PATH);

            await fileSystemProvider.File.WriteAllTextAsync(
                    absolutePath,
                    BlazorServerEmptyFacts.GetIndexRazorContents(cSharpProjectAbsolutePath.NameNoExtension))
                .ConfigureAwait(false);
        }

        // LaunchSettingsJson
        {
            var absolutePath = environmentProvider.JoinPaths(
                ancestorDirectory.Value,
                BlazorServerEmptyFacts.LAUNCH_SETTINGS_JSON_RELATIVE_FILE_PATH);

            await fileSystemProvider.File.WriteAllTextAsync(
                    absolutePath,
                    BlazorServerEmptyFacts.GetLaunchSettingsJsonContents(cSharpProjectAbsolutePath.NameNoExtension))
                .ConfigureAwait(false);
        }

        // MainLayoutRazor
        {
            var absolutePath = environmentProvider.JoinPaths(
                ancestorDirectory.Value,
                BlazorServerEmptyFacts.MAIN_LAYOUT_RAZOR_RELATIVE_FILE_PATH);

            await fileSystemProvider.File.WriteAllTextAsync(
                    absolutePath,
                    BlazorServerEmptyFacts.GetMainLayoutRazorContents(cSharpProjectAbsolutePath.NameNoExtension))
                .ConfigureAwait(false);
        }

        // ProgramCs
        {
            var absolutePath = environmentProvider.JoinPaths(
                ancestorDirectory.Value,
                BlazorServerEmptyFacts.PROGRAM_CS_RELATIVE_FILE_PATH);

            await fileSystemProvider.File.WriteAllTextAsync(
                    absolutePath,
                    BlazorServerEmptyFacts.GetProgramCsContents(cSharpProjectAbsolutePath.NameNoExtension))
                .ConfigureAwait(false);
        }

        // SiteCss
        {
            var absolutePath = environmentProvider.JoinPaths(
                ancestorDirectory.Value,
                BlazorServerEmptyFacts.SITE_CSS_RELATIVE_FILE_PATH);

            await fileSystemProvider.File.WriteAllTextAsync(
                    absolutePath,
                    BlazorServerEmptyFacts.GetSiteCssContents(cSharpProjectAbsolutePath.NameNoExtension))
                .ConfigureAwait(false);
        }
    }

    private static async Task HandleClassLibProjectTemplateAsync(
        string cSharpProjectAbsolutePathString,
        IFileSystemProvider fileSystemProvider,
        IEnvironmentProvider environmentProvider)
    {
        var cSharpProjectAbsolutePath = environmentProvider.AbsolutePathFactory(cSharpProjectAbsolutePathString, false);
        var parentDirectoryOfProject = cSharpProjectAbsolutePath.ParentDirectory;

        if (parentDirectoryOfProject is null)
            throw new NotImplementedException();

        var ancestorDirectory = parentDirectoryOfProject;

        // Class1Cs
        {
            var absolutePath = environmentProvider.JoinPaths(
                ancestorDirectory.Value,
                ClassLibFacts.CLASS_1_CS_RELATIVE_FILE_PATH);

            await fileSystemProvider.File.WriteAllTextAsync(
                    absolutePath,
                    ClassLibFacts.GetClass1CsContents(cSharpProjectAbsolutePath.NameNoExtension))
                .ConfigureAwait(false);
        }

        // Csproj
        {
            await fileSystemProvider.File.WriteAllTextAsync(
                    cSharpProjectAbsolutePathString,
                    ClassLibFacts.GetCsprojContents(cSharpProjectAbsolutePath.NameNoExtension))
                .ConfigureAwait(false);
        }
    }

    private static async Task HandleRazorLibProjectTemplateAsync(
        string cSharpProjectAbsolutePathString,
        IFileSystemProvider fileSystemProvider,
        IEnvironmentProvider environmentProvider)
    {
        var cSharpProjectAbsolutePath = environmentProvider.AbsolutePathFactory(cSharpProjectAbsolutePathString, false);
        var parentDirectoryOfProject = cSharpProjectAbsolutePath.ParentDirectory;

        if (parentDirectoryOfProject is null)
            throw new NotImplementedException();

        var ancestorDirectory = parentDirectoryOfProject;

        // Component1Razor
        {
            var absolutePath = environmentProvider.JoinPaths(
                ancestorDirectory.Value,
                RazorClassLibFacts.COMPONENT_1_RAZOR_RELATIVE_FILE_PATH);

            await fileSystemProvider.File.WriteAllTextAsync(
                    absolutePath,
                    RazorClassLibFacts.GetComponent1RazorContents(cSharpProjectAbsolutePath.NameNoExtension))
                .ConfigureAwait(false);
        }

        // Component1RazorCss
        {
            var absolutePath = environmentProvider.JoinPaths(
                ancestorDirectory.Value,
                RazorClassLibFacts.COMPONENT_1_RAZOR_CSS_RELATIVE_FILE_PATH);

            await fileSystemProvider.File.WriteAllTextAsync(
                    absolutePath,
                    RazorClassLibFacts.GetComponent1RazorCssContents(cSharpProjectAbsolutePath.NameNoExtension))
                .ConfigureAwait(false);
        }

        // Csproj
        {
            await fileSystemProvider.File.WriteAllTextAsync(
                    cSharpProjectAbsolutePathString,
                    RazorClassLibFacts.GetCsprojContents(cSharpProjectAbsolutePath.NameNoExtension))
                .ConfigureAwait(false);
        }

        // ExampleJsInteropCs
        {
            var absolutePath = environmentProvider.JoinPaths(
                ancestorDirectory.Value,
                RazorClassLibFacts.EXAMPLE_JS_INTEROP_CS_RELATIVE_FILE_PATH);

            await fileSystemProvider.File.WriteAllTextAsync(
                    absolutePath,
                    RazorClassLibFacts.GetExampleJsInteropCsContents(cSharpProjectAbsolutePath.NameNoExtension))
                .ConfigureAwait(false);
        }

        // ExampleJsInteropJs
        {
            var absolutePath = environmentProvider.JoinPaths(
                ancestorDirectory.Value,
                RazorClassLibFacts.EXAMPLE_JS_INTEROP_JS_RELATIVE_FILE_PATH);

            await fileSystemProvider.File.WriteAllTextAsync(
                    absolutePath,
                    RazorClassLibFacts.GetExampleJsInteropJsContents(cSharpProjectAbsolutePath.NameNoExtension))
                .ConfigureAwait(false);
        }

        // ImportsRazor
        {
            var absolutePath = environmentProvider.JoinPaths(
                ancestorDirectory.Value,
                RazorClassLibFacts.IMPORTS_RAZOR_RELATIVE_FILE_PATH);

            await fileSystemProvider.File.WriteAllTextAsync(
                    absolutePath,
                    RazorClassLibFacts.GetImportsRazorContents(cSharpProjectAbsolutePath.NameNoExtension))
                .ConfigureAwait(false);
        }
    }

    private static async Task HandleConsoleAppProjectTemplateAsync(
        string cSharpProjectAbsolutePathString,
        IFileSystemProvider fileSystemProvider,
        IEnvironmentProvider environmentProvider)
    {
        var cSharpProjectAbsolutePath = environmentProvider.AbsolutePathFactory(cSharpProjectAbsolutePathString, false);
        var parentDirectoryOfProject = cSharpProjectAbsolutePath.ParentDirectory;

        if (parentDirectoryOfProject is null)
            throw new NotImplementedException();

        var ancestorDirectory = parentDirectoryOfProject;

        // ProgramCs
        {
            var absolutePath = environmentProvider.JoinPaths(
                ancestorDirectory.Value,
                ConsoleAppFacts.PROGRAM_CS_RELATIVE_FILE_PATH);

            await fileSystemProvider.File.WriteAllTextAsync(
                    absolutePath,
                    ConsoleAppFacts.GetProgramCsContents(cSharpProjectAbsolutePath.NameNoExtension))
                .ConfigureAwait(false);
        }

        // Csproj
        {
            await fileSystemProvider.File.WriteAllTextAsync(
                    cSharpProjectAbsolutePathString,
                    ConsoleAppFacts.GetCsprojContents(cSharpProjectAbsolutePath.NameNoExtension))
                .ConfigureAwait(false);
        }
    }

    private static async Task HandleXUnitProjectTemplateAsync(
        string cSharpProjectAbsolutePathString,
        IFileSystemProvider fileSystemProvider,
        IEnvironmentProvider environmentProvider)
    {
        var cSharpProjectAbsolutePath = environmentProvider.AbsolutePathFactory(cSharpProjectAbsolutePathString, false);
        var parentDirectoryOfProject = cSharpProjectAbsolutePath.ParentDirectory;

        if (parentDirectoryOfProject is null)
            throw new NotImplementedException();

        var ancestorDirectory = parentDirectoryOfProject;

        // Csproj
        {
            await fileSystemProvider.File.WriteAllTextAsync(
                    cSharpProjectAbsolutePathString,
                    XUnitFacts.GetCsprojContents(cSharpProjectAbsolutePath.NameNoExtension))
                .ConfigureAwait(false);
        }

        // UnitTest1Cs
        {
            var absolutePath = environmentProvider.JoinPaths(
                ancestorDirectory.Value,
                XUnitFacts.UNIT_TEST_1_CS_RELATIVE_FILE_PATH);

            await fileSystemProvider.File.WriteAllTextAsync(
                    absolutePath,
                    XUnitFacts.GetUnitTest1CsContents(cSharpProjectAbsolutePath.NameNoExtension))
                .ConfigureAwait(false);
        }

        // UsingsCs
        {
            var absolutePath = environmentProvider.JoinPaths(
                ancestorDirectory.Value,
                XUnitFacts.USINGS_CS_RELATIVE_FILE_PATH);

            await fileSystemProvider.File.WriteAllTextAsync(
                    absolutePath,
                    XUnitFacts.GetUsingsCsContents(cSharpProjectAbsolutePath.NameNoExtension))
                .ConfigureAwait(false);
        }
    }

    public static Guid GetProjectTypeGuid(string projectTemplateShortName)
    {
        // I'm not going to DRY up the string "FAE04EC0-301F-11D3-BF4B-00C04F79EFBC" for now,
        // because I don't fully understand its purpose.

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
