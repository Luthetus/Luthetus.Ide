using Fluxor;
using Luthetus.Common.RazorLib.Commands.Models;
using Luthetus.Common.RazorLib.ComponentRenderers.Models;
using Luthetus.Common.RazorLib.Keyboards.Models;
using Luthetus.Common.RazorLib.Menus.Models;
using Luthetus.Common.RazorLib.Notifications.Models;
using Luthetus.Common.RazorLib.TreeViews.Models;	
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Ide.RazorLib.BackgroundTasks.Models;
using Luthetus.Ide.RazorLib.Namespaces.Models;
using Luthetus.Ide.RazorLib.DotNetSolutions.States;
using Luthetus.Ide.RazorLib.DotNetSolutions.Displays;
using Luthetus.Ide.RazorLib.DotNetSolutions.Displays.Internals;
using Luthetus.Ide.RazorLib.Menus.Models;
using Luthetus.Ide.RazorLib.TestExplorers.Models;

namespace Luthetus.Ide.RazorLib.TestExplorers.Models;

public class TestExplorerTreeViewKeyboardEventHandler : TreeViewKeyboardEventHandler
{
	private readonly ICommonComponentRenderers _commonComponentRenderers;
    private readonly IDispatcher _dispatcher;

    public TestExplorerTreeViewKeyboardEventHandler(
		    ICommonComponentRenderers commonComponentRenderers,
	        IDispatcher dispatcher,
            ITreeViewService treeViewService,
		    IBackgroundTaskService backgroundTaskService)
        : base(treeViewService, backgroundTaskService)
    {
    	_commonComponentRenderers = commonComponentRenderers;
        _dispatcher = dispatcher;
    }

    public override Task OnKeyDownAsync(TreeViewCommandArgs commandArgs)
    {
        if (commandArgs.KeyboardEventArgs is null)
            return Task.CompletedTask;

        base.OnKeyDownAsync(commandArgs);

        switch (commandArgs.KeyboardEventArgs.Code)
        {
            case KeyboardKeyFacts.WhitespaceCodes.ENTER_CODE:
                return InvokeOpenInEditor(commandArgs, true);
            case KeyboardKeyFacts.WhitespaceCodes.SPACE_CODE:
                return InvokeOpenInEditor(commandArgs, false);
        }

        return Task.CompletedTask;
    }

    private Task InvokeOpenInEditor(TreeViewCommandArgs commandArgs, bool shouldSetFocusToEditor)
    {
        var activeNode = commandArgs.TreeViewContainer.ActiveNode;

        if (activeNode is not TreeViewStringFragment treeViewStringFragment)
            return Task.CompletedTask;

        NotificationHelper.DispatchInformative(
	        nameof(TestExplorerTreeViewKeyboardEventHandler),
	        nameof(InvokeOpenInEditor),
	        _commonComponentRenderers,
	        _dispatcher,
	        TimeSpan.FromSeconds(5));

        return Task.CompletedTask;
    }
}