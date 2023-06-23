using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using System.Net.Http;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Routing;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

namespace Luthetus.Ide.Photino.TestApps.TestAppLuthetusCommonCase.InternalComponents.RenderCounter;

/// <example>
/// Render this component with an @ref="_fieldName".
/// In override OnAfterRender invoke _fieldName.IncrementCount();
/// This will re-render the child component with the exact amount of 'OnAfterRender'
/// invocations that occurred without an infinite loop / an eager counter that starts at 1.
/// </example>
public partial class RenderCounterDisplay : ComponentBase
{
    [Parameter, EditorRequired]
    public string DisplayName { get; set; }

    private int _renderCount;

    /// <summary>
    /// If guaranteed to be on the UI thread one might be interested in <see cref="IncrementCount"/> instead.
    /// </summary>
    public async Task IncrementCountAsync()
    {
        await InvokeAsync(IncrementCount);
    }
    
    /// <summary>
    /// If one is not guaranteed to be on the UI thread one might be interested in <see cref="IncrementCountAsync"/> instead.
    /// </summary>
    public void IncrementCount()
    {
        _renderCount++;

        StateHasChanged();
    }
}