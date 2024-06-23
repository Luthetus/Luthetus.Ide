using System.Collections.Immutable;
using Microsoft.Extensions.DependencyInjection;
using Fluxor;
using Luthetus.Common.RazorLib.Menus.Models;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;
using Luthetus.TextEditor.RazorLib.Installations.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.Groups.Models;
using Luthetus.CompilerServices.Lang.DotNetSolution.CompilerServiceCase;
using Luthetus.Ide.RazorLib.DotNetSolutions.States;

namespace Luthetus.Ide.RazorLib.DotNetSolutions.Models.Internals;

public class SolutionVisualizationDrawing<TItem> : ISolutionVisualizationDrawing
{
	public TItem Item { get; set; }
	public SolutionVisualizationDrawingKind SolutionVisualizationDrawingKind { get; set; }
	public int CenterX { get; set; }
	public int CenterY { get; set; }
	public int Radius { get; set; }
	public string Fill { get; set; }
	public int RenderCycle { get; set; }
	public int RenderCycleSequence { get; set; }

	object ISolutionVisualizationDrawing.Item => Item;

	public MenuOptionRecord GetMenuOptionRecord(
		IEnvironmentProvider environmentProvider,
		LuthetusTextEditorConfig textEditorConfig,
		IServiceProvider serviceProvider)
	{
		var menuOptionRecordList = new List<MenuOptionRecord>();
		
		var targetDisplayName = "unknown";

		if (Item is ILuthCompilerServiceResource compilerServiceResource)
		{
			var absolutePath = environmentProvider.AbsolutePathFactory(compilerServiceResource.ResourceUri.Value, false);
			targetDisplayName = absolutePath.NameWithExtension;

			menuOptionRecordList.Add(new MenuOptionRecord(
			    "Open in editor",
			    MenuOptionKind.Other,
				OnClickFunc: () => OpenFileInEditor(
					compilerServiceResource.ResourceUri.Value,
					textEditorConfig,
					serviceProvider)));

			if (SolutionVisualizationDrawingKind == SolutionVisualizationDrawingKind.Solution)
			{
				menuOptionRecordList.Add(new MenuOptionRecord(
				    "Load Projects",
				    MenuOptionKind.Other,
					OnClickFunc: () => LoadProjects(
						(DotNetSolutionResource)compilerServiceResource,
						textEditorConfig,
						serviceProvider)));			
			}
		}

		return new MenuOptionRecord(
		    targetDisplayName,
		    MenuOptionKind.Other,
			SubMenu: new MenuRecord(menuOptionRecordList.ToImmutableArray()));
	}

	private async Task OpenFileInEditor(
		string filePath,
		LuthetusTextEditorConfig textEditorConfig,
		IServiceProvider serviceProvider)
	{
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
            }
        }
    }

	private async Task LoadProjects(
		DotNetSolutionResource dotNetSolutionResource,
		LuthetusTextEditorConfig textEditorConfig,
		IServiceProvider serviceProvider)
	{
        if (textEditorConfig.RegisterModelFunc is null)
            return;

		var dotNetSolutionStateWrap = serviceProvider.GetRequiredService<IState<DotNetSolutionState>>();

		var dotNetSolutionModel = dotNetSolutionStateWrap.Value.DotNetSolutionsList.FirstOrDefault(x =>
    		x.AbsolutePath.Value == dotNetSolutionResource.ResourceUri.Value);

		foreach (var project in dotNetSolutionModel.DotNetProjectList)
		{
			var resourceUri = new ResourceUri(project.AbsolutePath.Value);

	        await textEditorConfig.RegisterModelFunc.Invoke(new RegisterModelArgs(
	                resourceUri,
	                serviceProvider))
	            .ConfigureAwait(false);
		}
		
		/*
		if (textEditorConfig.TryRegisterViewModelFunc is not null)
        {
            var viewModelKey = await textEditorConfig.TryRegisterViewModelFunc.Invoke(new TryRegisterViewModelArgs(
                    Key<TextEditorViewModel>.NewKey(),
                    dotNetSolutionResource.ResourceUri,
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
            }
        }
		*/
	}
}
