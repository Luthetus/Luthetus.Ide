using Fluxor;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorModels;
using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorServices;
using Luthetus.TextEditor.RazorLib.TextEditors.States;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Models.Internals;

/// <summary>
/// One must track whether the ViewModel is currently being rendered.
/// <br/><br/>
/// The reason for this is that the UI logic is lazily invoked.
/// That is to say, if a ViewModel has its underlying Model change, BUT the ViewModel is not currently being rendered. Then that ViewModel does not
/// react to the Model having changed.
/// </summary>
public class DisplayTracker : IDisposable
{
    private readonly object _linksLock = new();
    private readonly ITextEditorService _textEditorService;
    
    private IState<TextEditorModelState>? _modelStateWrap;
    private CancellationTokenSource _calculateVirtualizationResultCancellationTokenSource = new();

    public DisplayTracker(
        ITextEditorService textEditorService,
        Func<TextEditorViewModel?> getViewModelFunc,
        Func<TextEditorModel?> getModelFunc)
    {
        _textEditorService = textEditorService;
        GetViewModelFunc = getViewModelFunc;
        GetModelFunc = getModelFunc;
    }

    /// <summary>
    /// The instance for the ViewModel is constantly being re-created with the record 'with' keyword.
    /// So the only way to reliably get the current reference to the ViewModel is by invoking an Action to get the current record instance.
    /// </summary>
    public Func<TextEditorViewModel?> GetViewModelFunc { get; }
    public Func<TextEditorModel?> GetModelFunc { get; }
    /// <summary>
    /// <see cref="Links"/> refers to a Blazor TextEditorViewModelDisplay having had its OnParametersSet invoked
    /// and the ViewModelKey that was passed as a parameter matches this encompasing ViewModel's key. In this situation
    /// <see cref="Links"/> would be incremented by 1 in a concurrency safe manner.
    /// <br/><br/>
    /// As well OnParametersSet includes the case where the ViewModelKey that was passed as a parameter is changed.
    /// In this situation the previous ViewModel would have its <see cref="Links"/> decremented by 1 in a concurrency safe manner.
    /// <br/><br/>
    /// TextEditorViewModelDisplay implements IDisposable. In the Dispose implementation,
    /// the active ViewModel would have its <see cref="Links"/> decremented by 1 in a concurrency safe manner.
    /// </summary>
    public int Links { get; private set; }
    /// <summary>
    /// Since the UI logic is lazily calculated only for ViewModels which are currently rendered to the UI,
    /// when a ViewModel becomes rendered it needs to have its calculations performed so it is up to date.
    /// </summary>
    public bool IsFirstDisplay { get; private set; } = true;

    public void IncrementLinks(IState<TextEditorModelState> modelStateWrap)
    {
        lock (_linksLock)
        {
            Links++;

            if (Links == 1)
            {
                // This ViewModel was not being displayed until this point.
                // Due to lazily updating the UI, now that it IS being displayed,
                // proceed to subscribe to the events.

                IsFirstDisplay = true;

                _modelStateWrap = modelStateWrap;
                _modelStateWrap.StateChanged += ModelsStateWrap_StateChanged;
            }
        }
    }

    public void DecrementLinks(IState<TextEditorModelState> modelStateWrap)
    {
        lock (_linksLock)
        {
            Links--;

            if (Links == 0)
            {
                // This ViewModel will NO LONGER be rendered.
                // Due to lazily updating the UI, proceed to unsubscribe from the events.

                _modelStateWrap = modelStateWrap;
                _modelStateWrap.StateChanged -= ModelsStateWrap_StateChanged;
            }
        }
    }

    public bool ConsumeIsFirstDisplay()
    {
        lock (_linksLock)
        {
            var localIsFirstDisplay = IsFirstDisplay;
            IsFirstDisplay = false;

            return localIsFirstDisplay;
        }
    }

    private void ModelsStateWrap_StateChanged(object? sender, EventArgs e)
    {
        //var model = GetModelFunc.Invoke();
        //var viewModel = GetViewModelFunc.Invoke();

        //if (model is null || viewModel is null)
        //    return;

        //_calculateVirtualizationResultCancellationTokenSource.Cancel();
        //_calculateVirtualizationResultCancellationTokenSource = new();

        //_textEditorService.Post(nameof(ModelsStateWrap_StateChanged),
        //    _textEditorService.ViewModelApi.CalculateVirtualizationResultFactory(
        //        model.ResourceUri,
        //        viewModel.ViewModelKey,
        //        _calculateVirtualizationResultCancellationTokenSource.Token));
    }

    public void Dispose()
    {
        if (_modelStateWrap is not null)
            _modelStateWrap.StateChanged -= ModelsStateWrap_StateChanged;
    }
}