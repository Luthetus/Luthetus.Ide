using Luthetus.Common.RazorLib.Keys.Models;
using Microsoft.AspNetCore.Components;
using System.Reflection;
using Luthetus.Common.RazorLib.Reflectives.Models;

namespace Luthetus.Common.RazorLib.Reflectives.Displays;

public partial class ReflectivePanel : ComponentBase, IDisposable
{
    [Inject]
    private IReflectiveService ReflectiveService { get; set; } = null!;

    [Parameter, EditorRequired]
    public List<Type> ComponentTypeList { get; set; } = null!;
    
    protected override void OnInitialized()
    {
    	ReflectiveService.ReflectiveStateChanged += OnReflectiveStateChanged;
    	base.OnInitialized();
    }

    protected override Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            var model = new ReflectiveModel(
                Key<ReflectiveModel>.NewKey(),
                ComponentTypeList,
                ComponentTypeList.FirstOrDefault(x => x.Name == "SolutionExplorerDisplay")?.GUID ?? Guid.Empty,
                Guid.Empty,
                Array.Empty<PropertyInfo>(),
                new(),
                ReflectiveService);

            ReflectiveService.ReduceRegisterAction(model, 0);
        }

        return base.OnAfterRenderAsync(firstRender);
    }

    private void DispatchRegisterActionOnClick()
    {
        var model = new ReflectiveModel(
            Key<ReflectiveModel>.NewKey(),
            ComponentTypeList,
            Guid.Empty,
            Guid.Empty,
            Array.Empty<PropertyInfo>(),
            new(),
            ReflectiveService);

        ReflectiveService.ReduceRegisterAction(model, 0);
    }
    
    public async void OnReflectiveStateChanged()
    {
    	await InvokeAsync(StateHasChanged);
    }
    
    public void Dispose()
    {
    	ReflectiveService.ReflectiveStateChanged -= OnReflectiveStateChanged;
    }
}