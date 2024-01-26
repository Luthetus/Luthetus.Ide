using Luthetus.Ide.Wasm.Facts;
using Microsoft.AspNetCore.Components;
using Luthetus.Ide.RazorLib.DotNetSolutions.States;
using Luthetus.Ide.RazorLib.Editors.States;
using Luthetus.TextEditor.RazorLib.Lexes.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.TreeViews.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib.Decorations.Models;
using Luthetus.TextEditor.RazorLib.Diffs.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorServices;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorModels;
using Luthetus.Ide.RazorLib.Websites.ProjectTemplates.Models;
using Fluxor;

namespace Luthetus.Website.RazorLib;

public partial class LuthetusWebsiteInitializer : ComponentBase
{
    [Inject]
    private IEnvironmentProvider EnvironmentProvider { get; set; } = null!;
    [Inject]
    private IFileSystemProvider FileSystemProvider { get; set; } = null!;
    [Inject]
    private ITreeViewService TreeViewService { get; set; } = null!;
    [Inject]
    private ITextEditorService TextEditorService { get; set; } = null!;
    [Inject]
    private ITextEditorRegistryWrap TextEditorRegistryWrap { get; set; } = null!;
    [Inject]
    private IDecorationMapperRegistry DecorationMapperRegistry { get; set; } = null!;
    [Inject]
    private ICompilerServiceRegistry CompilerServiceRegistry { get; set; } = null!;
    [Inject]
    private IBackgroundTaskService BackgroundTaskService { get; set; } = null!;
    [Inject]
    private DotNetSolutionSync DotNetSolutionSync { get; set; } = null!;
    [Inject]
    private IState<DotNetSolutionState> DotNetSolutionStateWrap { get; set; } = null!;
    [Inject]
    private EditorSync EditorSync { get; set; } = null!;

    protected override void OnInitialized()
    {
        TextEditorRegistryWrap.DecorationMapperRegistry = DecorationMapperRegistry;
        TextEditorRegistryWrap.CompilerServiceRegistry = CompilerServiceRegistry;

        base.OnInitialized();
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            BackgroundTaskService.Enqueue(Key<BackgroundTask>.NewKey(), ContinuousBackgroundTaskWorker.GetQueueKey(),
                "Initialize Website",
                async () =>
                {
                    await WriteFileSystemInMemoryAsync();

                    await ParseSolutionAsync();

                    // This code block is hacky. I want the Solution Explorer to from the get-go be fully expanded, so the user can see 'Program.cs'
                    {
                        TreeViewService.MoveRight(
							DotNetSolutionState.TreeViewSolutionExplorerStateKey,
							false,
							false);

                        TreeViewService.MoveRight(
							DotNetSolutionState.TreeViewSolutionExplorerStateKey,
							false,
							false);
                        
						TreeViewService.MoveRight(
							DotNetSolutionState.TreeViewSolutionExplorerStateKey,
							false,
							false);
                    }
                });
        }

        await base.OnAfterRenderAsync(firstRender);
    }

    private async Task WriteFileSystemInMemoryAsync()
    {
        // Create a Blazor Wasm app
        await WebsiteProjectTemplateFacts.HandleNewCSharpProjectAsync(
            WebsiteProjectTemplateFacts.BlazorWasmEmptyProjectTemplate.ShortName!,
            InitialSolutionFacts.BLAZOR_CRUD_APP_WASM_CSPROJ_ABSOLUTE_FILE_PATH,
            FileSystemProvider,
            EnvironmentProvider);

        // ExampleSolution.sln
        await FileSystemProvider.File.WriteAllTextAsync(
            InitialSolutionFacts.SLN_ABSOLUTE_FILE_PATH,
            InitialSolutionFacts.SLN_CONTENTS);

        var solutionAbsolutePath = new AbsolutePath(
            InitialSolutionFacts.SLN_ABSOLUTE_FILE_PATH,
            false,
            EnvironmentProvider);

        DotNetSolutionSync.SetDotNetSolution(solutionAbsolutePath);

        // Display a file from the get-go so the user is less confused on what the website is.
        var absolutePath = new AbsolutePath(
            InitialSolutionFacts.BLAZOR_CRUD_APP_WASM_PROGRAM_CS_ABSOLUTE_FILE_PATH,
            false,
            EnvironmentProvider);

        EditorSync.OpenInEditor(absolutePath, false);
    }

    private async Task ParseSolutionAsync()
    {
        var allFiles = new List<string>();

        await RecursiveStep(
            new List<string> { "/" },
            allFiles);

        async Task RecursiveStep(IEnumerable<string> directories, List<string> allFiles)
        {
            foreach (var directory in directories)
            {
                var childDirectories = await FileSystemProvider.Directory
                    .GetDirectoriesAsync(directory);

                allFiles.AddRange(await FileSystemProvider.Directory
                    .GetFilesAsync(directory));

                await RecursiveStep(childDirectories, allFiles);
            }
        }

        foreach (var file in allFiles)
        {
            var absolutePath = new AbsolutePath(file, false, EnvironmentProvider);
            var resourceUri = new ResourceUri(file);
            var fileLastWriteTime = await FileSystemProvider.File.GetLastWriteTimeAsync(file);
            var content = await FileSystemProvider.File.ReadAllTextAsync(file);
            
            var decorationMapper = DecorationMapperRegistry.GetDecorationMapper(absolutePath.ExtensionNoPeriod);
            var compilerService = CompilerServiceRegistry.GetCompilerService(absolutePath.ExtensionNoPeriod);

            var textEditorModel = new TextEditorModel(
                resourceUri,
                fileLastWriteTime,
                absolutePath.ExtensionNoPeriod,
                content,
                decorationMapper,
                compilerService);

            TextEditorService.ModelApi.RegisterCustom(textEditorModel);

            TextEditorService.Post(
                nameof(TextEditorService.ModelApi.AddPresentationModelFactory),
                async editContext =>
                {
                    await TextEditorService.ModelApi.AddPresentationModelFactory(
                            textEditorModel.ResourceUri,
                            CompilerServiceDiagnosticPresentationFacts.EmptyPresentationModel)
                        .Invoke(editContext);

                    await TextEditorService.ModelApi.AddPresentationModelFactory(
                            textEditorModel.ResourceUri,
                            DiffPresentationFacts.EmptyInPresentationModel)
                        .Invoke(editContext);

                    await TextEditorService.ModelApi.AddPresentationModelFactory(
                            textEditorModel.ResourceUri,
                            DiffPresentationFacts.EmptyOutPresentationModel)
                        .Invoke(editContext);

                    textEditorModel.CompilerService.RegisterResource(textEditorModel.ResourceUri);
                });
        }
    }
}