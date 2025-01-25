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
            BodyElementDimensions.WidthDimensionAttribute.DimensionUnitList.AddRange(new[]
            {
                new DimensionUnit(
                	80,
                	DimensionUnitKind.Percentage),
                new DimensionUnit(
                	0,
                	DimensionUnitKind.Pixels,
                	DimensionOperatorKind.Subtract,
                	DimensionUnitFacts.Purposes.OFFSET),
            });
        }

        // _tabsElementDimensions
        {
            TabsElementDimensions.WidthDimensionAttribute.DimensionUnitList.AddRange(new[]
            {
                new DimensionUnit(
                	20,
                	DimensionUnitKind.Percentage),
                new DimensionUnit(
                	0,
                	DimensionUnitKind.Pixels,
                	DimensionOperatorKind.Subtract,
                	DimensionUnitFacts.Purposes.OFFSET),
            });
        }
    }

    public ElementDimensions BodyElementDimensions { get; } = new();
	public ElementDimensions TabsElementDimensions { get; } = new();
}
