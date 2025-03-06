using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.Extensions.CompilerServices.Syntax;
using Luthetus.CompilerServices.DotNetSolution.Models.Associated;

namespace Luthetus.CompilerServices.DotNetSolution.Models;

/// <summary>
/// GlobalSection(SolutionConfigurationPlatforms) = preSolution<br/>
/// &#x9;Debug|Any CPU = Debug|Any CPU<br/>
/// &#x9;Release|Any CPU = Release|Any CPU<br/>
/// EndGlobalSection<br/>
/// </summary>
public record DotNetSolutionGlobalSection
{
    public DotNetSolutionGlobalSection(
        SyntaxToken? globalSectionArgument,
        SyntaxToken? globalSectionOrder,
        AssociatedEntryGroup associatedEntryGroup)
    {
        GlobalSectionArgument = globalSectionArgument;
        GlobalSectionOrder = globalSectionOrder;
        AssociatedEntryGroup = associatedEntryGroup;
    }

    /// <summary>
    /// Given: ...GlobalSection(SolutionConfigurationPlatforms) = preSolution...<br/>
    /// Then: "SolutionConfigurationPlatforms" is the <see cref="GlobalSectionArgument"/>
    /// </summary>
    public SyntaxToken? GlobalSectionArgument { get; init; }
    /// <summary>
    /// Given: ...GlobalSection(SolutionConfigurationPlatforms) = preSolution...<br/>
    /// Then: "preSolution" is the <see cref="GlobalSectionOrder"/>
    /// </summary>
    public SyntaxToken? GlobalSectionOrder { get; init; }
    /// <summary>
    /// Given: GlobalSection(ProjectConfigurationPlatforms) = postSolution<br/>
	/// &#x9;{623099D9-D9DE-47E8-8CCD-BC301C82F70F}.Debug|Any CPU.ActiveCfg = Debug|Any CPU<br/>
	/// &#x9;{623099D9-D9DE-47E8-8CCD-BC301C82F70F}.Debug|Any CPU.Build.0 = Debug|Any CPU<br/>
	/// &#x9;{623099D9-D9DE-47E8-8CCD-BC301C82F70F}.Release|Any CPU.ActiveCfg = Release|Any CPU<br/>
	/// &#x9;{623099D9-D9DE-47E8-8CCD-BC301C82F70F}.Release|Any CPU.Build.0 = Release|Any CPU<br/>
	/// EndGlobalSection<br/>
    /// <br/>
    /// Then: new [<br/>
    ///     {623099D9-D9DE-47E8-8CCD-BC301C82F70F}.Debug|Any CPU.ActiveCfg = Debug|Any CPU,<br/>
    ///     {623099D9-D9DE-47E8-8CCD-BC301C82F70F}.Debug|Any CPU.Build.0 = Debug|Any CPU,<br/>
    ///     {623099D9-D9DE-47E8-8CCD-BC301C82F70F}.Release|Any CPU.ActiveCfg = Release|Any CPU,<br/>
    ///     {623099D9-D9DE-47E8-8CCD-BC301C82F70F}.Release|Any CPU.Build.0 = Release|Any CPU,<br/>
    /// ]<br/>
    /// is the <see cref="AssociatedEntryGroup"/>
    /// </summary>
    public AssociatedEntryGroup AssociatedEntryGroup { get; init; }
}
