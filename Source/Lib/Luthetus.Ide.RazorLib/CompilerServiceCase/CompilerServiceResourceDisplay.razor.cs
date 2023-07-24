using Luthetus.CompilerServices.Lang.DotNetSolution.CompilerServiceCase;
using Luthetus.CompilerServices.Lang.CSharpProject.CompilerServiceCase;
using Luthetus.TextEditor.RazorLib.CompilerServiceCase;
using Microsoft.AspNetCore.Components;
using Luthetus.CompilerServices.Lang.CSharp.CompilerServiceCase;
using Luthetus.Common.RazorLib.FileSystem.Interfaces;

namespace Luthetus.Ide.RazorLib.CompilerServiceCase;

public partial class CompilerServiceResourceDisplay : ComponentBase
{
    [Inject]
    private CSharpProjectCompilerService CSharpProjectCompilerService { get; set; } = null!;
    [Inject]
    private IEnvironmentProvider EnvironmentProvider { get; set; } = null!;

    [CascadingParameter(Name = "CompilerServiceDisplayDimensions")]
    public CompilerServiceDisplayDimensions CompilerServiceDisplayDimensions { get; set; } = null!;

    [Parameter, EditorRequired]
    public ICompilerServiceResource CompilerServiceResource { get; set; } = null!;
    [Parameter, EditorRequired]
    public int IndexAmongSiblings { get; set; }
    [Parameter, EditorRequired]
    public int CountOfSiblingsAndSelf { get; set; }
    /// <summary>
    /// <see cref="Depth"/> begins at 0
    /// </summary>
    [Parameter, EditorRequired]
    public int Depth { get; set; }

    private double GetYCenterInPixels()
    {
        var verticalOffsetByDepth = (Depth + 1) * CompilerServiceDisplayDimensions.CircleRadiusInPixels;
        var svgPaddingTop = CompilerServiceDisplayDimensions.SvgPadding;

        return verticalOffsetByDepth + svgPaddingTop;
    }
    
    private double GetXCenterInPixels()
    {
        var percentageOfHorizontalSpacingBetweenEachCircle = 
            1.0 / (CountOfSiblingsAndSelf + 1.0);

        var horizontalSpacingBetweenEachCircle =
            CompilerServiceDisplayDimensions.ViewBoxWidth * percentageOfHorizontalSpacingBetweenEachCircle;

        var horizontalOffsetByMarginBetweenCircles = 
            IndexAmongSiblings * CompilerServiceDisplayDimensions.MinimumMarginRightBetweenSiblingsAndSelf;

        var svgPaddingLeft = CompilerServiceDisplayDimensions.SvgPadding;

        return horizontalSpacingBetweenEachCircle +
            horizontalOffsetByMarginBetweenCircles +
            svgPaddingLeft;
    }

    private string GetFill(ICompilerServiceResource localCompilerServiceResource)
    {
        if (localCompilerServiceResource is DotNetSolutionResource)
            return "var(--luth_icon-solution-font-color)";
        if (localCompilerServiceResource is CSharpProjectResource)
            return "var(--luth_icon-project-font-color)";
        if (localCompilerServiceResource is CSharpResource)
            return "var(--luth_icon-c-sharp-class-font-color)";
        
        return "currentColor";
    }
}