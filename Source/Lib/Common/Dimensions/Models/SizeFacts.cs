namespace Luthetus.Common.RazorLib.Dimensions.Models;

/// <summary>
/// TODO: SphagettiCode - I don't like this file (2023-09-19)
/// Why does SizeFacts.cs, HtmlFacts.Button.cs, and HtmlFacts.Main.cs exist? Merge all three of these?
/// </summary>
public static class SizeFacts
{
    public static class Ide
    {
        public static class Header
        {
            public static readonly DimensionUnit Height = new()
            {
                Value = 3,
                DimensionUnitKind = DimensionUnitKind.RootCharacterHeight
            };
        }
    }
}