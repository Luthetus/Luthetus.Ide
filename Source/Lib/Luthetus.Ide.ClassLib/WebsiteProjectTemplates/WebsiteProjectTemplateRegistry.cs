using Luthetus.Common.RazorLib.FileSystem.Classes.FilePath;
using Luthetus.Common.RazorLib.FileSystem.Interfaces;
using Luthetus.Ide.ClassLib.WebsiteProjectTemplates.BlazorServerEmptyCase;
using Luthetus.Ide.ClassLib.WebsiteProjectTemplates.BlazorWasmEmptyCase;
using Luthetus.Ide.ClassLib.WebsiteProjectTemplates.ClassLibCase;
using Luthetus.Ide.ClassLib.WebsiteProjectTemplates.ConsoleAppCase;
using Luthetus.Ide.ClassLib.WebsiteProjectTemplates.RazorClassLibCase;
using Luthetus.Ide.ClassLib.WebsiteProjectTemplates.XUnitCase;
using System.Collections.Immutable;

namespace Luthetus.Ide.ClassLib.WebsiteProjectTemplates;

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
        string cSharpProjectAbsoluteFilePathString,
        IFileSystemProvider fileSystemProvider,
        IEnvironmentProvider environmentProvider)
    {
        if (projectTemplateShortName == BlazorWasmEmptyProjectTemplate.ShortName)
            await HandleBlazorWasmEmptyProjectTemplateAsync(cSharpProjectAbsoluteFilePathString, fileSystemProvider, environmentProvider);
        else if (projectTemplateShortName == BlazorServerSideEmptyProjectTemplate.ShortName)
            await HandleBlazorServerSideEmptyProjectTemplateAsync(cSharpProjectAbsoluteFilePathString, fileSystemProvider, environmentProvider);
        else if (projectTemplateShortName == ClassLibProjectTemplate.ShortName)
            await HandleClassLibProjectTemplateAsync(cSharpProjectAbsoluteFilePathString, fileSystemProvider, environmentProvider);
        else if (projectTemplateShortName == RazorClassLibProjectTemplate.ShortName)
            await HandleRazorLibProjectTemplateAsync(cSharpProjectAbsoluteFilePathString, fileSystemProvider, environmentProvider);
        else if (projectTemplateShortName == ConsoleAppProjectTemplate.ShortName)
            await HandleConsoleAppProjectTemplateAsync(cSharpProjectAbsoluteFilePathString, fileSystemProvider, environmentProvider);
        else if (projectTemplateShortName == XUnitProjectTemplate.ShortName)
            await HandleXUnitProjectTemplateAsync(cSharpProjectAbsoluteFilePathString, fileSystemProvider, environmentProvider);
        else
            throw new NotImplementedException($"The {nameof(ProjectTemplate.ShortName)}: '{projectTemplateShortName}' was not recognized.");
    }

    private static async Task HandleBlazorWasmEmptyProjectTemplateAsync(
        string cSharpProjectAbsoluteFilePathString,
        IFileSystemProvider fileSystemProvider,
        IEnvironmentProvider environmentProvider)
    {
        var cSharpProjectAbsoluteFilePath = new AbsoluteFilePath(cSharpProjectAbsoluteFilePathString, false, environmentProvider);
        var parentDirectoryOfProject = cSharpProjectAbsoluteFilePath.ParentDirectory;

        if (parentDirectoryOfProject is null)
            throw new NotImplementedException();

        var parentDirectoryOfProjectAbsoluteFilePathString = parentDirectoryOfProject.GetAbsoluteFilePathString();

        // AppCss
        {
            var absoluteFilePath = environmentProvider.JoinPaths(
                parentDirectoryOfProjectAbsoluteFilePathString,
                BlazorWasmEmptyFacts.APP_CSS_RELATIVE_FILE_PATH);

            await fileSystemProvider.File.WriteAllTextAsync(
                absoluteFilePath,
                BlazorWasmEmptyFacts.GetAppCssContents(cSharpProjectAbsoluteFilePath.FileNameNoExtension));
        }

        // Csproj
        {
            await fileSystemProvider.File.WriteAllTextAsync(
                cSharpProjectAbsoluteFilePathString,
                BlazorWasmEmptyFacts.GetCsprojContents(cSharpProjectAbsoluteFilePath.FileNameNoExtension));
        }

        // ImportsRazor
        {
            var absoluteFilePath = environmentProvider.JoinPaths(
                parentDirectoryOfProjectAbsoluteFilePathString,
                BlazorWasmEmptyFacts.IMPORTS_RAZOR_RELATIVE_FILE_PATH);

            await fileSystemProvider.File.WriteAllTextAsync(
                absoluteFilePath,
                BlazorWasmEmptyFacts.GetImportsRazorContents(cSharpProjectAbsoluteFilePath.FileNameNoExtension));
        }
        
        // IndexHtml
        {
            var absoluteFilePath = environmentProvider.JoinPaths(
                parentDirectoryOfProjectAbsoluteFilePathString,
                BlazorWasmEmptyFacts.INDEX_HTML_RELATIVE_FILE_PATH);

            await fileSystemProvider.File.WriteAllTextAsync(
                absoluteFilePath,
                BlazorWasmEmptyFacts.GetIndexHtmlContents(cSharpProjectAbsoluteFilePath.FileNameNoExtension));
        }
        
        // IndexRazor
        {
            var absoluteFilePath = environmentProvider.JoinPaths(
                parentDirectoryOfProjectAbsoluteFilePathString,
                BlazorWasmEmptyFacts.INDEX_RAZOR_RELATIVE_FILE_PATH);

            await fileSystemProvider.File.WriteAllTextAsync(
                absoluteFilePath,
                BlazorWasmEmptyFacts.GetIndexRazorContents(cSharpProjectAbsoluteFilePath.FileNameNoExtension));
        }
        
        // LaunchSettingsJson
        {
            var absoluteFilePath = environmentProvider.JoinPaths(
                parentDirectoryOfProjectAbsoluteFilePathString,
                BlazorWasmEmptyFacts.LAUNCH_SETTINGS_JSON_RELATIVE_FILE_PATH);

            await fileSystemProvider.File.WriteAllTextAsync(
                absoluteFilePath,
                BlazorWasmEmptyFacts.GetLaunchSettingsJsonContents(cSharpProjectAbsoluteFilePath.FileNameNoExtension));
        }
        
        // MainLayoutRazor
        {
            var absoluteFilePath = environmentProvider.JoinPaths(
                parentDirectoryOfProjectAbsoluteFilePathString,
                BlazorWasmEmptyFacts.MAIN_LAYOUT_RAZOR_RELATIVE_FILE_PATH);

            await fileSystemProvider.File.WriteAllTextAsync(
                absoluteFilePath,
                BlazorWasmEmptyFacts.GetMainLayoutRazorContents(cSharpProjectAbsoluteFilePath.FileNameNoExtension));
        }
        
        // ProgramCs
        {
            var absoluteFilePath = environmentProvider.JoinPaths(
                parentDirectoryOfProjectAbsoluteFilePathString,
                BlazorWasmEmptyFacts.PROGRAM_CS_RELATIVE_FILE_PATH);

            await fileSystemProvider.File.WriteAllTextAsync(
                absoluteFilePath,
                BlazorWasmEmptyFacts.GetProgramCsContents(cSharpProjectAbsoluteFilePath.FileNameNoExtension));
        }
    }

    private static async Task HandleBlazorServerSideEmptyProjectTemplateAsync(
        string cSharpProjectAbsoluteFilePathString,
        IFileSystemProvider fileSystemProvider,
        IEnvironmentProvider environmentProvider)
    {
        var cSharpProjectAbsoluteFilePath = new AbsoluteFilePath(cSharpProjectAbsoluteFilePathString, false, environmentProvider);
        var parentDirectoryOfProject = cSharpProjectAbsoluteFilePath.ParentDirectory;

        if (parentDirectoryOfProject is null)
            throw new NotImplementedException();

        var parentDirectoryOfProjectAbsoluteFilePathString = parentDirectoryOfProject.GetAbsoluteFilePathString();

        // AppRazor
        {
            var absoluteFilePath = environmentProvider.JoinPaths(
                parentDirectoryOfProjectAbsoluteFilePathString,
                BlazorServerEmptyFacts.APP_RAZOR_RELATIVE_FILE_PATH);

            await fileSystemProvider.File.WriteAllTextAsync(
                absoluteFilePath,
                BlazorServerEmptyFacts.GetAppRazorContents(cSharpProjectAbsoluteFilePath.FileNameNoExtension));
        }

        // AppSettingsDevelopmentJson
        {
            var absoluteFilePath = environmentProvider.JoinPaths(
                parentDirectoryOfProjectAbsoluteFilePathString,
                BlazorServerEmptyFacts.APP_SETTINGS_DEVELOPMENT_JSON_RELATIVE_FILE_PATH);

            await fileSystemProvider.File.WriteAllTextAsync(
                absoluteFilePath,
                BlazorServerEmptyFacts.GetAppSettingsDevelopmentJsonContents(cSharpProjectAbsoluteFilePath.FileNameNoExtension));
        }

        // AppSettingsJson
        {
            var absoluteFilePath = environmentProvider.JoinPaths(
                parentDirectoryOfProjectAbsoluteFilePathString,
                BlazorServerEmptyFacts.APP_SETTINGS_JSON_RELATIVE_FILE_PATH);

            await fileSystemProvider.File.WriteAllTextAsync(
                absoluteFilePath,
                BlazorServerEmptyFacts.GetAppSettingsJsonContents(cSharpProjectAbsoluteFilePath.FileNameNoExtension));
        }

        // Csproj
        {
            await fileSystemProvider.File.WriteAllTextAsync(
                cSharpProjectAbsoluteFilePathString,
                BlazorServerEmptyFacts.GetCsprojContents(cSharpProjectAbsoluteFilePath.FileNameNoExtension));
        }

        // HostCshtml
        {
            var absoluteFilePath = environmentProvider.JoinPaths(
                parentDirectoryOfProjectAbsoluteFilePathString,
                BlazorServerEmptyFacts.HOST_CSHTML_RELATIVE_FILE_PATH);

            await fileSystemProvider.File.WriteAllTextAsync(
                absoluteFilePath,
                BlazorServerEmptyFacts.GetHostCshtmlContents(cSharpProjectAbsoluteFilePath.FileNameNoExtension));
        }

        // ImportsRazor
        {
            var absoluteFilePath = environmentProvider.JoinPaths(
                parentDirectoryOfProjectAbsoluteFilePathString,
                BlazorServerEmptyFacts.IMPORTS_RAZOR_RELATIVE_FILE_PATH);

            await fileSystemProvider.File.WriteAllTextAsync(
                absoluteFilePath,
                BlazorServerEmptyFacts.GetImportsRazorContents(cSharpProjectAbsoluteFilePath.FileNameNoExtension));
        }

        // IndexRazor
        {
            var absoluteFilePath = environmentProvider.JoinPaths(
                parentDirectoryOfProjectAbsoluteFilePathString,
                BlazorServerEmptyFacts.INDEX_RAZOR_RELATIVE_FILE_PATH);

            await fileSystemProvider.File.WriteAllTextAsync(
                absoluteFilePath,
                BlazorServerEmptyFacts.GetIndexRazorContents(cSharpProjectAbsoluteFilePath.FileNameNoExtension));
        }

        // LaunchSettingsJson
        {
            var absoluteFilePath = environmentProvider.JoinPaths(
                parentDirectoryOfProjectAbsoluteFilePathString,
                BlazorServerEmptyFacts.LAUNCH_SETTINGS_JSON_RELATIVE_FILE_PATH);

            await fileSystemProvider.File.WriteAllTextAsync(
                absoluteFilePath,
                BlazorServerEmptyFacts.GetLaunchSettingsJsonContents(cSharpProjectAbsoluteFilePath.FileNameNoExtension));
        }

        // MainLayoutRazor
        {
            var absoluteFilePath = environmentProvider.JoinPaths(
                parentDirectoryOfProjectAbsoluteFilePathString,
                BlazorServerEmptyFacts.MAIN_LAYOUT_RAZOR_RELATIVE_FILE_PATH);

            await fileSystemProvider.File.WriteAllTextAsync(
                absoluteFilePath,
                BlazorServerEmptyFacts.GetMainLayoutRazorContents(cSharpProjectAbsoluteFilePath.FileNameNoExtension));
        }

        // ProgramCs
        {
            var absoluteFilePath = environmentProvider.JoinPaths(
                parentDirectoryOfProjectAbsoluteFilePathString,
                BlazorServerEmptyFacts.PROGRAM_CS_RELATIVE_FILE_PATH);

            await fileSystemProvider.File.WriteAllTextAsync(
                absoluteFilePath,
                BlazorServerEmptyFacts.GetProgramCsContents(cSharpProjectAbsoluteFilePath.FileNameNoExtension));
        }

        // SiteCss
        {
            var absoluteFilePath = environmentProvider.JoinPaths(
                parentDirectoryOfProjectAbsoluteFilePathString,
                BlazorServerEmptyFacts.SITE_CSS_RELATIVE_FILE_PATH);

            await fileSystemProvider.File.WriteAllTextAsync(
                absoluteFilePath,
                BlazorServerEmptyFacts.GetSiteCssContents(cSharpProjectAbsoluteFilePath.FileNameNoExtension));
        }
    }

    private static async Task HandleClassLibProjectTemplateAsync(
        string cSharpProjectAbsoluteFilePathString,
        IFileSystemProvider fileSystemProvider,
        IEnvironmentProvider environmentProvider)
    {
        var cSharpProjectAbsoluteFilePath = new AbsoluteFilePath(cSharpProjectAbsoluteFilePathString, false, environmentProvider);
        var parentDirectoryOfProject = cSharpProjectAbsoluteFilePath.ParentDirectory;

        if (parentDirectoryOfProject is null)
            throw new NotImplementedException();

        var parentDirectoryOfProjectAbsoluteFilePathString = parentDirectoryOfProject.GetAbsoluteFilePathString();

        // Class1Cs
        {
            var absoluteFilePath = environmentProvider.JoinPaths(
                parentDirectoryOfProjectAbsoluteFilePathString,
                ClassLibFacts.CLASS_1_CS_RELATIVE_FILE_PATH);

            await fileSystemProvider.File.WriteAllTextAsync(
                absoluteFilePath,
                ClassLibFacts.GetClass1CsContents(cSharpProjectAbsoluteFilePath.FileNameNoExtension));
        }

        // Csproj
        {
            await fileSystemProvider.File.WriteAllTextAsync(
                cSharpProjectAbsoluteFilePathString,
                ClassLibFacts.GetCsprojContents(cSharpProjectAbsoluteFilePath.FileNameNoExtension));
        }
    }

    private static async Task HandleRazorLibProjectTemplateAsync(
        string cSharpProjectAbsoluteFilePathString,
        IFileSystemProvider fileSystemProvider,
        IEnvironmentProvider environmentProvider)
    {
        var cSharpProjectAbsoluteFilePath = new AbsoluteFilePath(cSharpProjectAbsoluteFilePathString, false, environmentProvider);
        var parentDirectoryOfProject = cSharpProjectAbsoluteFilePath.ParentDirectory;

        if (parentDirectoryOfProject is null)
            throw new NotImplementedException();

        var parentDirectoryOfProjectAbsoluteFilePathString = parentDirectoryOfProject.GetAbsoluteFilePathString();

        // Component1Razor
        {
            var absoluteFilePath = environmentProvider.JoinPaths(
                parentDirectoryOfProjectAbsoluteFilePathString,
                RazorClassLibFacts.COMPONENT_1_RAZOR_RELATIVE_FILE_PATH);

            await fileSystemProvider.File.WriteAllTextAsync(
                absoluteFilePath,
                RazorClassLibFacts.GetComponent1RazorContents(cSharpProjectAbsoluteFilePath.FileNameNoExtension));
        }

        // Component1RazorCss
        {
            var absoluteFilePath = environmentProvider.JoinPaths(
                parentDirectoryOfProjectAbsoluteFilePathString,
                RazorClassLibFacts.COMPONENT_1_RAZOR_CSS_RELATIVE_FILE_PATH);

            await fileSystemProvider.File.WriteAllTextAsync(
                absoluteFilePath,
                RazorClassLibFacts.GetComponent1RazorCssContents(cSharpProjectAbsoluteFilePath.FileNameNoExtension));
        }

        // Csproj
        {
            await fileSystemProvider.File.WriteAllTextAsync(
                cSharpProjectAbsoluteFilePathString,
                RazorClassLibFacts.GetCsprojContents(cSharpProjectAbsoluteFilePath.FileNameNoExtension));
        }

        // ExampleJsInteropCs
        {
            var absoluteFilePath = environmentProvider.JoinPaths(
                parentDirectoryOfProjectAbsoluteFilePathString,
                RazorClassLibFacts.EXAMPLE_JS_INTEROP_CS_RELATIVE_FILE_PATH);

            await fileSystemProvider.File.WriteAllTextAsync(
                absoluteFilePath,
                RazorClassLibFacts.GetExampleJsInteropCsContents(cSharpProjectAbsoluteFilePath.FileNameNoExtension));
        }

        // ExampleJsInteropJs
        {
            var absoluteFilePath = environmentProvider.JoinPaths(
                parentDirectoryOfProjectAbsoluteFilePathString,
                RazorClassLibFacts.EXAMPLE_JS_INTEROP_JS_RELATIVE_FILE_PATH);

            await fileSystemProvider.File.WriteAllTextAsync(
                absoluteFilePath,
                RazorClassLibFacts.GetExampleJsInteropJsContents(cSharpProjectAbsoluteFilePath.FileNameNoExtension));
        }

        // ImportsRazor
        {
            var absoluteFilePath = environmentProvider.JoinPaths(
                parentDirectoryOfProjectAbsoluteFilePathString,
                RazorClassLibFacts.IMPORTS_RAZOR_RELATIVE_FILE_PATH);

            await fileSystemProvider.File.WriteAllTextAsync(
                absoluteFilePath,
                RazorClassLibFacts.GetImportsRazorContents(cSharpProjectAbsoluteFilePath.FileNameNoExtension));
        }
    }

    private static async Task HandleConsoleAppProjectTemplateAsync(
        string cSharpProjectAbsoluteFilePathString,
        IFileSystemProvider fileSystemProvider,
        IEnvironmentProvider environmentProvider)
    {
        var cSharpProjectAbsoluteFilePath = new AbsoluteFilePath(cSharpProjectAbsoluteFilePathString, false, environmentProvider);
        var parentDirectoryOfProject = cSharpProjectAbsoluteFilePath.ParentDirectory;

        if (parentDirectoryOfProject is null)
            throw new NotImplementedException();

        var parentDirectoryOfProjectAbsoluteFilePathString = parentDirectoryOfProject.GetAbsoluteFilePathString();

        // ProgramCs
        {
            var absoluteFilePath = environmentProvider.JoinPaths(
                parentDirectoryOfProjectAbsoluteFilePathString,
                ConsoleAppFacts.PROGRAM_CS_RELATIVE_FILE_PATH);

            await fileSystemProvider.File.WriteAllTextAsync(
                absoluteFilePath,
                ConsoleAppFacts.GetProgramCsContents(cSharpProjectAbsoluteFilePath.FileNameNoExtension));
        }

        // Csproj
        {
            await fileSystemProvider.File.WriteAllTextAsync(
                cSharpProjectAbsoluteFilePathString,
                ConsoleAppFacts.GetCsprojContents(cSharpProjectAbsoluteFilePath.FileNameNoExtension));
        }
    }

    private static async Task HandleXUnitProjectTemplateAsync(
        string cSharpProjectAbsoluteFilePathString,
        IFileSystemProvider fileSystemProvider,
        IEnvironmentProvider environmentProvider)
    {
        var cSharpProjectAbsoluteFilePath = new AbsoluteFilePath(cSharpProjectAbsoluteFilePathString, false, environmentProvider);
        var parentDirectoryOfProject = cSharpProjectAbsoluteFilePath.ParentDirectory;

        if (parentDirectoryOfProject is null)
            throw new NotImplementedException();

        var parentDirectoryOfProjectAbsoluteFilePathString = parentDirectoryOfProject.GetAbsoluteFilePathString();

        // Csproj
        {
            await fileSystemProvider.File.WriteAllTextAsync(
                cSharpProjectAbsoluteFilePathString,
                XUnitFacts.GetCsprojContents(cSharpProjectAbsoluteFilePath.FileNameNoExtension));
        }

        // UnitTest1Cs
        {
            var absoluteFilePath = environmentProvider.JoinPaths(
                parentDirectoryOfProjectAbsoluteFilePathString,
                XUnitFacts.UNIT_TEST_1_CS_RELATIVE_FILE_PATH);

            await fileSystemProvider.File.WriteAllTextAsync(
                absoluteFilePath,
                XUnitFacts.GetUnitTest1CsContents(cSharpProjectAbsoluteFilePath.FileNameNoExtension));
        }
        
        // UsingsCs
        {
            var absoluteFilePath = environmentProvider.JoinPaths(
                parentDirectoryOfProjectAbsoluteFilePathString,
                XUnitFacts.USINGS_CS_RELATIVE_FILE_PATH);

            await fileSystemProvider.File.WriteAllTextAsync(
                absoluteFilePath,
                XUnitFacts.GetUsingsCsContents(cSharpProjectAbsoluteFilePath.FileNameNoExtension));
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
