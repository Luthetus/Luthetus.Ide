namespace Luthetus.TextEditor.RazorLib.TextEditors.Models;

public interface ITextEditorHeaderRegistry
{
	/// <summary>
    /// If an 'extensionNoPeriod' key does not exist in the map,
    /// the default is TextEditorDefaultHeaderDisplay.
	/// </summary>
    public Type? GetHeader(string extensionNoPeriod);
    
    /// <summary>
    /// If the 'extensionNoPeriod' key value pair does not exist in the map then add it.
    /// Otherwise, replace the existing value with the 'type'.
    /// </summary>
    public void UpsertHeader(string extensionNoPeriod, Type? type);
    
    /// <summary>
    /// Remove the 'extensionNoPeriod' key value pair from the Dictionary (if it exists).
    /// </summary>
    public void RemoveHeader(string extensionNoPeriod);
}
