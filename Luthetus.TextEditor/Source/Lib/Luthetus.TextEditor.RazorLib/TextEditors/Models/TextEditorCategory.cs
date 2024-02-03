using Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorModels;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Models;

/// <summary>
/// An optional distinction between many <see cref="TextEditorViewModel"/>(s) which
/// display the same <see cref="TextEditorModel"/>.
/// <br/>
/// Example: one might have a 'main' text editor. Perhaps this category is named "main".
///          Furthermore one might have a 'peek' window. Perhaps this category is named "peek".
///          This results in one being able to have two separate view models, which display the
///          same underlying model, and distinguish between them.
/// </summary>
/// <param name="Value">
/// The category string value itself.
/// </param>
public record TextEditorCategory(string Value);
