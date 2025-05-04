using Luthetus.Extensions.CompilerServices.Syntax.Nodes.Interfaces;
using Luthetus.TextEditor.RazorLib.CompilerServices;

namespace Luthetus.Extensions.CompilerServices;

public struct Scope
{
	/// <summary>Use '-1' to represent a null int.</summary>
	public Scope(
		ICodeBlockOwner codeBlockOwner,
		int indexKey,
		int parentIndexKey,
		int startInclusiveIndex,
		int endExclusiveIndex)
	{
		CodeBlockOwner = codeBlockOwner;
		IndexKey = indexKey;
		ParentIndexKey = parentIndexKey;
		StartInclusiveIndex = startInclusiveIndex;
		EndExclusiveIndex = endExclusiveIndex;
	}

	public bool ConstructorWasInvoked => CodeBlockOwner is not null;
	/// <summary>
	/// This is believed to be far better than a <see cref="ISyntaxNode"/>
	/// having a pointer to its parent.
	///
	/// (i.e.: with this the Nodes can be left as a singly linked tree.)
	///
	/// Then, one can query the <see cref="IBinder"/>
	/// to determine what the active scope is given a positionIndex within a file, or etc...
	///
	/// This scope struct then acts as an index into the compilation unit's node tree.
	/// 
	/// If a scope contains 100 nodes, then 100 pointers would have previously been needed due to 1 per node.
	///
	/// By having the 'Scope' contain a pointer to a node, the same example only requires 1 pointer
	/// to the node that created the scope.
	///
	/// From there, traversal down the tree is done with ISyntaxNode.GetChildList();
	///
	/// Traversal up the tree is done by reading from the <see cref="IBinderSession"/>
	/// inside the <see cref="IBinderSession.ScopeList"/> at index <see cref="ParentIndexKey"/>.
	///
	/// This tells you the node that owns the parent scope.
	///
	/// If it turns out that this is too cumbersome to use in practice then
	/// perhaps a different solution should be considered.
	/// </summary>
	public ICodeBlockOwner CodeBlockOwner { get; }
	/// <summary>
	/// Use '-1' to represent a null int.
	/// 
	/// All scopes in a file are stored in a flat list, grouped by the ResourceUri they were found in.
	/// Everytime a scope is instantiated, it gets an incrementing counter as its IndexKey
	/// (which is equal to the index it was inserted at... 'list.Count').
	///
	/// Therefore, to go to the parent scope: 'var parentScope = list[ParentIndexKey]'
	/// To get "yourself": 'var selfScope = list[IndexKey]'
	///
	/// Every time the file is parsed the counter resets to 0.
	///
	/// In the future there should be optimizations that don't reparse the entire file,
	/// but instead only the scopes that need to be re-parsed given the change made.
	///
	/// This shouldn't be an issue beyond integer overflow.
	/// And to in the future solve the integer overflow,
	/// in the rare case that someone parses int.MaxValue number of scopes
	/// in a singular C# ResourceUri they likely don't
	/// actually have that many scopes in the file, but instead
	/// hit that number due to the future optimization that wasn't reseting the counter.
	/// Then that would trigger a counter reset to 0, and the entire file
	/// would have to be reparsed for that edge case.
	///
	/// If a file needs int.MaxValue number of scopes during a fresh parse with the counter starting at 0,
	/// then I presume this code will throw an exception.
	///
	/// But wouldn't it just the same do so with the previous code once the list of scopes
	/// hit a count > int.MaxValue?
	///
	/// If it turns out that int.MaxValue is an issue, the storage of scopes needs to be re-worked.
	/// But it isn't expected to be the case.
	///
	/// Side note: in order to avoid race conditions involving out dated data,
	/// should the list itself have an int associated with it, that the IScope.cs has as a property,
	/// so it can verify that the "batch id" is the same.
	/// </summary>
	public int IndexKey { get; }
	/// <inheritdoc cref="IndexKey"/>
	public int ParentIndexKey { get; }
	public int StartInclusiveIndex { get; }
	/// <summary>
	/// Beware of this property's setter,
	/// as a hack to set the ending index later on in this
	/// type's lifecycle because it isn't immediately known.
	/// </summary>
	public int EndExclusiveIndex { get; set; }
}
