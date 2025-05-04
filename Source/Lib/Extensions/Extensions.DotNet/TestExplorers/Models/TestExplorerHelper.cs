using Luthetus.Common.RazorLib.ComponentRenderers.Models;
using Luthetus.Common.RazorLib.Notifications.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.TextEditor.RazorLib.Installations.Models;
using Luthetus.TextEditor.RazorLib.Groups.Models;
using Luthetus.CompilerServices.CSharp.BinderCase;
using Luthetus.CompilerServices.CSharp.CompilerServiceCase;
using Luthetus.TextEditor.RazorLib.CompilerServices;

namespace Luthetus.Extensions.DotNet.TestExplorers.Models;

public static class TestExplorerHelper
{
	/// <summary>
	/// TODO: D.R.Y.: This method is copy and pasted, then altered a bit, from
	/// <see cref="TextEditor.RazorLib.Commands.Models.Defaults.TextEditorCommandDefaultFunctions.GoToDefinitionFactory"/>.
	/// </summary>
	public static Func<TextEditorEditContext, ValueTask> ShowTestInEditorFactory(
		string className,
		string methodName,
		ICommonComponentRenderers commonComponentRenderers,
		INotificationService notificationService,
		ICompilerServiceRegistry compilerServiceRegistry,
		ITextEditorService textEditorService,
		IServiceProvider serviceProvider)
	{
		return (editContext) =>
		{
			/*var wordTextSpan = TextEditorTextSpan.FabricateTextSpan(className);

            var possibleCSharpCompilerService = compilerServiceRegistry
                .GetCompilerService(ExtensionNoPeriodFacts.C_SHARP_CLASS);

            if (possibleCSharpCompilerService is null)
			{
				NotificationHelper.DispatchInformative(
					nameof(TestExplorerTreeViewMouseEventHandler),
					$"Could not open in editor because {nameof(possibleCSharpCompilerService)} was null",
					commonComponentRenderers,
					notificationService,
					TimeSpan.FromSeconds(5));

				return ValueTask.CompletedTask;
			}

			var cSharpCompilerService = (CSharpCompilerService)possibleCSharpCompilerService;
			var cSharpBinder = (CSharpBinder)cSharpCompilerService.Binder;

			// ImmutableDictionary<NamespaceAndTypeIdentifiers, TypeDefinitionNode>
			var allTypeDefinitions = cSharpBinder.AllTypeDefinitions;

			var typeDefinitionNodeList = allTypeDefinitions.Where(kvp => kvp.Key == className)
				.ToList();

			if (typeDefinitionNodeList.Count == 0)
			{
				NotificationHelper.DispatchInformative(
					nameof(TestExplorerTreeViewMouseEventHandler),
					$"Could not open in editor because typeDefinitionNodeList.Count == 0",
					commonComponentRenderers,
					notificationService,
					TimeSpan.FromSeconds(5));

				return ValueTask.CompletedTask;
			}

			var typeDefinitionNode = typeDefinitionNodeList.First().Value;

			var definitionTextSpan = typeDefinitionNode.TypeIdentifierToken.TextSpan;

			var functionDefinitionNodeList = typeDefinitionNode.GetFunctionDefinitionNodes();

			if (functionDefinitionNodeList.Length != 0)
			{
				var methodMatchList = functionDefinitionNodeList
					.Where(x => x.FunctionIdentifierToken.TextSpan.GetText() == methodName)
					.ToList();

				if (methodMatchList.Count != 0)
				{
					var functionDefinitionNode = methodMatchList.First();
					definitionTextSpan = functionDefinitionNode.FunctionIdentifierToken.TextSpan;

					NotificationHelper.DispatchInformative(
						nameof(TestExplorerTreeViewMouseEventHandler),
						$"The method: {methodName}, was found",
						commonComponentRenderers,
						notificationService,
						TimeSpan.FromSeconds(5));
				}
				else
				{
					NotificationHelper.DispatchInformative(
						nameof(TestExplorerTreeViewMouseEventHandler),
						$"Could not find the method because methodMatchList.Count == 0",
						commonComponentRenderers,
						notificationService,
						TimeSpan.FromSeconds(5));
				}
			}
			else
			{
				NotificationHelper.DispatchInformative(
					nameof(TestExplorerTreeViewMouseEventHandler),
					$"Could not find the method because functionDefinitionNodeList.Length == 0",
					commonComponentRenderers,
					notificationService,
					TimeSpan.FromSeconds(5));
			}

			if (definitionTextSpan == default)
			{
				NotificationHelper.DispatchInformative(
					nameof(TestExplorerTreeViewMouseEventHandler),
					$"Could not open in editor because definitionTextSpan was null",
					commonComponentRenderers,
					notificationService,
					TimeSpan.FromSeconds(5));

				return ValueTask.CompletedTask;
			}

			var definitionModel = textEditorService.ModelApi.GetOrDefault(definitionTextSpan.ResourceUri);

			if (definitionModel is null)
			{
				if (textEditorService.TextEditorConfig.RegisterModelFunc is not null)
				{
					textEditorService.TextEditorConfig.RegisterModelFunc.Invoke(
						new RegisterModelArgs(definitionTextSpan.ResourceUri, serviceProvider));

					var definitionModelModifier = editContext.GetModelModifier(definitionTextSpan.ResourceUri);

					if (definitionModel is null)
					{
						NotificationHelper.DispatchInformative(
							nameof(TestExplorerTreeViewMouseEventHandler),
							$"Could not open in editor because definitionModel was null",
							commonComponentRenderers,
							notificationService,
							TimeSpan.FromSeconds(5));

						return ValueTask.CompletedTask;
					}
				}
				else
				{
					NotificationHelper.DispatchInformative(
						nameof(TestExplorerTreeViewMouseEventHandler),
						$"Could not open in editor because textEditorService.TextEditorConfig.RegisterModelFunc was null",
						commonComponentRenderers,
						notificationService,
						TimeSpan.FromSeconds(5));

					return ValueTask.CompletedTask;
				}
			}

			var definitionViewModels = textEditorService.ModelApi.GetViewModelsOrEmpty(definitionTextSpan.ResourceUri);

			if (!definitionViewModels.Any())
			{
				if (textEditorService.TextEditorConfig.TryRegisterViewModelFunc is not null)
				{
					textEditorService.TextEditorConfig.TryRegisterViewModelFunc.Invoke(new TryRegisterViewModelArgs(
						Key<TextEditorViewModel>.NewKey(),
						definitionTextSpan.ResourceUri,
						new Category("main"),
						true,
						serviceProvider));

					definitionViewModels = textEditorService.ModelApi.GetViewModelsOrEmpty(definitionTextSpan.ResourceUri);

					if (!definitionViewModels.Any())
					{
						NotificationHelper.DispatchInformative(
							nameof(TestExplorerTreeViewMouseEventHandler),
							$"Could not open in editor because !definitionViewModels.Any()",
							commonComponentRenderers,
							notificationService,
							TimeSpan.FromSeconds(5));

						return ValueTask.CompletedTask;
					}
				}
				else
				{
					NotificationHelper.DispatchInformative(
						nameof(TestExplorerTreeViewMouseEventHandler),
						$"Could not open in editor because textEditorService.TextEditorConfig.TryRegisterViewModelFunc was null",
						commonComponentRenderers,
						notificationService,
						TimeSpan.FromSeconds(5));

					return ValueTask.CompletedTask;
				}
			}

			var definitionViewModelKey = definitionViewModels.First().ViewModelKey;

			var definitionViewModelModifier = editContext.GetViewModelModifier(definitionViewModelKey);
			var definitionCursorModifierBag = editContext.GetCursorModifierBag(definitionViewModelModifier?.ViewModel);
			var definitionPrimaryCursorModifier = editContext.GetPrimaryCursorModifier(definitionCursorModifierBag);

			if (definitionViewModelModifier is null || !definitionCursorModifierBag.ConstructorWasInvoked || definitionPrimaryCursorModifier is null)
			{
				NotificationHelper.DispatchInformative(
					nameof(TestExplorerTreeViewMouseEventHandler),
					$"Could not open in editor because definitionViewModelModifier was null || definitionCursorModifierBag was null || definitionPrimaryCursorModifier was null",
					commonComponentRenderers,
					notificationService,
					TimeSpan.FromSeconds(5));

				return ValueTask.CompletedTask;
			}

			var rowData = definitionModel.GetLineInformationFromPositionIndex(definitionTextSpan.StartInclusiveIndex);
			var columnIndex = definitionTextSpan.StartInclusiveIndex - rowData.PositionStartInclusiveIndex;

			definitionPrimaryCursorModifier.SelectionAnchorPositionIndex = null;
			definitionPrimaryCursorModifier.LineIndex = rowData.Index;
			definitionPrimaryCursorModifier.ColumnIndex = columnIndex;
			definitionPrimaryCursorModifier.PreferredColumnIndex = columnIndex;

			if (textEditorService.TextEditorConfig.TryShowViewModelFunc is not null)
			{
				textEditorService.TextEditorConfig.TryShowViewModelFunc.Invoke(new TryShowViewModelArgs(
					definitionViewModelKey,
					Key<TextEditorGroup>.Empty,
					true,
					serviceProvider));
			}
			else
			{
				NotificationHelper.DispatchInformative(
					nameof(TestExplorerTreeViewMouseEventHandler),
					$"Could not open in editor because textEditorService.TextEditorConfig.TryShowViewModelFunc was null",
					commonComponentRenderers,
					notificationService,
					TimeSpan.FromSeconds(5));
			}*/
			
			return ValueTask.CompletedTask;
		};
	}
}
