using Luthetus.Ide.ClassLib.KeymapCase;
using System.Collections.Immutable;

namespace Luthetus.Ide.ClassLib.Context;

public static class ContextFacts
{
    public static readonly ContextRecord GlobalContext = new(
        ContextKey.NewContextKey(),
        "Global",
        "global",
        Keymap.Empty);

    public static readonly ContextRecord FolderExplorerContext = new(
        ContextKey.NewContextKey(),
        "Folder Explorer",
        "folder-explorer",
        Keymap.Empty);
    
    public static readonly ContextRecord SolutionExplorerContext = new(
        ContextKey.NewContextKey(),
        "Solution Explorer",
        "solution-explorer",
        Keymap.Empty);
    
    public static readonly ContextRecord CompilerServiceExplorerContext = new(
        ContextKey.NewContextKey(),
        "Compiler Service Explorer",
        "compiler-service-explorer",
        Keymap.Empty);
    
    public static readonly ContextRecord BackgroundServicesContext = new(
        ContextKey.NewContextKey(),
        "Background Services",
        "background-services",
        Keymap.Empty);

    public static readonly ContextRecord DialogDisplayContext = new(
        ContextKey.NewContextKey(),
        "Dialog Display",
        "dialog-display",
        Keymap.Empty);

    public static readonly ContextRecord MainLayoutHeaderContext = new(
        ContextKey.NewContextKey(),
        "MainLayout Header",
        "main-layout-header",
        Keymap.Empty);

    public static readonly ContextRecord MainLayoutFooterContext = new(
        ContextKey.NewContextKey(),
        "MainLayout Footer",
        "main-layout-footer",
        Keymap.Empty);

    public static readonly ContextRecord EditorContext = new(
        ContextKey.NewContextKey(),
        "Editor",
        "editor",
        Keymap.Empty);

    public static readonly ContextRecord TextEditorContext = new(
        ContextKey.NewContextKey(),
        "Text Editor",
        "text-editor",
        Keymap.Empty);

    public static readonly ContextRecord TerminalContext = new(
        ContextKey.NewContextKey(),
        "Terminal",
        "terminal",
        Keymap.Empty);

    public static readonly ContextRecord NuGetPackageManagerContext = new(
        ContextKey.NewContextKey(),
        "NuGetPackageManager",
        "nu-get-package-manager",
        Keymap.Empty);

    public static readonly ContextRecord GitContext = new(
        ContextKey.NewContextKey(),
        "Git",
        "git",
        Keymap.Empty);

    public static readonly ImmutableArray<ContextRecord> ContextRecords = new[]
    {
        GlobalContext,
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