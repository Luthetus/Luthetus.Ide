using Luthetus.CompilerServices.Lang.DotNetSolution.Obsolete.DotNetSolutionGlobalSectionTypes;

namespace Luthetus.CompilerServices.Lang.DotNetSolution.Obsolete;

public record DotNetSolutionGlobalSection(
    DotNetSolutionToken<NestedProjects> GlobalSectionNestedProjectsToken);
