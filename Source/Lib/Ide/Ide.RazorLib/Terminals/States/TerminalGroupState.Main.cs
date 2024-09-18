using Fluxor;
using Luthetus.Common.RazorLib.Dimensions.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Resizes.Displays;
using Luthetus.Ide.RazorLib.Terminals.Models;

namespace Luthetus.Ide.RazorLib.Terminals.States;

[FeatureState]
public partial record TerminalGroupState(Key<ITerminal> ActiveTerminalKey)
{
    public TerminalGroupState() : this(TerminalFacts.GENERAL_KEY)
    {
        // _bodyElementDimensions
        {
            var widthDimensionAttribute = BodyElementDimensions.DimensionAttributeList.First(
                x => x.DimensionAttributeKind == DimensionAttributeKind.Width);

            widthDimensionAttribute.DimensionUnitList.AddRange(new[]
            {
                new DimensionUnit
                {
                    Value = 80,
                    DimensionUnitKind = DimensionUnitKind.Percentage,
                },
                new DimensionUnit
                {
                    Value = Luthetus.Common.RazorLib.Options.States.AppOptionsState.DEFAULT_RESIZE_HANDLE_WIDTH_IN_PIXELS / 2,
                    DimensionUnitKind = DimensionUnitKind.Pixels,
                    DimensionOperatorKind = DimensionOperatorKind.Subtract
                },
            });
        }

        // _tabsElementDimensions
        {
            var widthDimensionAttribute = TabsElementDimensions.DimensionAttributeList.First(
                x => x.DimensionAttributeKind == DimensionAttributeKind.Width);

            widthDimensionAttribute.DimensionUnitList.AddRange(new[]
            {
                new DimensionUnit
                {
                    Value = 20,
                    DimensionUnitKind = DimensionUnitKind.Percentage,
                },
                new DimensionUnit
                {
                    Value = Luthetus.Common.RazorLib.Options.States.AppOptionsState.DEFAULT_RESIZE_HANDLE_WIDTH_IN_PIXELS / 2,
                    DimensionUnitKind = DimensionUnitKind.Pixels,
                    DimensionOperatorKind = DimensionOperatorKind.Subtract
                },
            });
        }
    }

    public ElementDimensions BodyElementDimensions { get; } = new();
	public ElementDimensions TabsElementDimensions { get; } = new();
}
