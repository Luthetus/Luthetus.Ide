using Luthetus.Ide.ClassLib.Context;
using Luthetus.Ide.ClassLib.KeymapCase;
using Microsoft.JSInterop;

namespace Luthetus.Ide.ClassLib.CommandCase;

public class CommandFactory : ICommandFactory
{
    private readonly IJSRuntime _jsRuntime;
    private readonly object InitializeLock = new();
    
    private bool _isInitialized;

    public CommandFactory(IJSRuntime jsRuntime)
    {
        _jsRuntime = jsRuntime;
    }

    public void Initialize()
    {
        if (!_isInitialized)
        {
            lock (InitializeLock)
            {
                if (_isInitialized)
                    return;

                _isInitialized = true;

                InitializeGlobalContext();
            }
        }
    }

    public ICommand ConstructFocusContextElementCommand(ContextRecord contextRecord)
    {
        return new Command(
            async () =>
            {
                var success = await _jsRuntime.InvokeAsync<bool>(
                    "luthetusIde.tryFocusHtmlElementById",
                    contextRecord.ContextElementId);

                if (!success)
                {
                    // TODO: Add a 'reveal' Func to perhaps set an active panel tab if needed,
                    // then invoke javascript one last time to try again.
                }
            },
            "Focus Context Element",
            "focus-context-element",
            false);
    }

    private void InitializeGlobalContext()
    {
        // ActiveContextsContext
        {
            var keymapArgument = new KeymapArgument("KeyA", null, false, false, true, true, true, true);
            _ = ContextFacts.GlobalContext.Keymap.Map.TryAdd(keymapArgument, ConstructFocusContextElementCommand(ContextFacts.ActiveContextsContext));
        }
        // BackgroundServicesContext
        {
            var keymapArgument = new KeymapArgument("KeyB", null, false, false, true, true, true, true);
            _ = ContextFacts.GlobalContext.Keymap.Map.TryAdd(keymapArgument, ConstructFocusContextElementCommand(ContextFacts.BackgroundServicesContext));
        }
        // CompilerServiceExplorerContext
        {
            var keymapArgument = new KeymapArgument("KeyC", null, false, false, true, true, true, true);
            _ = ContextFacts.GlobalContext.Keymap.Map.TryAdd(keymapArgument, ConstructFocusContextElementCommand(ContextFacts.CompilerServiceExplorerContext));
        }
        // DialogDisplayContext
        {
            var keymapArgument = new KeymapArgument("KeyD", null, false, false, true, true, true, true);
            _ = ContextFacts.GlobalContext.Keymap.Map.TryAdd(keymapArgument, ConstructFocusContextElementCommand(ContextFacts.DialogDisplayContext));
        }
        // EditorContext
        {
            var keymapArgument = new KeymapArgument("KeyE", null, false, false, true, true, true, true);
            _ = ContextFacts.GlobalContext.Keymap.Map.TryAdd(keymapArgument, ConstructFocusContextElementCommand(ContextFacts.EditorContext));
        }
        // FolderExplorerContext
        {
            var keymapArgument = new KeymapArgument("KeyF", null, false, false, true, true, true, true);
            _ = ContextFacts.GlobalContext.Keymap.Map.TryAdd(keymapArgument, ConstructFocusContextElementCommand(ContextFacts.FolderExplorerContext));
        }
        //// GitContext
        //{
        //    var keymapArgument = new KeymapArgument("KeyG", null, false, false, true, true, true, true);
        //    _ = ContextFacts.GlobalContext.Keymap.Map.TryAdd(keymapArgument, ConstructFocusContextElementCommand(ContextFacts.GitContext));
        //}
        // GlobalContext
        {
            var keymapArgument = new KeymapArgument("KeyG", null, false, false, true, true, true, true);
            _ = ContextFacts.GlobalContext.Keymap.Map.TryAdd(keymapArgument, ConstructFocusContextElementCommand(ContextFacts.GlobalContext));
        }
        //// MainLayoutFooterContext
        //{
        //    var keymapArgument = new KeymapArgument("KeyF", null, false, false, true, true, true, true);
        //    _ = ContextFacts.GlobalContext.Keymap.Map.TryAdd(keymapArgument, ConstructFocusContextElementCommand(ContextFacts.MainLayoutFooterContext));
        //}
        //// MainLayoutHeaderContext
        //{
        //    var keymapArgument = new KeymapArgument("KeyH", null, false, false, true, true, true, true);
        //    _ = ContextFacts.GlobalContext.Keymap.Map.TryAdd(keymapArgument, ConstructFocusContextElementCommand(ContextFacts.MainLayoutHeaderContext));
        //}
        // NuGetPackageManagerContext
        {
            var keymapArgument = new KeymapArgument("KeyN", null, false, false, true, true, true, true);
            _ = ContextFacts.GlobalContext.Keymap.Map.TryAdd(keymapArgument, ConstructFocusContextElementCommand(ContextFacts.NuGetPackageManagerContext));
        }
        // SolutionExplorerContext
        {
            var keymapArgument = new KeymapArgument("KeyS", null, false, false, true, true, true, true);
            _ = ContextFacts.GlobalContext.Keymap.Map.TryAdd(keymapArgument, ConstructFocusContextElementCommand(ContextFacts.SolutionExplorerContext));
        }
        //// TerminalContext
        //{
        //    var keymapArgument = new KeymapArgument("KeyT", null, false, false, true, true, true, true);
        //    _ = ContextFacts.GlobalContext.Keymap.Map.TryAdd(keymapArgument, ConstructFocusContextElementCommand(ContextFacts.TerminalContext));
        //}
        // TextEditorContext
        {
            var keymapArgument = new KeymapArgument("KeyT", null, false, false, true, true, true, true);
            _ = ContextFacts.GlobalContext.Keymap.Map.TryAdd(keymapArgument, ConstructFocusContextElementCommand(ContextFacts.TextEditorContext));
        }
    }
}
