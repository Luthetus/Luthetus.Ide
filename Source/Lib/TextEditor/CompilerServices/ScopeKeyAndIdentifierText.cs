using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;

namespace Luthetus.TextEditor.RazorLib.CompilerServices;

public record struct ScopeKeyAndIdentifierText(
	Key<IScope> ScopeKey,
	string IdentifierText);
