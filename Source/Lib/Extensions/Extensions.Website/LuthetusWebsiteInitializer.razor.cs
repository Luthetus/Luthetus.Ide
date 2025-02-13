using Microsoft.AspNetCore.Components;
using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Common.RazorLib.BackgroundTasks.Models;
using Luthetus.Common.RazorLib.TreeViews.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Installations.Models;
using Luthetus.TextEditor.RazorLib.Decorations.Models;
using Luthetus.TextEditor.RazorLib.Diffs.Models;
using Luthetus.TextEditor.RazorLib.Lexers.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;
using Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;
using Luthetus.TextEditor.RazorLib.CompilerServices.Facts;
using Luthetus.TextEditor.RazorLib;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.Extensions.DotNet.DotNetSolutions.Models;
using Luthetus.Extensions.DotNet.Websites.ProjectTemplates.Models;
using Luthetus.Extensions.DotNet.BackgroundTasks.Models;
using Luthetus.Ide.RazorLib.BackgroundTasks.Models;
using Luthetus.Ide.Wasm.Facts;

namespace Luthetus.Website.RazorLib;

public partial class LuthetusWebsiteInitializer : ComponentBase
{
    [Inject]
    private LuthetusCommonApi CommonApi { get; set; } = null!;
    [Inject]
    private ITextEditorService TextEditorService { get; set; } = null!;
    [Inject]
    private ITextEditorRegistryWrap TextEditorRegistryWrap { get; set; } = null!;
    [Inject]
    private IDecorationMapperRegistry DecorationMapperRegistry { get; set; } = null!;
    [Inject]
    private ICompilerServiceRegistry CompilerServiceRegistry { get; set; } = null!;
    [Inject]
    private ITextEditorHeaderRegistry TextEditorHeaderRegistry { get; set; } = null!;
    [Inject]
    private IdeBackgroundTaskApi IdeBackgroundTaskApi { get; set; } = null!;
    [Inject]
    private DotNetBackgroundTaskApi DotNetBackgroundTaskApi { get; set; } = null!;

    protected override void OnInitialized()
    {
        TextEditorRegistryWrap.DecorationMapperRegistry = DecorationMapperRegistry;
        TextEditorRegistryWrap.CompilerServiceRegistry = CompilerServiceRegistry;

        base.OnInitialized();
    }

