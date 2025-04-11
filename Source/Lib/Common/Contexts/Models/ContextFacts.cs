using Luthetus.Common.RazorLib.Keymaps.Models;
using Luthetus.Common.RazorLib.Keys.Models;

namespace Luthetus.Common.RazorLib.Contexts.Models;

public static class ContextFacts
{
	public static readonly ContextRecord GlobalContext = new(
        Key<ContextRecord>.NewKey(),
        "Global",
        "global",
        new Keymap(Key<Keymap>.NewKey(), "Global"));

    public static readonly ContextRecord ActiveContextsContext = new(
        Key<ContextRecord>.NewKey(),
        "Active Contexts",
        "active-contexts",
        IKeymap.Empty);

	public static readonly ContextRecord FindAllReferencesContext = new(
        Key<ContextRecord>.NewKey(),
        "Find All References",
        "find-all-references",
        IKeymap.Empty);

    public static readonly ContextRecord FolderExplorerContext = new(
        Key<ContextRecord>.NewKey(),
        "Folder Explorer",
        "folder-explorer",
        IKeymap.Empty);

    public static readonly ContextRecord SolutionExplorerContext = new(
        Key<ContextRecord>.NewKey(),
        "Solution Explorer",
        "solution-explorer",
        IKeymap.Empty);

    public static readonly ContextRecord CompilerServiceExplorerContext = new(
        Key<ContextRecord>.NewKey(),
        "Compiler Service Explorer",
        "compiler-service-explorer",
        IKeymap.Empty);
    
    public static readonly ContextRecord CompilerServiceEditorContext = new(
        Key<ContextRecord>.NewKey(),
        "Compiler Service Editor",
        "compiler-service-editor",
        IKeymap.Empty);

	public static readonly ContextRecord TestExplorerContext = new(
        Key<ContextRecord>.NewKey(),
        "Test Explorer",
        "test-explorer",
        IKeymap.Empty);
    
    public static readonly ContextRecord CSharpReplContext = new(
        Key<ContextRecord>.NewKey(),
        "C# REPL",
        "c-sharp-repl",
        IKeymap.Empty);

    public static readonly ContextRecord BackgroundServicesContext = new(
        Key<ContextRecord>.NewKey(),
        "Background Services",
        "background-services",
        IKeymap.Empty);

    public static readonly ContextRecord DialogDisplayContext = new(
        Key<ContextRecord>.NewKey(),
        "Dialog Display",
        "dialog-display",
        IKeymap.Empty);

    public static readonly ContextRecord MainLayoutHeaderContext = new(
        Key<ContextRecord>.NewKey(),
        "MainLayout Header",
        "main-layout-header",
        IKeymap.Empty);

    public static readonly ContextRecord MainLayoutFooterContext = new(
        Key<ContextRecord>.NewKey(),
        "MainLayout Footer",
        "main-layout-footer",
        IKeymap.Empty);

    public static readonly ContextRecord EditorContext = new(
        Key<ContextRecord>.NewKey(),
        "Editor",
        "editor",
        IKeymap.Empty);

    public static readonly ContextRecord TextEditorContext = new(
        Key<ContextRecord>.NewKey(),
        "Text Editor",
        "text-editor",
        IKeymap.Empty);

    public static readonly ContextRecord ErrorListContext = new(
        Key<ContextRecord>.NewKey(),
        "Error List",
        "error-list",
        IKeymap.Empty);

    public static readonly ContextRecord OutputContext = new(
        Key<ContextRecord>.NewKey(),
        "Output",
        "output",
        IKeymap.Empty);

    public static readonly ContextRecord NuGetPackageManagerContext = new(
        Key<ContextRecord>.NewKey(),
        "NuGetPackageManager",
        "nu-get-package-manager",
        IKeymap.Empty);

    public static readonly ContextRecord GitContext = new(
        Key<ContextRecord>.NewKey(),
        "Git",
        "git",
        IKeymap.Empty);
    
    public static readonly ContextRecord TerminalContext = new(
        Key<ContextRecord>.NewKey(),
        "Terminal",
        "terminal",
        IKeymap.Empty);
        
    public static readonly ContextRecord NotificationContext = new(
        Key<ContextRecord>.NewKey(),
        "Notification",
        "notification",
        IKeymap.Empty);
    
    public static readonly ContextRecord DialogContext = new(
        Key<ContextRecord>.NewKey(),
        "Dialog",
        "dialog",
        IKeymap.Empty);
        
    public static readonly ContextRecord WidgetContext = new(
        Key<ContextRecord>.NewKey(),
        "Widget",
        "widget",
        IKeymap.Empty);
    
    public static readonly ContextRecord DropdownContext = new(
        Key<ContextRecord>.NewKey(),
        "Dropdown",
        "dropdown",
        IKeymap.Empty);

    public static readonly IReadOnlyList<ContextRecord> AllContextsList = new List<ContextRecord>()
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
        WidgetContext,
        DropdownContext,
    };
    
    /// <summary>
    /// Used when repositioning a dropdown so that it appears on screen.
    /// </summary>
    public static string RootHtmlElementId { get; set; } = GlobalContext.ContextElementId;
}