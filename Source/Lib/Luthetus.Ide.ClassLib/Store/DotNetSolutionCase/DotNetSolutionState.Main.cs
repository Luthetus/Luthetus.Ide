using System.Collections.Immutable;
using Fluxor;
using Luthetus.Ide.ClassLib.FileSystem.Classes.FilePath;
using Luthetus.Ide.ClassLib.InputFile;
using Luthetus.Ide.ClassLib.Namespaces;
using Luthetus.Ide.ClassLib.Store.InputFileCase;
using Luthetus.Ide.ClassLib.ComponentRenderers;
using Luthetus.Ide.ClassLib.DotNet;
using Luthetus.Ide.ClassLib.FileConstants;
using Luthetus.Ide.ClassLib.FileSystem.Interfaces;

namespace Luthetus.Ide.ClassLib.Store.DotNetSolutionCase;

[FeatureState]
public partial record DotNetSolutionState(
    DotNetSolution? DotNetSolution)
{
    private DotNetSolutionState() : this(default(DotNetSolution?))
    {
    }
    
    public static async Task SetActiveSolutionAsync(
        string solutionAbsolutePathString,
        IFileSystemProvider fileSystemProvider,
        IEnvironmentProvider environmentProvider,
        IDispatcher dispatcher)
    {
        var content = await fileSystemProvider.File.ReadAllTextAsync(
            solutionAbsolutePathString,
            CancellationToken.None);

        var solutionAbsoluteFilePath = new AbsoluteFilePath(
            solutionAbsolutePathString,
            false,
            environmentProvider);
        
        var solutionNamespacePath = new NamespacePath(
            string.Empty,
            solutionAbsoluteFilePath);

        var dotNetSolution = DotNetSolutionParser.Parse(
            content,
            solutionNamespacePath,
            environmentProvider);
        
        dispatcher.Dispatch(
            new WithAction(
                inDotNetSolutionState => inDotNetSolutionState with
                {
                    DotNetSolution = dotNetSolution
                }));
    }
    
    public static void ShowInputFile(
        IDispatcher dispatcher,
        ILuthetusIdeComponentRenderers luthetusIdeComponentRenderers,
        IFileSystemProvider fileSystemProvider,
        IEnvironmentProvider environmentProvider)
    {
        dispatcher.Dispatch(
            new InputFileState.RequestInputFileStateFormAction(
                "TextEditor",
                async afp =>
                {
                    await OpenInSolutionExplorerAsync(
                        afp, 
                        dispatcher,
                        luthetusIdeComponentRenderers,
                        fileSystemProvider,
                        environmentProvider);
                },
                afp =>
                {
                    if (afp is null ||
                        afp.IsDirectory)
                    {
                        return Task.FromResult(false);
                    }

                    return Task.FromResult(true);
                },
                new[]
                {
                    new InputFilePattern(
                        ".NET Solution",
                        afp => 
                            afp.ExtensionNoPeriod == ExtensionNoPeriodFacts.DOT_NET_SOLUTION)
                }.ToImmutableArray()));
    }
    
    public static async Task OpenInSolutionExplorerAsync(
        IAbsoluteFilePath? absoluteFilePath,
        IDispatcher dispatcher,
        ILuthetusIdeComponentRenderers luthetusIdeComponentRenderers,
        IFileSystemProvider fileSystemProvider,
        IEnvironmentProvider environmentProvider)
    {
        if (absoluteFilePath is null ||
            absoluteFilePath.IsDirectory)
        {
            return;
        }

        await SetActiveSolutionAsync(
            absoluteFilePath.GetAbsoluteFilePathString(),
            fileSystemProvider,
            environmentProvider,
            dispatcher);
    }
}