    protected override Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            BackgroundTaskService.Enqueue(
                Key<IBackgroundTask>.NewKey(),
                BackgroundTaskFacts.ContinuousQueueKey,
                "Initialize Website",
                async () =>
                {
                    await WriteFileSystemInMemoryAsync().ConfigureAwait(false);

                    await ParseSolutionAsync().ConfigureAwait(false);

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

        return base.OnAfterRenderAsync(firstRender);
    }

    private async Task WriteFileSystemInMemoryAsync()
    {
        // Create a Blazor Wasm app
        await WebsiteProjectTemplateFacts.HandleNewCSharpProjectAsync(
                WebsiteProjectTemplateFacts.BlazorWasmEmptyProjectTemplate.ShortName!,
                InitialSolutionFacts.BLAZOR_CRUD_APP_WASM_CSPROJ_ABSOLUTE_FILE_PATH,
                FileSystemProvider,
                EnvironmentProvider)
            .ConfigureAwait(false);

        await FileSystemProvider.File.WriteAllTextAsync(
                InitialSolutionFacts.PERSON_CS_ABSOLUTE_FILE_PATH,
                InitialSolutionFacts.PERSON_CS_CONTENTS)
            .ConfigureAwait(false);
        
        await FileSystemProvider.File.WriteAllTextAsync(
                InitialSolutionFacts.PERSON_DISPLAY_RAZOR_CS_ABSOLUTE_FILE_PATH,
                InitialSolutionFacts.PERSON_DISPLAY_RAZOR_CS_CONTENTS)
            .ConfigureAwait(false);
        
        await FileSystemProvider.File.WriteAllTextAsync(
                InitialSolutionFacts.PERSON_DISPLAY_RAZOR_ABSOLUTE_FILE_PATH,
                InitialSolutionFacts.PERSON_DISPLAY_RAZOR_CONTENTS)
            .ConfigureAwait(false);
            
		/*await FileSystemProvider.File.WriteAllTextAsync(
                InitialSolutionFacts.BLAZOR_CRUD_APP_ALL_C_SHARP_SYNTAX_ABSOLUTE_FILE_PATH,
                InitialSolutionFacts.BLAZOR_CRUD_APP_ALL_C_SHARP_SYNTAX_CONTENTS)
            .ConfigureAwait(false);*/
        
        // ExampleSolution.sln
        await FileSystemProvider.File.WriteAllTextAsync(
                InitialSolutionFacts.SLN_ABSOLUTE_FILE_PATH,
                InitialSolutionFacts.SLN_CONTENTS)
            .ConfigureAwait(false);

        var solutionAbsolutePath = EnvironmentProvider.AbsolutePathFactory(
            InitialSolutionFacts.SLN_ABSOLUTE_FILE_PATH,
            false);
        
        // This line is also in LuthetusExtensionsDotNetInitializer,
        // but its duplicated here because the website
        // won't open the first file correctly without this.
        TextEditorHeaderRegistry.UpsertHeader("cs", typeof(Luthetus.TextEditor.RazorLib.TextEditors.Displays.Internals.TextEditorCompilerServiceHeaderDisplay));

        DotNetBackgroundTaskApi.DotNetSolution.SetDotNetSolution(solutionAbsolutePath);

        // Display a file from the get-go so the user is less confused on what the website is.
        var absolutePath = EnvironmentProvider.AbsolutePathFactory(
            InitialSolutionFacts.PERSON_CS_ABSOLUTE_FILE_PATH,
            false);

        await TextEditorService.OpenInEditorAsync(
			absolutePath.Value,
			false,
			null,
			new Category("main"),
			Key<TextEditorViewModel>.NewKey());
    }

    private async Task ParseSolutionAsync()
    {
        var allFiles = new List<string>();

        await RecursiveStep(
                new List<string> { "/" },
                allFiles)
            .ConfigureAwait(false);

        async Task RecursiveStep(IEnumerable<string> directories, List<string> allFiles)
        {
            foreach (var directory in directories)
            {
                var childDirectories = await FileSystemProvider.Directory
                    .GetDirectoriesAsync(directory)
                    .ConfigureAwait(false);

                allFiles.AddRange(
                    await FileSystemProvider.Directory.GetFilesAsync(directory).ConfigureAwait(false));

                await RecursiveStep(childDirectories, allFiles).ConfigureAwait(false);
            }
        }

        foreach (var file in allFiles)
        {
            var absolutePath = EnvironmentProvider.AbsolutePathFactory(file, false);
            var resourceUri = new ResourceUri(file);
            var fileLastWriteTime = await FileSystemProvider.File.GetLastWriteTimeAsync(file).ConfigureAwait(false);
            var content = await FileSystemProvider.File.ReadAllTextAsync(file).ConfigureAwait(false);
            
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

            TextEditorService.TextEditorWorker.PostUnique(
                nameof(TextEditorService.ModelApi.AddPresentationModel),
                editContext =>
                {
					var modelModifier = editContext.GetModelModifier(textEditorModel.ResourceUri);

					if (modelModifier is null)
						return ValueTask.CompletedTask;

					TextEditorService.ModelApi.AddPresentationModel(
                        editContext,
						modelModifier,
                        CompilerServiceDiagnosticPresentationFacts.EmptyPresentationModel);
                    
                    TextEditorService.ModelApi.AddPresentationModel(
						editContext,
						modelModifier,
                        FindOverlayPresentationFacts.EmptyPresentationModel);

                    TextEditorService.ModelApi.AddPresentationModel(
						editContext,
						modelModifier,
                        DiffPresentationFacts.EmptyInPresentationModel);

                    TextEditorService.ModelApi.AddPresentationModel(
                        editContext,
                        modelModifier,
                        DiffPresentationFacts.EmptyOutPresentationModel);

                    textEditorModel.CompilerService.RegisterResource(
                    	textEditorModel.ResourceUri,
                    	shouldTriggerResourceWasModified: true);
					return ValueTask.CompletedTask;
				});
        }
    }
}