using Microsoft.AspNetCore.Components;

namespace Luthetus.Ide.RazorLib.CompilerServiceCase;

public partial class CompilerServiceControlsDisplay : ComponentBase
{
    public CompilerServiceControlsDisplay()
    {
        ViewBoxWidthInputId = $"luth_ide_compiler-service-controls_view-box-width-input_{ComponentIdentifier}";
        ViewBoxHeightInputId = $"luth_ide_compiler-service-controls_view-box-height-input_{ComponentIdentifier}";
        CircleRadiusInPixelsInputId = $"luth_ide_compiler-service-controls_circle-radius-in-pixels-input_{ComponentIdentifier}";
        MinimumMarginRightBetweenSiblingsAndSelfInputId = $"luth_ide_compiler-service-controls_minimum-margin-right-between-siblings-and-self-input_{ComponentIdentifier}";
        MinimumMarginBottomBetweenRowsInputId = $"luth_ide_compiler-service-controls_minimum-margin-bottom-between-rows-input_{ComponentIdentifier}";
        SvgPaddingInputId = $"luth_ide_compiler-service-controls_svg-padding-input_{ComponentIdentifier}";
        SvgFontSizeInPixelsInputId = $"luth_ide_compiler-service-controls_svg-font-size-in-pixels-input_{ComponentIdentifier}";
    }

    [Parameter, EditorRequired]
    public CompilerServiceDisplayDimensions CompilerServiceDisplayDimensions { get; set; } = null!;
    [Parameter, EditorRequired]
    public Func<CompilerServiceDisplayDimensions, Task> DimensionsStateHasChangedFunc { get; set; } = null!;

    private const int MINIMUM_VIEW_BOX_WIDTH = 5;
    private const int MINIMUM_VIEW_BOX_HEIGHT = 5;
    private const int MINIMUM_CIRCLE_RADIUS_IN_PIXELS = 5;
    private const int MINIMUM_MINIMUM_MARGIN_RIGHT_BETWEEN_SIBLINGS_AND_SELF = 1;
    private const int MINIMUM_MINIMUM_MARGIN_BOTTOM_BETWEEN_ROWS = 1;
    private const int MINIMUM_SVG_PADDING = 1;
    private const int MINIMUM_SVG_FONT_SIZE_IN_PIXELS = 1;

    private readonly Guid ComponentIdentifier = Guid.NewGuid();

    private readonly string ViewBoxWidthInputId;
    private readonly string ViewBoxHeightInputId;
    private readonly string CircleRadiusInPixelsInputId;
    private readonly string MinimumMarginRightBetweenSiblingsAndSelfInputId;
    private readonly string MinimumMarginBottomBetweenRowsInputId;
    private readonly string SvgPaddingInputId;
    private readonly string SvgFontSizeInPixelsInputId;

    private int InputViewBoxWidth
    {
        get => CompilerServiceDisplayDimensions.ViewBoxWidth;
        set
        {
            var validatedValue = value;

            if (value < MINIMUM_VIEW_BOX_WIDTH)
                validatedValue = MINIMUM_VIEW_BOX_WIDTH;

            DimensionsStateHasChangedFunc.Invoke(CompilerServiceDisplayDimensions with
            {
                ViewBoxWidth = validatedValue
            });
        }
    }

    private int InputViewBoxHeight
    {
        get => CompilerServiceDisplayDimensions.ViewBoxHeight;
        set
        {
            var validatedValue = value;

            if (value < MINIMUM_VIEW_BOX_HEIGHT)
                validatedValue = MINIMUM_VIEW_BOX_HEIGHT;

            DimensionsStateHasChangedFunc.Invoke(CompilerServiceDisplayDimensions with
            {
                ViewBoxHeight = validatedValue
            });
        }
    }
    
    private double InputCircleRadiusInPixels
    {
        get => CompilerServiceDisplayDimensions.CircleRadiusInPixels;
        set
        {
            var validatedValue = value;

            if (value < MINIMUM_CIRCLE_RADIUS_IN_PIXELS)
                validatedValue = MINIMUM_CIRCLE_RADIUS_IN_PIXELS;

            DimensionsStateHasChangedFunc.Invoke(CompilerServiceDisplayDimensions with
            {
                CircleRadiusInPixels = validatedValue
            });
        }
    }
    
    private double InputMinimumMarginRightBetweenSiblingsAndSelf
    {
        get => CompilerServiceDisplayDimensions.MinimumMarginRightBetweenSiblingsAndSelf;
        set
        {
            var validatedValue = value;

            if (value < MINIMUM_MINIMUM_MARGIN_RIGHT_BETWEEN_SIBLINGS_AND_SELF)
                validatedValue = MINIMUM_MINIMUM_MARGIN_RIGHT_BETWEEN_SIBLINGS_AND_SELF;

            DimensionsStateHasChangedFunc.Invoke(CompilerServiceDisplayDimensions with
            {
                MinimumMarginRightBetweenSiblingsAndSelf = validatedValue
            });
        }
    }
    
    private double InputMinimumMarginBottomBetweenRows
    {
        get => CompilerServiceDisplayDimensions.MinimumMarginBottomBetweenRows;
        set
        {
            var validatedValue = value;

            if (value < MINIMUM_MINIMUM_MARGIN_BOTTOM_BETWEEN_ROWS)
                validatedValue = MINIMUM_MINIMUM_MARGIN_BOTTOM_BETWEEN_ROWS;

            DimensionsStateHasChangedFunc.Invoke(CompilerServiceDisplayDimensions with
            {
                MinimumMarginBottomBetweenRows = validatedValue
            });
        }
    }
    
    private double InputSvgPadding
    {
        get => CompilerServiceDisplayDimensions.SvgPadding;
        set
        {
            var validatedValue = value;

            if (value < MINIMUM_SVG_PADDING)
                validatedValue = MINIMUM_SVG_PADDING;

            DimensionsStateHasChangedFunc.Invoke(CompilerServiceDisplayDimensions with
            {
                SvgPadding = validatedValue
            });
        }
    }
    
    private double InputSvgFontSizeInPixels
    {
        get => CompilerServiceDisplayDimensions.SvgFontSizeInPixels;
        set
        {
            var validatedValue = value;

            if (value < MINIMUM_SVG_FONT_SIZE_IN_PIXELS)
                validatedValue = MINIMUM_SVG_FONT_SIZE_IN_PIXELS;

            DimensionsStateHasChangedFunc.Invoke(CompilerServiceDisplayDimensions with
            {
                SvgFontSizeInPixels = validatedValue
            });
        }
    }
}