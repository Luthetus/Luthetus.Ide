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
                    Value = 0,
                    DimensionUnitKind = DimensionUnitKind.Pixels,
                    DimensionOperatorKind = DimensionOperatorKind.Subtract,
                    Purpose = DimensionUnitFacts.Purposes.OFFSET,
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
                    Value = 0,
                    DimensionUnitKind = DimensionUnitKind.Pixels,
                    DimensionOperatorKind = DimensionOperatorKind.Subtract,
                    Purpose = DimensionUnitFacts.Purposes.OFFSET,
                },
            });
        }
    }

    public ElementDimensions BodyElementDimensions { get; } = new();
	public ElementDimensions TabsElementDimensions { get; } = new();
}
