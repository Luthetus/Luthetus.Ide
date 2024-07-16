using System.Collections.Immutable;
using Microsoft.Extensions.DependencyInjection;
using Fluxor;
using Luthetus.Common.RazorLib.Menus.Models;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Notifications.Models;
using Luthetus.Common.RazorLib.ComponentRenderers.Models;
using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;
using Luthetus.TextEditor.RazorLib.Installations.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.TextEditor.RazorLib.Groups.Models;
using Luthetus.CompilerServices.DotNetSolution.CompilerServiceCase;
using Luthetus.CompilerServices.CSharpProject.CompilerServiceCase;
using Luthetus.CompilerServices.RazorLib.DotNetSolutions.States;

namespace Luthetus.CompilerServices.RazorLib.DotNetSolutions.Models.Internals;

public class SolutionVisualizationDrawingCircle<TItem> : ISolutionVisualizationDrawingCircle
{
	public TItem Item { get; set; }
	public SolutionVisualizationDrawingKind SolutionVisualizationDrawingKind => SolutionVisualizationDrawingKind.Circle;
	public SolutionVisualizationItemKind SolutionVisualizationItemKind { get; set; }
	public int CenterX { get; set; }
	public int CenterY { get; set; }
	public int Radius { get; set; }
	public string Fill { get; set; }
	public int RenderCycle { get; set; }
	public int RenderCycleSequence { get; set; }

	object ISolutionVisualizationDrawing.Item => Item;

	public MenuOptionRecord GetMenuOptionRecord(
		SolutionVisualizationModel localSolutionVisualizationModel,
		IEnvironmentProvider environmentProvider,
		LuthetusTextEditorConfig textEditorConfig,
		IServiceProvider serviceProvider)
	{
		var menuOptionRecordList = new List<MenuOptionRecord>();
		
		var targetDisplayName = "unknown";

		if (Item is ICompilerServiceResource compilerServiceResource)
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

			if (SolutionVisualizationItemKind == SolutionVisualizationItemKind.Solution)
			{
				menuOptionRecordList.Add(new MenuOptionRecord(
				    "Load Projects",
				    MenuOptionKind.Other,
					OnClickFunc: () => LoadProjects(
						(DotNetSolutionResource)compilerServiceResource,
						textEditorConfig,
						serviceProvider)));			
			}
			else if (SolutionVisualizationItemKind == SolutionVisualizationItemKind.Project)
			{
				menuOptionRecordList.Add(new MenuOptionRecord(
				    "Load Classes",
				    MenuOptionKind.Other,
					OnClickFunc: () => LoadClasses(
						(CSharpProjectResource)compilerServiceResource,
						localSolutionVisualizationModel,
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
	}

	private async Task LoadClasses(
		CSharpProjectResource cSharpProjectResource,
		SolutionVisualizationModel localSolutionVisualizationModel,
		LuthetusTextEditorConfig textEditorConfig,
		IServiceProvider serviceProvider)
	{
        if (textEditorConfig.RegisterModelFunc is null)
            return;

		var environmentProvider = serviceProvider.GetRequiredService<IEnvironmentProvider>();
		var fileSystemProvider = serviceProvider.GetRequiredService<IFileSystemProvider>();
		var commonComponentRenderers = serviceProvider.GetRequiredService<ICommonComponentRenderers>();
		var dispatcher = serviceProvider.GetRequiredService<IDispatcher>();

		var projectAbsolutePath = environmentProvider.AbsolutePathFactory(cSharpProjectResource.ResourceUri.Value, false);

		var parentDirectory = projectAbsolutePath.ParentDirectory;
		if (parentDirectory is null)
	        return;

		var startingAbsolutePathForSearch = parentDirectory.Value;

		var discoveredFileList = new List<string>();

		await DiscoverFilesRecursively(startingAbsolutePathForSearch, discoveredFileList).ConfigureAwait(false);

		NotificationHelper.DispatchInformative(
	        "SolutionVisualizationDrawing",
	        $"{projectAbsolutePath.NameWithExtension} discovery {discoveredFileList.Count} C# files",
	        commonComponentRenderers,
	        dispatcher,
	        TimeSpan.FromSeconds(6));

		foreach (var file in discoveredFileList)
		{
			if (!localSolutionVisualizationModel.ParentMap.TryAdd(file, this))
				localSolutionVisualizationModel.ParentMap[file] = this;
		}

		foreach (var file in discoveredFileList)
		{
			var resourceUri = new ResourceUri(file);

	        await textEditorConfig.RegisterModelFunc.Invoke(new RegisterModelArgs(
	                resourceUri,
	                serviceProvider))
	            .ConfigureAwait(false);
		}

        async Task DiscoverFilesRecursively(string directoryPathParent, List<string> discoveredFileList)
        {
            var directoryPathChildList = await fileSystemProvider.Directory.GetDirectoriesAsync(
                    directoryPathParent,
                    CancellationToken.None)
                .ConfigureAwait(false);

            var filePathChildList = await fileSystemProvider.Directory.GetFilesAsync(
                    directoryPathParent,
                    CancellationToken.None)
                .ConfigureAwait(false);

            foreach (var filePathChild in filePathChildList)
            {
                if (filePathChild.EndsWith(".cs"))
                    discoveredFileList.Add(filePathChild);
            }

            foreach (var directoryPathChild in directoryPathChildList)
            {
                if (directoryPathChild.Contains(".vs") || directoryPathChild.Contains(".git") || directoryPathChild.Contains("bin") || directoryPathChild.Contains("obj"))
                    continue;

                await DiscoverFilesRecursively(directoryPathChild, discoveredFileList).ConfigureAwait(false);
            }
        }
	}
}
