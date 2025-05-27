using Luthetus.Common.RazorLib.TreeViews.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib.TextEditors.Models;
using Luthetus.CompilerServices.DotNetSolution.Models;
using Luthetus.Ide.RazorLib.BackgroundTasks.Models;
using Luthetus.Ide.RazorLib.InputFiles.Models;
using Luthetus.Extensions.DotNet.BackgroundTasks.Models;

namespace Luthetus.Extensions.DotNet.DotNetSolutions.Models;

/// <summary>
/// TODO: Investigate making this a record struct
/// TODO: 'Key<DotNetSolutionModel>? DotNetSolutionModelKey' should not be nullable use Key<DotNetSolutionModel>.Empty.
/// </summary>
public record DotNetSolutionState(
    Key<DotNetSolutionModel>? DotNetSolutionModelKey,
    int IsExecutingAsyncTaskLinks)
{
    public static readonly Key<TreeViewContainer> TreeViewSolutionExplorerStateKey = Key<TreeViewContainer>.NewKey();

    public DotNetSolutionState() : this(Key<DotNetSolutionModel>.Empty, 0)
    {
    }

    public List<DotNetSolutionModel> DotNetSolutionsList { get; init; } = new();

    public DotNetSolutionModel? DotNetSolutionModel => DotNetSolutionsList.FirstOrDefault(x =>
        x.Key == DotNetSolutionModelKey);

    public static void ShowInputFile(
    	IdeBackgroundTaskApi ideBackgroundTaskApi,
    	DotNetBackgroundTaskApi compilerServicesBackgroundTaskApi)
    {
        ideBackgroundTaskApi.InputFile.Enqueue(new InputFileIdeApiWorkArgs
		{
			WorkKind = InputFileIdeApiWorkKind.RequestInputFileStateForm,
			Message = "Solution Explorer",
            OnAfterSubmitFunc = absolutePath =>
            {
                if (absolutePath.ExactInput is not null)
                    compilerServicesBackgroundTaskApi.DotNetSolution.Enqueue(new DotNetSolutionIdeWorkArgs
                    {
                    	WorkKind = DotNetSolutionIdeWorkKind.SetDotNetSolution,
                    	DotNetSolutionAbsolutePath = absolutePath,
                    });

				return Task.CompletedTask;
            },
            SelectionIsValidFunc = absolutePath =>
            {
                if (absolutePath.ExactInput is null || absolutePath.IsDirectory)
                    return Task.FromResult(false);

                return Task.FromResult(
                    absolutePath.ExtensionNoPeriod == ExtensionNoPeriodFacts.DOT_NET_SOLUTION ||
    				absolutePath.ExtensionNoPeriod == ExtensionNoPeriodFacts.DOT_NET_SOLUTION_X);
            },
            InputFilePatterns = new()
            {
                new InputFilePattern(
                    ".NET Solution",
                    absolutePath => absolutePath.ExtensionNoPeriod == ExtensionNoPeriodFacts.DOT_NET_SOLUTION ||
                    				absolutePath.ExtensionNoPeriod == ExtensionNoPeriodFacts.DOT_NET_SOLUTION_X)
            }
        });
    }
}