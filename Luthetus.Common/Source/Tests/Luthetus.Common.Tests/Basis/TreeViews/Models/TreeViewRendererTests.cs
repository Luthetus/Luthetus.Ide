namespace Luthetus.Common.RazorLib.TreeViews.Models;

public record TreeViewRendererTests(
    Type DynamicComponentType,
    Dictionary<string, object?>? DynamicComponentParameters);