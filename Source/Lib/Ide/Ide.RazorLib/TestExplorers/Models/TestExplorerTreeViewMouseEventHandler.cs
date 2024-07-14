using Fluxor;
using Luthetus.Common.RazorLib.Commands.Models;
using Luthetus.Common.RazorLib.TreeViews.Models;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.ComponentRenderers.Models;
using Luthetus.Common.RazorLib.Notifications.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;
using Luthetus.TextEditor.RazorLib.BackgroundTasks.Models;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.TextEditor.RazorLib.Installations.Models;
using Luthetus.TextEditor.RazorLib.Groups.Models;
using Luthetus.Ide.RazorLib.BackgroundTasks.Models;
using Luthetus.Ide.RazorLib.Namespaces.Models;
using Luthetus.Ide.RazorLib.TestExplorers.Models;
using Luthetus.Ide.RazorLib.CompilerServices.Models;

namespace Luthetus.Ide.RazorLib.TestExplorers.Models;

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
	        
	    _textEditorService.Post(new ReadOnlyTextEditorTask(
            nameof(TestExplorerTreeViewMouseEventHandler),
            ShowTestInEditorFactory(className),
            null));

		return Task.CompletedTask;
    }
    
    /// <summary>
    /// TODO: D.R.Y.: This method is copy and pasted, then altered a bit, from
    /// <see cref="Luthetus.TextEditor.RazorLib.Commands.Models.Defaults.TextEditorCommandDefaultFunctions.GoToDefinitionFactory"/>.
    /// </summary>
    private TextEditorEdit ShowTestInEditorFactory(string className)
    {
    	return (IEditContext editContext) =>
        {
			var wordTextSpan = TextEditorTextSpan.FabricateTextSpan(className);
			
			if (_compilerServiceRegistry is not CompilerServiceRegistry)
			{
				NotificationHelper.DispatchInformative(
			        nameof(TestExplorerTreeViewMouseEventHandler),
			        $"Could not open in editor because _compilerServiceRegistry was not the type: 'CompilerServiceRegistry'; it was '{_compilerServiceRegistry.GetType().Name}'",
			        _commonComponentRenderers,
			        _dispatcher,
			        TimeSpan.FromSeconds(5));
			
				return Task.CompletedTask;
			}
			
			var cSharpCompilerService = ((CompilerServiceRegistry)_compilerServiceRegistry).CSharpCompilerService;
			
            var definitionTextSpan = cSharpCompilerService.Binder.GetDefinition(wordTextSpan);

            if (definitionTextSpan is null)
            {
            	NotificationHelper.DispatchInformative(
			        nameof(TestExplorerTreeViewMouseEventHandler),
			        $"Could not open in editor because definitionTextSpan was null",
			        _commonComponentRenderers,
			        _dispatcher,
			        TimeSpan.FromSeconds(5));
            
                return Task.CompletedTask;
            }

            var definitionModel = _textEditorService.ModelApi.GetOrDefault(definitionTextSpan.ResourceUri);

            if (definitionModel is null)
            {
                if (_textEditorService.TextEditorConfig.RegisterModelFunc is not null)
                {
                    _textEditorService.TextEditorConfig.RegisterModelFunc.Invoke(
                        new RegisterModelArgs(definitionTextSpan.ResourceUri, _serviceProvider));

                    var definitionModelModifier = editContext.GetModelModifier(definitionTextSpan.ResourceUri);

                    if (definitionModel is null)
                    {
                    	NotificationHelper.DispatchInformative(
					        nameof(TestExplorerTreeViewMouseEventHandler),
					        $"Could not open in editor because definitionModel was null",
					        _commonComponentRenderers,
					        _dispatcher,
					        TimeSpan.FromSeconds(5));
                    
                        return Task.CompletedTask;
                    }
                }
                else
                {
                	NotificationHelper.DispatchInformative(
				        nameof(TestExplorerTreeViewMouseEventHandler),
				        $"Could not open in editor because _textEditorService.TextEditorConfig.RegisterModelFunc was null",
				        _commonComponentRenderers,
				        _dispatcher,
				        TimeSpan.FromSeconds(5));
                
                    return Task.CompletedTask;
                }
            }

            var definitionViewModels = _textEditorService.ModelApi.GetViewModelsOrEmpty(definitionTextSpan.ResourceUri);

            if (!definitionViewModels.Any())
            {
                if (_textEditorService.TextEditorConfig.TryRegisterViewModelFunc is not null)
                {
                    _textEditorService.TextEditorConfig.TryRegisterViewModelFunc.Invoke(new TryRegisterViewModelArgs(
                        Key<TextEditorViewModel>.NewKey(),
                        definitionTextSpan.ResourceUri,
                        new Category("main"),
                        true,
                        _serviceProvider));

                    definitionViewModels = _textEditorService.ModelApi.GetViewModelsOrEmpty(definitionTextSpan.ResourceUri);

                    if (!definitionViewModels.Any())
                    {
                    	NotificationHelper.DispatchInformative(
					        nameof(TestExplorerTreeViewMouseEventHandler),
					        $"Could not open in editor because !definitionViewModels.Any()",
					        _commonComponentRenderers,
					        _dispatcher,
					        TimeSpan.FromSeconds(5));
					        
                        return Task.CompletedTask;
                    }
                }
                else
                {
                	NotificationHelper.DispatchInformative(
				        nameof(TestExplorerTreeViewMouseEventHandler),
				        $"Could not open in editor because _textEditorService.TextEditorConfig.TryRegisterViewModelFunc was null",
				        _commonComponentRenderers,
				        _dispatcher,
				        TimeSpan.FromSeconds(5));
                
                    return Task.CompletedTask;
                }
            }

            var definitionViewModelKey = definitionViewModels.First().ViewModelKey;
            
            var definitionViewModelModifier = editContext.GetViewModelModifier(definitionViewModelKey);
            var definitionCursorModifierBag = editContext.GetCursorModifierBag(definitionViewModelModifier?.ViewModel);
            var definitionPrimaryCursorModifier = editContext.GetPrimaryCursorModifier(definitionCursorModifierBag);

            if (definitionViewModelModifier is null || definitionCursorModifierBag is null || definitionPrimaryCursorModifier is null)
            {
            	NotificationHelper.DispatchInformative(
			        nameof(TestExplorerTreeViewMouseEventHandler),
			        $"Could not open in editor because definitionViewModelModifier was null || definitionCursorModifierBag was null || definitionPrimaryCursorModifier was null",
			        _commonComponentRenderers,
			        _dispatcher,
			        TimeSpan.FromSeconds(5));
				        
                return Task.CompletedTask;
            }

            var rowData = definitionModel.GetLineInformationFromPositionIndex(definitionTextSpan.StartingIndexInclusive);
            var columnIndex = definitionTextSpan.StartingIndexInclusive - rowData.StartPositionIndexInclusive;

            definitionPrimaryCursorModifier.SelectionAnchorPositionIndex = null;
            definitionPrimaryCursorModifier.LineIndex = rowData.Index;
            definitionPrimaryCursorModifier.ColumnIndex = columnIndex;
            definitionPrimaryCursorModifier.PreferredColumnIndex = columnIndex;

            if (_textEditorService.TextEditorConfig.TryShowViewModelFunc is not null)
            {
                _textEditorService.TextEditorConfig.TryShowViewModelFunc.Invoke(new TryShowViewModelArgs(
                    definitionViewModelKey,
                    Key<TextEditorGroup>.Empty,
                    _serviceProvider));
            }
            else
            {
            	NotificationHelper.DispatchInformative(
			        nameof(TestExplorerTreeViewMouseEventHandler),
			        $"Could not open in editor because _textEditorService.TextEditorConfig.TryShowViewModelFunc was null",
			        _commonComponentRenderers,
			        _dispatcher,
			        TimeSpan.FromSeconds(5));
            }

            return Task.CompletedTask;
        };
    }
}