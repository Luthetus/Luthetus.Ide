using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib.Installations.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.Groups.Models;
using Luthetus.TextEditor.RazorLib.Lexers.Models;

namespace Luthetus.TextEditor.RazorLib.FindAlls.Models;

public static class TreeViewFindAllTextSpanHelper
{
	public static async Task OpenInEditorOnClick(
		TreeViewFindAllTextSpan treeViewFindAllTextSpan,
		bool shouldSetFocusToEditor,
		ITextEditorService textEditorService,
		LuthetusTextEditorConfig textEditorConfig,
		IServiceProvider serviceProvider)
	{
		string filePath = treeViewFindAllTextSpan.AbsolutePath.Value;
		var resourceUri = new ResourceUri(filePath);

        if (textEditorConfig.RegisterModelFunc is null)
			return;

        await textEditorConfig.RegisterModelFunc.Invoke(new RegisterModelArgs(
                resourceUri,
                serviceProvider))
            .ConfigureAwait(false);

        if (textEditorConfig.TryRegisterViewModelFunc is not null)
		{
			var viewModelKey = await textEditorConfig.TryRegisterViewModelFunc.Invoke(new TryRegisterViewModelArgs(
				    Key<TextEditorViewModel>.NewKey(),
                    resourceUri,
                    new Category("main"),
				    false,
				    serviceProvider))
                .ConfigureAwait(false);

            if (viewModelKey != Key<TextEditorViewModel>.Empty &&
				textEditorConfig.TryShowViewModelFunc is not null)
            {
				await textEditorConfig.TryShowViewModelFunc.Invoke(new TryShowViewModelArgs(
					    viewModelKey,
					    Key<TextEditorGroup>.Empty,
					    serviceProvider))
                    .ConfigureAwait(false);
                    
                textEditorService.PostUnique(
                	nameof(OpenInEditorOnClick),
                	editContext =>
                	{
	                	var modelModifier = editContext.GetModelModifier(resourceUri);
			            var viewModelModifier = editContext.GetViewModelModifier(viewModelKey);
			            var cursorModifierBag = editContext.GetCursorModifierBag(viewModelModifier?.ViewModel);
			            var primaryCursorModifier = editContext.GetPrimaryCursorModifier(cursorModifierBag);
			
			            if (modelModifier is null || viewModelModifier is null || cursorModifierBag is null || primaryCursorModifier is null)
			                return Task.CompletedTask;
                	
                		var lineAndColumnIndices = modelModifier.GetLineAndColumnIndicesFromPositionIndex(
                			treeViewFindAllTextSpan.Item.StartingIndexInclusive);
                			
                		primaryCursorModifier.LineIndex = lineAndColumnIndices.lineIndex;
                		primaryCursorModifier.ColumnIndex = lineAndColumnIndices.columnIndex;
                		
                		return Task.CompletedTask;
                	});
            }
        }
	}
}
