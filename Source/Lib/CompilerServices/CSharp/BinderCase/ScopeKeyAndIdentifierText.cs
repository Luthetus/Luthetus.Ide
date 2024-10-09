using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.TextEditor.RazorLib.CompilerServices.Interfaces;

namespace Luthetus.CompilerServices.CSharp.BinderCase;

public record struct ScopeKeyAndIdentifierText(
	Key<IScope> ScopeKey,
	string IdentifierText);
