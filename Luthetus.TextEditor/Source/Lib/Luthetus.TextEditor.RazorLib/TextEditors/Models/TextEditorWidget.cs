using Luthetus.Common.RazorLib.Dynamics.Models;
using Luthetus.Common.RazorLib.Keys.Models;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Models;

public record TextEditorWidget(
    Key<IDynamicViewModel> DynamicViewModelKey,
    string Title,
    string HtmlElementId,
    Type ComponentType,
    Dictionary<string, object?>? ComponentParameterMap);
