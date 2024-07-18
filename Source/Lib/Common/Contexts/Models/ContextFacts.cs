using System.Collections.Immutable;
using Luthetus.Common.RazorLib.Keymaps.Models;
using Luthetus.Common.RazorLib.Keys.Models;

namespace Luthetus.Common.RazorLib.Contexts.Models;

public static class ContextFacts
{
    public static readonly ContextRecord GlobalContext = new(
        Key<ContextRecord>.NewKey(),
        "Global",
        "global",
        Keymap.Empty);

    public static readonly ContextRecord ActiveContextsContext = new(
        Key<ContextRecord>.NewKey(),
        "Active Contexts",
        "active-contexts",
        Keymap.Empty);

    public static readonly ContextRecord FolderExplorerContext = new(
        Key<ContextRecord>.NewKey(),
        "Folder Explorer",
        "folder-explorer",
        Keymap.Empty);

    public static readonly ContextRecord SolutionExplorerContext = new(
        Key<ContextRecord>.NewKey(),
        "Solution Explorer",
        "solution-explorer",
        Keymap.Empty);

    public static readonly ContextRecord CompilerServiceExplorerContext = new(
        Key<ContextRecord>.NewKey(),
        "Compiler Service Explorer",
        "compiler-service-explorer",
        Keymap.Empty);
    
    public static readonly ContextRecord CompilerServiceEditorContext = new(
        Key<ContextRecord>.NewKey(),
        "Compiler Service Editor",
        "compiler-service-editor",
        Keymap.Empty);

	public static readonly ContextRecord TestExplorerContext = new(
        Key<ContextRecord>.NewKey(),
        "Test Explorer",
        "test-explorer",
        Keymap.Empty);
    
    public static readonly ContextRecord CSharpReplContext = new(
        Key<ContextRecord>.NewKey(),
        "C# REPL",
        "c-sharp-repl",
        Keymap.Empty);

    public static readonly ContextRecord BackgroundServicesContext = new(
        Key<ContextRecord>.NewKey(),
        "Background Services",
        "background-services",
        Keymap.Empty);

    public static readonly ContextRecord DialogDisplayContext = new(
        Key<ContextRecord>.NewKey(),
        "Dialog Display",
        "dialog-display",
        Keymap.Empty);

    public static readonly ContextRecord MainLayoutHeaderContext = new(
        Key<ContextRecord>.NewKey(),
        "MainLayout Header",
        "main-layout-header",
        Keymap.Empty);

    public static readonly ContextRecord MainLayoutFooterContext = new(
        Key<ContextRecord>.NewKey(),
        "MainLayout Footer",
        "main-layout-footer",
        Keymap.Empty);

    public static readonly ContextRecord EditorContext = new(
        Key<ContextRecord>.NewKey(),
        "Editor",
        "editor",
        Keymap.Empty);

    public static readonly ContextRecord TextEditorContext = new(
        Key<ContextRecord>.NewKey(),
        "Text Editor",
        "text-editor",
        Keymap.Empty);

    public static readonly ContextRecord ErrorListContext = new(
        Key<ContextRecord>.NewKey(),
        "Error List",
        "error-list",
        Keymap.Empty);

    public static readonly ContextRecord OutputContext = new(
        Key<ContextRecord>.NewKey(),
        "Output",
        "output",
        Keymap.Empty);

    public static readonly ContextRecord NuGetPackageManagerContext = new(
        Key<ContextRecord>.NewKey(),
        "NuGetPackageManager",
        "nu-get-package-manager",
        Keymap.Empty);

    public static readonly ContextRecord GitContext = new(
        Key<ContextRecord>.NewKey(),
        "Git",
        "git",
        Keymap.Empty);
    
    public static readonly ContextRecord TerminalContext = new(
        Key<ContextRecord>.NewKey(),
        "Terminal",
        "terminal",
        Keymap.Empty);
        
    public static readonly ContextRecord NotificationContext = new(
        Key<ContextRecord>.NewKey(),
        "Notification",
        "notification",
        Keymap.Empty);
    
    public static readonly ContextRecord DialogContext = new(
        Key<ContextRecord>.NewKey(),
        "Dialog",
        "dialog",
        Keymap.Empty);
    
    public static readonly ContextRecord DropdownContext = new(
        Key<ContextRecord>.NewKey(),
        "Dropdown",
        "dropdown",
        Keymap.Empty);

    public static readonly ImmutableArray<ContextRecord> AllContextsList = new[]
    {
        GlobalContext,
        ActiveContextsContext,
        FolderExplorerContext,
        SolutionExplorerContext,
        CompilerServiceExplorerContext,
        CompilerServiceEditorContext,
        CSharpReplContext,
        BackgroundServicesContext,
        DialogDisplayContext,
        MainLayoutHeaderContext,
        MainLayoutFooterContext,
        EditorContext,
        TextEditorContext,
		ErrorListContext,
        OutputContext,
        NuGetPackageManagerContext,
        GitContext,
        TerminalContext,
        NotificationContext,
        DialogContext,
        DropdownContext,
    }.ToImmutableArray();
}