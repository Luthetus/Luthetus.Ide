using Microsoft.AspNetCore.Components;
using Fluxor;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Reactives.Models;
using Luthetus.Common.RazorLib.Options.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;
using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax;
using Luthetus.TextEditor.RazorLib.Exceptions;
using Luthetus.TextEditor.RazorLib.Groups.Models;
using Luthetus.TextEditor.RazorLib.Groups.States;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.States;
using Luthetus.CompilerServices.CSharp.CompilerServiceCase;
using Luthetus.Extensions.DotNet.CompilerServices.States;
using Luthetus.Ide.RazorLib.Editors.Models;

namespace Luthetus.Extensions.DotNet.CompilerServices.Displays;

public partial class CompilerServiceEditorDisplay : ComponentBase, IDisposable
{
	/// <summary>
	/// Start with <see cref="CSharpCompilerService"/>,
	/// then make <see cref="CompilerServiceEditorDisplay"/> more generic, to accept just an
	/// <see cref="TextEditor.RazorLib.CompilerServices.Interfaces.ILuthCompilerService"/>
	/// (2024-01-28)
	/// </summary>
	[Inject]
	public ICompilerServiceRegistry CompilerServiceRegistry { get; set; } = null!;
	[Inject]
	public IAppOptionsService AppOptionsService { get; set; } = null!;
	[Inject]
	private IState<CompilerServiceEditorState> CompilerServiceEditorStateWrap { get; set; } = null!;
	[Inject]
	private IState<TextEditorState> TextEditorStateWrap { get; set; } = null!;
	[Inject]
	private IState<TextEditorGroupState> TextEditorGroupStateWrap { get; set; } = null!;

	private readonly Throttle _throttleEventCausingReRender = new(TimeSpan.FromMilliseconds(75));

	private CSharpCompilerService _cSharpCompilerService = null!;

	private bool _shouldRecalculateViewModel = true;
	private CompilerServiceEditorViewModel _viewModel = null!;

	protected override void OnInitialized()
	{
        // TODO: This must be removed as it puts a requirement to have the CSharpCompilerService...
        //       ...instead generalize this component to iterate over the CompilerServiceRegistry.CompilerServiceList
        _cSharpCompilerService = (CSharpCompilerService)CompilerServiceRegistry.GetCompilerService(ExtensionNoPeriodFacts.C_SHARP_CLASS);

		_cSharpCompilerService.ResourceRegistered += CSharpCompilerService_StateChanged;
		_cSharpCompilerService.ResourceParsed += CSharpCompilerService_StateChanged;
		_cSharpCompilerService.ResourceDisposed += CSharpCompilerService_StateChanged;
		_cSharpCompilerService.CursorMovedInSyntaxTree += CSharpCompilerService_StateChanged;

		CompilerServiceEditorStateWrap.StateChanged += CompilerServiceEditorStateWrap_StateChanged;
		TextEditorGroupStateWrap.StateChanged += TextEditorGroupStateWrap_StateChanged;
		TextEditorStateWrap.StateChanged += TextEditorStateWrap_StateChanged;

		base.OnInitialized();
	}

	private void RecalculateViewModel()
	{
		try
		{
			var localCSharpCompilerService = _cSharpCompilerService;
			var localCompilerServiceEditorState = CompilerServiceEditorStateWrap.Value;
			var localTextEditorGroupState = TextEditorGroupStateWrap.Value;
			var localTextEditorState = TextEditorStateWrap.Value;

			var editorTextEditorGroup = localTextEditorGroupState.GroupList.FirstOrDefault(
				x => x.GroupKey == EditorIdeApi.EditorTextEditorGroupKey);

			var activeViewModelKey = editorTextEditorGroup?.ActiveViewModelKey ?? Key<TextEditorViewModel>.Empty;

			var viewModel = localTextEditorState.ViewModelList.FirstOrDefault(
				x => x.ViewModelKey == activeViewModelKey);

			var interfaceCompilerServiceResource = viewModel is null
				? null
				: localCSharpCompilerService.GetCompilerServiceResourceFor(viewModel.ResourceUri);

			var cSharpResource = interfaceCompilerServiceResource is null
				? null
				: (CSharpResource)interfaceCompilerServiceResource;

			var textEditorModel = (TextEditorModel?)null;
			if (viewModel is not null)
			{
				var exists = localTextEditorState.__ModelList.TryGetValue(
	        		viewModel.ResourceUri, out textEditorModel);
			}

			int? primaryCursorPositionIndex = textEditorModel is null || viewModel is null
				? null
				: textEditorModel.GetPositionIndex(viewModel.PrimaryCursor);

			var syntaxNode = primaryCursorPositionIndex is null || localCSharpCompilerService.Binder is null || cSharpResource?.CompilationUnit is null
				? null
				: localCSharpCompilerService.Binder.GetSyntaxNode(primaryCursorPositionIndex.Value, cSharpResource.CompilationUnit);

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

	private async void TextEditorStateWrap_StateChanged(object? sender, EventArgs e)
	{
		ThrottledReRender();
	}

	private async void TextEditorGroupStateWrap_StateChanged(object? sender, EventArgs e)
	{
		ThrottledReRender();
	}

	private async void CompilerServiceEditorStateWrap_StateChanged(object? sender, EventArgs e)
	{
		ThrottledReRender();
	}

	private async void CSharpCompilerService_StateChanged()
	{
		ThrottledReRender();
	}

	private void ThrottledReRender()
	{
		_throttleEventCausingReRender.Run(async _ =>
		{
			_shouldRecalculateViewModel = true;
			await InvokeAsync(StateHasChanged);
		});
	}

	public void Dispose()
	{
		_cSharpCompilerService.ResourceRegistered -= CSharpCompilerService_StateChanged;
		_cSharpCompilerService.ResourceParsed -= CSharpCompilerService_StateChanged;
		_cSharpCompilerService.ResourceDisposed -= CSharpCompilerService_StateChanged;
		_cSharpCompilerService.CursorMovedInSyntaxTree -= CSharpCompilerService_StateChanged;

		CompilerServiceEditorStateWrap.StateChanged -= CompilerServiceEditorStateWrap_StateChanged;
		TextEditorGroupStateWrap.StateChanged -= TextEditorGroupStateWrap_StateChanged;
		TextEditorStateWrap.StateChanged -= TextEditorStateWrap_StateChanged;
	}

	private class CompilerServiceEditorViewModel
	{
		public CSharpCompilerService? LocalCSharpCompilerService { get; set; }
		public CompilerServiceEditorState? LocalCompilerServiceEditorState { get; set; }
		public TextEditorGroupState? LocalTextEditorGroupState { get; set; }
		public TextEditorState? LocalTextEditorState { get; set; }
		public TextEditorGroup? EditorTextEditorGroup { get; set; }
		public Key<TextEditorViewModel> ActiveViewModelKey { get; set; }
		public TextEditorViewModel? ViewModel { get; set; }
		public ICompilerServiceResource? InterfaceCompilerServiceResource { get; set; }
		public CSharpResource? CSharpResource { get; set; }
		public TextEditorModel? TextEditorModel { get; set; }
		public int? PrimaryCursorPositionIndex { get; set; }
		public ISyntaxNode? SyntaxNode { get; set; }
	}
}