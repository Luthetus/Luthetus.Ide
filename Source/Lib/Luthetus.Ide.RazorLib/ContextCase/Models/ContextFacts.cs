using Luthetus.Ide.RazorLib.KeymapCase.Models;
using System.Collections.Immutable;

namespace Luthetus.Ide.RazorLib.ContextCase.Models;

public static class ContextFacts
{
    public static readonly ContextRecord GlobalContext = new(
        ContextKey.NewKey(),
        "Global",
        "global",
        Keymap.Empty);

    public static readonly ContextRecord ActiveContextsContext = new(
        ContextKey.NewKey(),
        "Active Contexts",
        "active-contexts",
        Keymap.Empty);

    public static readonly ContextRecord FolderExplorerContext = new(
        ContextKey.NewKey(),
        "Folder Explorer",
        "folder-explorer",
        Keymap.Empty);

    public static readonly ContextRecord SolutionExplorerContext = new(
        ContextKey.NewKey(),
        "Solution Explorer",
        "solution-explorer",
        Keymap.Empty);

    public static readonly ContextRecord CompilerServiceExplorerContext = new(
        ContextKey.NewKey(),
        "Compiler Service Explorer",
        "compiler-service-explorer",
        Keymap.Empty);

    public static readonly ContextRecord BackgroundServicesContext = new(
        ContextKey.NewKey(),
        "Background Services",
        "background-services",
        Keymap.Empty);

    public static readonly ContextRecord DialogDisplayContext = new(
        ContextKey.NewKey(),
        "Dialog Display",
        "dialog-display",
        Keymap.Empty);

    public static readonly ContextRecord MainLayoutHeaderContext = new(
        ContextKey.NewKey(),
        "MainLayout Header",
        "main-layout-header",
        Keymap.Empty);

    public static readonly ContextRecord MainLayoutFooterContext = new(
        ContextKey.NewKey(),
        "MainLayout Footer",
        "main-layout-footer",
        Keymap.Empty);

    public static readonly ContextRecord EditorContext = new(
        ContextKey.NewKey(),
        "Editor",
        "editor",
        Keymap.Empty);

    public static readonly ContextRecord TextEditorContext = new(
        ContextKey.NewKey(),
        "Text Editor",
        "text-editor",
        Keymap.Empty);

    public static readonly ContextRecord TerminalContext = new(
        ContextKey.NewKey(),
        "Terminal",
        "terminal",
        Keymap.Empty);

    public static readonly ContextRecord NuGetPackageManagerContext = new(
        ContextKey.NewKey(),
        "NuGetPackageManager",
        "nu-get-package-manager",
        Keymap.Empty);

    public static readonly ContextRecord GitContext = new(
        ContextKey.NewKey(),
        "Git",
        "git",
        Keymap.Empty);

    public static readonly ImmutableArray<ContextRecord> ContextRecords = new[]
    {
        GlobalContext,
        ActiveContextsContext,
        FolderExplorerContext,
        SolutionExplorerContext,
        CompilerServiceExplorerContext,
        BackgroundServicesContext,
        DialogDisplayContext,
        MainLayoutHeaderContext,
        MainLayoutFooterContext,
        EditorContext,
        TextEditorContext,
        TerminalContext,
        NuGetPackageManagerContext,
        GitContext,
    }.ToImmutableArray();
}