using Microsoft.AspNetCore.Components;
using Luthetus.TextEditor.RazorLib;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;
using Luthetus.CompilerServices.CSharp.CompilerServiceCase;

namespace Luthetus.Extensions.DotNet.CompilerServices.Displays;

public partial class CompilerServiceEditorDisplay : ComponentBase
{
	/*[Inject]
	public ITextEditorService TextEditorService { get; set; } = null!;
	[Inject]
	public ICompilerServiceRegistry CompilerServiceRegistry { get; set; } = null!;

	private MarkupString? _markupString;
	
	protected override void OnInitialized()
	{
        // TODO: This must be removed as it puts a requirement to have the CSharpCompilerService...
        //       ...instead generalize this component to iterate over the CompilerServiceRegistry.CompilerServiceList
        _cSharpCompilerService = (CSharpCompilerService)CompilerServiceRegistry.GetCompilerService(ExtensionNoPeriodFacts.C_SHARP_CLASS);
		base.OnInitialized();
	}
	
	private void RecalculateViewModel()
	{
		try
		{
			var localCSharpCompilerService = _cSharpCompilerService;
			var localCompilerServiceEditorState = DotNetBackgroundTaskApi.CompilerServiceEditorService.GetCompilerServiceEditorState();
			var localTextEditorGroupState = TextEditorService.GroupApi.GetTextEditorGroupState();
			var localTextEditorState = TextEditorService.TextEditorState;

			var editorTextEditorGroup = localTextEditorGroupState.GroupList.FirstOrDefault(
				x => x.GroupKey == EditorIdeApi.EditorTextEditorGroupKey);

			var activeViewModelKey = editorTextEditorGroup?.ActiveViewModelKey ?? Key<TextEditorViewModel>.Empty;
			
			var model_viewmodel_tuple = localTextEditorState.GetModelAndViewModelOrDefault(
				activeViewModelKey);
				
			var viewModel = model_viewmodel_tuple.ViewModel;
			var textEditorModel = model_viewmodel_tuple.Model;

			var interfaceCompilerServiceResource = viewModel is null
				? null
				: localCSharpCompilerService.GetCompilerServiceResourceFor(viewModel.ResourceUri);

			var cSharpResource = interfaceCompilerServiceResource is null
				? null
				: (CSharpResource)interfaceCompilerServiceResource;

			int? primaryCursorPositionIndex = textEditorModel is null || viewModel is null
				? null
				: textEditorModel.GetPositionIndex(viewModel.PrimaryCursor);

			var syntaxNode = primaryCursorPositionIndex is null || localCSharpCompilerService.Binder is null || cSharpResource?.CompilationUnit is null
				? null
				: localCSharpCompilerService.Binder.GetSyntaxNode(primaryCursorPositionIndex.Value, cSharpResource.ResourceUri, cSharpResource);

			_viewModel = new CompilerServiceEditorViewModel
			{
				LocalCSharpCompilerService = localCSharpCompilerService,
				LocalCompilerServiceEditorState = localCompilerServiceEditorState,
				LocalTextEditorGroupState = localTextEditorGroupState,
				LocalTextEditorState = localTextEditorState,
				EditorTextEditorGroup = editorTextEditorGroup,
				ActiveViewModelKey = activeViewModelKey,
				ViewModel = viewModel,
				InterfaceCompilerServiceResource = interfaceCompilerServiceResource,
				CSharpResource = cSharpResource,
				TextEditorModel = textEditorModel,
				PrimaryCursorPositionIndex = primaryCursorPositionIndex,
				SyntaxNode = syntaxNode,
			};
		}
		catch (LuthetusTextEditorException)
		{
			// Eat this exception
		}
	}
	
	private void HandleOnClick()
	{
		WriteChildrenIndentedRecursive();
	}

	/// <summary>
	/// TODO: It is probably more efficient to do this as a RenderFragment (maybe the name I'm looking for  is RenderTreeBuilder), rather than MarkupString.
	/// </summary>
	private void WriteChildrenIndentedRecursive(
		ISyntaxNode node,
		StringBuilder? stringBuilderLocal = null,
		string name = "node",
		int indentation = 0)
    {
    	stringBuilderLocal ??= new();
    	
    	stringBuilderLocal.Append($"<div class=\"luth_compiler-service-editor-indentation-{indentation}\">");
    	stringBuilderLocal.Append(node.SyntaxKind);
    	
    	// For the child tokens
    	var childIndentation = indentation + 1;
		foreach (var child in node.GetChildList())
		{
			if (child is ISyntaxNode syntaxNode)
			{
				WriteChildrenIndentedRecursive(syntaxNode, stringBuilderLocal: stringBuilderLocal, indentation: childIndentation);
			}
			else if (child is SyntaxToken syntaxToken)
			{
				stringBuilderLocal.Append($"<div class=\"luth_compiler-service-editor-indentation-{childIndentation}\">");
				stringBuilderLocal.Append($"{child.SyntaxKind}__{syntaxToken.TextSpan.GetText()}");
				stringBuilderLocal.Append($"</div>");
			}
		}
		
		stringBuilderLocal.Append($"</div>");
		
		if (indentation == 0)
			_markupString = (MarkupString)stringBuilderLocal.ToString();
    }*/
}
