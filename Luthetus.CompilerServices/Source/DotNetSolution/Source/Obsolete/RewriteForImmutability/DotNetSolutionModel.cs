using Luthetus.Common.RazorLib.FileSystems.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Namespaces.Models;
using Luthetus.CompilerServices.Lang.DotNetSolution.Obsolete.DotNetSolutionGlobalSectionTypes;
using System.Collections.Immutable;

namespace Luthetus.CompilerServices.Lang.DotNetSolution.Obsolete.RewriteForImmutability;

public class DotNetSolutionModel : IDotNetSolution
{
    private readonly ImmutableList<DotNetSolutionToken<IDotNetProject>> _projectTokens;
    private readonly ImmutableList<DotNetSolutionToken<DotNetSolutionFolder>> _solutionFolderTokens;
    private readonly DotNetSolutionToken<DotNetSolutionGlobal>? _globalToken;
    private readonly DotNetSolutionToken<DotNetSolutionGlobalSection> _globalSectionToken;
    private readonly DotNetSolutionToken<NestedProjects> _nestedProjectsToken;

    public DotNetSolutionModel(
        Key<DotNetSolutionModel> dotNetSolutionModelKey,
        NamespacePath namespacePath,
        string solutionFileContents,
        List<DotNetSolutionToken<IDotNetProject>> projectTokens,
        List<DotNetSolutionToken<DotNetSolutionFolder>> solutionFolderTokens,
        DotNetSolutionToken<DotNetSolutionGlobal>? globalToken,
        DotNetSolutionToken<DotNetSolutionGlobalSection> globalSectionToken,
        DotNetSolutionToken<NestedProjects> nestedProjectsToken)
    {
        _projectTokens = projectTokens.ToImmutableList();
        _solutionFolderTokens = solutionFolderTokens.ToImmutableList();
        _globalToken = globalToken;
        _globalSectionToken = globalSectionToken;
        _nestedProjectsToken = nestedProjectsToken;

        DotNetSolutionModelKey = dotNetSolutionModelKey;
        NamespacePath = namespacePath;
        SolutionFileContents = solutionFileContents;
    }

    public Key<DotNetSolutionModel> DotNetSolutionModelKey { get; }
    public NamespacePath NamespacePath { get; }
    public string SolutionFileContents { get; private set; }

    public ImmutableArray<IDotNetProject> DotNetProjects => _projectTokens
        .Select(pt => pt.Token)
        .ToImmutableArray();

    public ImmutableArray<DotNetSolutionFolder> SolutionFolders => _solutionFolderTokens
        .Select(sft => sft.Token)
        .ToImmutableArray();

    public ImmutableArray<NestedProjectEntry> NestedProjectEntries => _nestedProjectsToken.Token.NestedProjectEntries;

    public DotNetSolutionModelBuilder ToBuilder()
    {
        var dotNetSolutionModelBuilder = new DotNetSolutionModelBuilder(
            DotNetSolutionModelKey,
            NamespacePath,
            SolutionFileContents,
            _projectTokens.ToList(),
            _solutionFolderTokens.ToList(),
            _globalToken,
            _globalSectionToken,
            _nestedProjectsToken);

        return dotNetSolutionModelBuilder;
    }

    public DotNetSolutionModelBuilder AddDotNetProject(
        IDotNetProject dotNetProject,
        IEnvironmentProvider environmentProvider)
    {
        return ToBuilder().AddDotNetProject(
            dotNetProject,
            environmentProvider);
    }
}
