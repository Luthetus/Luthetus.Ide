using Fluxor;
using Luthetus.Common.RazorLib.Commands.Models;
using Luthetus.Common.RazorLib.TreeViews.Models;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.ComponentRenderers.Models;
using Luthetus.Common.RazorLib.Notifications.Models;
using Luthetus.TextEditor.RazorLib;
using Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;

namespace Luthetus.Extensions.DotNet.TestExplorers.Models;

public class TestExplorerTreeViewMouseEventHandler : TreeViewMouseEventHandler
{
	private readonly ICommonComponentRenderers _commonComponentRenderers;
	private readonly IDispatcher _dispatcher;
	private readonly ICompilerServiceRegistry _compilerServiceRegistry;
	private readonly ITextEditorService _textEditorService;
	private readonly IServiceProvider _serviceProvider;

	public TestExplorerTreeViewMouseEventHandler(
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

	public override Task OnDoubleClickAsync(TreeViewCommandArgs commandArgs)
	{
		base.OnDoubleClickAsync(commandArgs);

		if (commandArgs.NodeThatReceivedMouseEvent is not TreeViewStringFragment treeViewStringFragment)
		{
			NotificationHelper.DispatchInformative(
				nameof(TestExplorerTreeViewMouseEventHandler),
				$"Could not open in editor because node is not type: {nameof(TreeViewStringFragment)}",
				_commonComponentRenderers,
				_dispatcher,
				TimeSpan.FromSeconds(5));

			return Task.CompletedTask;
		}

		if (treeViewStringFragment.Parent is not TreeViewStringFragment parentTreeViewStringFragment)
		{
			NotificationHelper.DispatchInformative(
				nameof(TestExplorerTreeViewMouseEventHandler),
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

		var methodName = treeViewStringFragment.Item.Value.Trim();

		NotificationHelper.DispatchInformative(
			nameof(TestExplorerTreeViewMouseEventHandler),
			methodName + "()",
			_commonComponentRenderers,
			_dispatcher,
			TimeSpan.FromSeconds(5));

		_textEditorService.PostUnique(
			nameof(TestExplorerTreeViewMouseEventHandler),
			TestExplorerHelper.ShowTestInEditorFactory(
				className,
				methodName,
				_commonComponentRenderers,
				_dispatcher,
				_compilerServiceRegistry,
				_textEditorService,
				_serviceProvider));

		return Task.CompletedTask;
	}
}