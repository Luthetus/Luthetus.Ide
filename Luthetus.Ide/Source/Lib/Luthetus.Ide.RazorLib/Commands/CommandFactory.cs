using Fluxor;
using Luthetus.Common.RazorLib.Commands.Models;
using Luthetus.Common.RazorLib.Contexts.Models;
using Luthetus.Common.RazorLib.Keymaps.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Panels.States;
using Microsoft.JSInterop;

namespace Luthetus.Ide.RazorLib.Commands;

public class CommandFactory : ICommandFactory
{
    private readonly IJSRuntime _jsRuntime;
    private readonly IState<PanelsState> _panelsStateWrap;
    private readonly IDispatcher _dispatcher;

    public CommandFactory(
        IJSRuntime jsRuntime,
        IState<PanelsState> panelsStateWrap,
        IDispatcher dispatcher)
    {
        _jsRuntime = jsRuntime;
        _panelsStateWrap = panelsStateWrap;
        _dispatcher = dispatcher;
    }

    public void Initialize()
    {
        // ActiveContextsContext
        {
            _ = ContextFacts.GlobalContext.Keymap.Map.TryAdd(
                new KeymapArgument("KeyA", false, true, true, Key<KeymapLayer>.Empty),
                ConstructFocusContextElementCommand(
                    ContextFacts.ActiveContextsContext, "Focus: ActiveContexts", "focus-active-contexts"));
        }
        // BackgroundServicesContext
        {
            _ = ContextFacts.GlobalContext.Keymap.Map.TryAdd(
                new KeymapArgument("KeyB", false, true, true, Key<KeymapLayer>.Empty),
                ConstructFocusContextElementCommand(
                    ContextFacts.BackgroundServicesContext, "Focus: BackgroundServices", "focus-background-services"));
        }
        // CompilerServiceExplorerContext
        {
            _ = ContextFacts.GlobalContext.Keymap.Map.TryAdd(
                new KeymapArgument("KeyC", false, true, true, Key<KeymapLayer>.Empty),
                ConstructFocusContextElementCommand(
                    ContextFacts.CompilerServiceExplorerContext, "Focus: CompilerServiceExplorer", "focus-compiler-service-explorer"));
        }
        // DialogDisplayContext
        {
            _ = ContextFacts.GlobalContext.Keymap.Map.TryAdd(
                new KeymapArgument("KeyD", false, true, true, Key<KeymapLayer>.Empty),
                ConstructFocusContextElementCommand(
                    ContextFacts.DialogDisplayContext, "Focus: DialogDisplay", "focus-dialog-display"));
        }
        // EditorContext
        {
            _ = ContextFacts.GlobalContext.Keymap.Map.TryAdd(
                new KeymapArgument("KeyE", false, true, true, Key<KeymapLayer>.Empty),
                ConstructFocusContextElementCommand(
                    ContextFacts.EditorContext, "Focus: Editor", "focus-editor"));
        }
        // FolderExplorerContext
        {
            _ = ContextFacts.GlobalContext.Keymap.Map.TryAdd(
                new KeymapArgument("KeyF", false, true, true, Key<KeymapLayer>.Empty),
                ConstructFocusContextElementCommand(
                    ContextFacts.FolderExplorerContext, "Focus: FolderExplorer", "focus-folder-explorer"));
        }
        // GitContext
        {
            _ = ContextFacts.GlobalContext.Keymap.Map.TryAdd(
                new KeymapArgument("KeyG", false, true, true, Key<KeymapLayer>.Empty),
                ConstructFocusContextElementCommand(
                    ContextFacts.GitContext, "Focus: Git", "focus-git"));
        }
        // GlobalContext
        {
            _ = ContextFacts.GlobalContext.Keymap.Map.TryAdd(
                new KeymapArgument("KeyG", false, true, true, Key<KeymapLayer>.Empty),
                ConstructFocusContextElementCommand(
                    ContextFacts.GlobalContext, "Focus: Global", "focus-global"));
        }
        // MainLayoutFooterContext
        {
            _ = ContextFacts.GlobalContext.Keymap.Map.TryAdd(
                new KeymapArgument("KeyF", false, true, true, Key<KeymapLayer>.Empty),
                ConstructFocusContextElementCommand(
                    ContextFacts.MainLayoutFooterContext, "Focus: Footer", "focus-footer"));
        }
        // MainLayoutHeaderContext
        {
            _ = ContextFacts.GlobalContext.Keymap.Map.TryAdd(
                new KeymapArgument("KeyH", false, true, true, Key<KeymapLayer>.Empty),
                ConstructFocusContextElementCommand(
                    ContextFacts.MainLayoutHeaderContext, "Focus: Header", "focus-header"));
        }
        // NuGetPackageManagerContext
        {
            _ = ContextFacts.GlobalContext.Keymap.Map.TryAdd(
                new KeymapArgument("KeyN", false, true, true, Key<KeymapLayer>.Empty),
                ConstructFocusContextElementCommand(
                    ContextFacts.NuGetPackageManagerContext, "Focus: NuGetPackageManager", "focus-nu-get-package-manager"));
        }
        // CSharpReplContext
        {
            _ = ContextFacts.GlobalContext.Keymap.Map.TryAdd(
                new KeymapArgument("KeyR", false, true, true, Key<KeymapLayer>.Empty),
                ConstructFocusContextElementCommand(
                    ContextFacts.SolutionExplorerContext, "Focus: C# REPL", "focus-c-sharp-repl"));
        }
        // SolutionExplorerContext
        {
            _ = ContextFacts.GlobalContext.Keymap.Map.TryAdd(
                new KeymapArgument("KeyS", false, true, true, Key<KeymapLayer>.Empty),
                ConstructFocusContextElementCommand(
                    ContextFacts.SolutionExplorerContext, "Focus: SolutionExplorer", "focus-solution-explorer"));
        }
        // TerminalContext
        {
            _ = ContextFacts.GlobalContext.Keymap.Map.TryAdd(
                new KeymapArgument("KeyT", false, true, true, Key<KeymapLayer>.Empty),
                ConstructFocusContextElementCommand(
                    ContextFacts.TerminalContext, "Focus: Terminal", "focus-terminal"));
        }
        // TextEditorContext
        {
            _ = ContextFacts.GlobalContext.Keymap.Map.TryAdd(
                new KeymapArgument("KeyT", false, true, true, Key<KeymapLayer>.Empty),
                ConstructFocusContextElementCommand(
                    ContextFacts.TextEditorContext, "Focus: TextEditor", "focus-text-editor"));
        }
    }

    public CommandNoType ConstructFocusContextElementCommand(
        ContextRecord contextRecord,
        string displayName,
        string internalIdentifier)
    {
        return new CommonCommand(
            displayName, internalIdentifier, false,
            async commandParameter =>
            {
                var success = await TrySetFocus();

                if (!success)
                {
                    _dispatcher.Dispatch(new PanelsState.SetPanelTabAsActiveByContextRecordKeyAction(
                        contextRecord.ContextKey));

                    _ = await TrySetFocus();
                }
            });

        async Task<bool> TrySetFocus()
        {
            return await _jsRuntime.InvokeAsync<bool>(
                "luthetusIde.tryFocusHtmlElementById",
                contextRecord.ContextElementId);
        }
    }
}
