using Luthetus.Common.RazorLib.FileSystem.Interfaces;
using Luthetus.Common.RazorLib.Namespaces;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Luthetus.Ide.RazorLib.CSharpProjectForm.Facts;

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

        RazorLibProjectTemplate = new ProjectTemplate(
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
            ClassLibProjectTemplate,
            RazorLibProjectTemplate,
            ConsoleAppProjectTemplate,
            XUnitProjectTemplate,
        }.ToImmutableArray();
    }

    public static ProjectTemplate BlazorWasmEmptyProjectTemplate { get; }
    public static ProjectTemplate BlazorServerSideEmptyProjectTemplate { get; }
    public static ProjectTemplate ClassLibProjectTemplate { get; }
    public static ProjectTemplate RazorLibProjectTemplate { get; }
    public static ProjectTemplate ConsoleAppProjectTemplate { get; }
    public static ProjectTemplate XUnitProjectTemplate { get; }

    public static ImmutableArray<ProjectTemplate> WebsiteProjectTemplatesContainer { get; }

    public static async Task HandleNewCSharpProjectAsync(
        string projectTemplateShortName,
        string cSharpProjectName,
        string optionalParameters,
        string parentDirectoryName,
        NamespacePath solutionNamespacePath,
        string cSharpProjectAbsoluteFilePathString,
        IFileSystemProvider fileSystemProvider)
    {
        if (projectTemplateShortName == BlazorWasmEmptyProjectTemplate.ShortName)
            await HandleBlazorWasmEmptyProjectTemplateAsync(projectTemplateShortName, cSharpProjectName, optionalParameters, parentDirectoryName, solutionNamespacePath, cSharpProjectAbsoluteFilePathString, fileSystemProvider);
        else if (projectTemplateShortName == BlazorServerSideEmptyProjectTemplate.ShortName)
            await HandleBlazorServerSideEmptyProjectTemplateAsync(projectTemplateShortName, cSharpProjectName, optionalParameters, parentDirectoryName, solutionNamespacePath, cSharpProjectAbsoluteFilePathString, fileSystemProvider);
        else if (projectTemplateShortName == ClassLibProjectTemplate.ShortName)
            await HandleClassLibProjectTemplateAsync(projectTemplateShortName, cSharpProjectName, optionalParameters, parentDirectoryName, solutionNamespacePath, cSharpProjectAbsoluteFilePathString, fileSystemProvider);
        else if (projectTemplateShortName == RazorLibProjectTemplate.ShortName)
            await HandleRazorLibProjectTemplateAsync(projectTemplateShortName, cSharpProjectName, optionalParameters, parentDirectoryName, solutionNamespacePath, cSharpProjectAbsoluteFilePathString, fileSystemProvider);
        else if (projectTemplateShortName == ConsoleAppProjectTemplate.ShortName)
            await HandleConsoleAppProjectTemplateAsync(projectTemplateShortName, cSharpProjectName, optionalParameters, parentDirectoryName, solutionNamespacePath, cSharpProjectAbsoluteFilePathString, fileSystemProvider);
        else if (projectTemplateShortName == XUnitProjectTemplate.ShortName)
            await HandleXUnitProjectTemplateAsync(projectTemplateShortName, cSharpProjectName, optionalParameters, parentDirectoryName, solutionNamespacePath, cSharpProjectAbsoluteFilePathString, fileSystemProvider);
        else
            throw new NotImplementedException($"The {nameof(ProjectTemplate.ShortName)}: '{projectTemplateShortName}' was not recognized.");
    }

    private static async Task HandleBlazorWasmEmptyProjectTemplateAsync(string projectTemplateShortName, string cSharpProjectName, string optionalParameters, string parentDirectoryName, NamespacePath solutionNamespacePath, string cSharpProjectAbsoluteFilePathString, IFileSystemProvider fileSystemProvider)
    {
        await fileSystemProvider.File.WriteAllTextAsync(
            cSharpProjectAbsoluteFilePathString,
            BlazorWasmEmptyFacts.CSPROJ_CONTENTS);
    }

    private static async Task HandleBlazorServerSideEmptyProjectTemplateAsync(string projectTemplateShortName, string cSharpProjectName, string optionalParameters, string parentDirectoryName, NamespacePath solutionNamespacePath, string cSharpProjectAbsoluteFilePathString, IFileSystemProvider fileSystemProvider)
    {
        throw new NotImplementedException();
    }

    private static async Task HandleClassLibProjectTemplateAsync(string projectTemplateShortName, string cSharpProjectName, string optionalParameters, string parentDirectoryName, NamespacePath solutionNamespacePath, string cSharpProjectAbsoluteFilePathString, IFileSystemProvider fileSystemProvider)
    {
        throw new NotImplementedException();
    }

    private static async Task HandleRazorLibProjectTemplateAsync(string projectTemplateShortName, string cSharpProjectName, string optionalParameters, string parentDirectoryName, NamespacePath solutionNamespacePath, string cSharpProjectAbsoluteFilePathString, IFileSystemProvider fileSystemProvider)
    {
        throw new NotImplementedException();
    }

    private static async Task HandleConsoleAppProjectTemplateAsync(string projectTemplateShortName, string cSharpProjectName, string optionalParameters, string parentDirectoryName, NamespacePath solutionNamespacePath, string cSharpProjectAbsoluteFilePathString, IFileSystemProvider fileSystemProvider)
    {
        throw new NotImplementedException();
    }

    private static async Task HandleXUnitProjectTemplateAsync(string projectTemplateShortName, string cSharpProjectName, string optionalParameters, string parentDirectoryName, NamespacePath solutionNamespacePath, string cSharpProjectAbsoluteFilePathString, IFileSystemProvider fileSystemProvider)
    {
        throw new NotImplementedException();
    }
}
