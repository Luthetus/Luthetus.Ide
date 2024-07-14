using Fluxor;
using Luthetus.Common.RazorLib.Commands.Models;
using Luthetus.Common.RazorLib.ComponentRenderers.Models;
using Luthetus.Common.RazorLib.Keyboards.Models;
using Luthetus.Common.RazorLib.Menus.Models;
using Luthetus.Common.RazorLib.Notifications.Models;
using Luthetus.Common.RazorLib.TreeViews.Models;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.TextEditor.RazorLib;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;
using Luthetus.TextEditor.RazorLib.BackgroundTasks.Models;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.TextEditor.RazorLib.Installations.Models;
using Luthetus.TextEditor.RazorLib.Groups.Models;
using Luthetus.CompilerServices.Lang.CSharp.BinderCase;
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
    private readonly ICompilerServiceRegistry _compilerServiceRegistry;
    private readonly ITextEditorService _textEditorService;
	private readonly IServiceProvider _serviceProvider;
            
    public TestExplorerTreeViewKeyboardEventHandler(
		    ICommonComponentRenderers commonComponentRenderers,
	        IDispatcher dispatcher,
	        ICompilerServiceRegistry compilerServiceRegistry,
	        ITextEditorService textEditorService,
	        IServiceProvider serviceProvider,
            ITreeViewService treeViewService,
		    IBackgroundTaskService backgroundTaskService)
        : base(treeViewService, backgroundTaskService)
    {
    	_commonComponentRenderers = commonComponentRenderers;
        _dispatcher = dispatcher;
        _compilerServiceRegistry = compilerServiceRegistry;
        _textEditorService = textEditorService;
        _serviceProvider = serviceProvider;
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
        {
        	NotificationHelper.DispatchInformative(
		        nameof(TestExplorerTreeViewKeyboardEventHandler),
		        $"Could not open in editor because node is not type: {nameof(TreeViewStringFragment)}",
		        _commonComponentRenderers,
		        _dispatcher,
		        TimeSpan.FromSeconds(5));
	        
        	return Task.CompletedTask;
        }
            
        if (treeViewStringFragment.Parent is not TreeViewStringFragment parentTreeViewStringFragment)
        {
            NotificationHelper.DispatchInformative(
		        nameof(TestExplorerTreeViewKeyboardEventHandler),
		        $"Could not open in editor because node's parent does not seem to include a class name",
		        _commonComponentRenderers,
		        _dispatcher,
		        TimeSpan.FromSeconds(5));
            
            return Task.CompletedTask;
        }
        
        var className = parentTreeViewStringFragment.Item.Value.Split('.').Last();

        NotificationHelper.DispatchInformative(
	        nameof(TestExplorerTreeViewMouseEventHandler),
	        className + ".cs",
	        _commonComponentRenderers,
	        _dispatcher,
	        TimeSpan.FromSeconds(5));

        _textEditorService.Post(new ReadOnlyTextEditorTask(
            nameof(TestExplorerTreeViewKeyboardEventHandler),
            TestExplorerHelper.ShowTestInEditorFactory(
            	className,
				_commonComponentRenderers,
		        _dispatcher,
		        _compilerServiceRegistry,
		        _textEditorService,
		        _serviceProvider),
            null));

        return Task.CompletedTask;
    }
}