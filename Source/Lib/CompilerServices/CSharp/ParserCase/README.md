Goal: Add 'Find all references' { 'Shift' + 'F12' }
===================================================

It seems reasonable that the implementation details for a 'type member'
and a 'local variable' would not be 1 for 1.

Because, local variables would only be referenceable from the same scope or a child scope.
And, since the scope (or child scopes) exist only within the same file,
finding the references to these locals would not require as much
pre-calculation (partial classes are not relevant here because it would be a type-member-variable in that case).

A type-member-variable can be referenced in other files than just the the same file that
the variable is declared in.

So, more pre-calculation could be useful in the 'type-member-variable' case.


Type-member-variable:
---------------------
Track references by having the binder contain a Dictionary<FullyQualifiedName, List<ResourceUri>>
such that the 'List<ResourceUri>' is a list that contains all the files which contain at least 1 or more
references to the 'FullyQualifiedName'.

In order to know the count of references to the 'FullyQualifiedName',
visit each 'ResourceUri' and at this point decide how to go from here.

Maybe one could check the 'CSharpBinderSession' for each resource uri and
it has a Dictionary<FullyQualifiedName, List<TextEditorTextSpan>>

Where each 'List<TextEditorTextSpan>' is a list containing all the text spans
that reference the 'FullyQualifiedName' within that file.

public Dictionary<int, (ResourceUri ResourceUri, int StartInclusiveIndex)> SymbolIdToExternalTextSpanMap { get; } = new();

Binder.cs
{
	Dictionary<FullyQualifiedName, List<ResourceUri>> _map;
	
	StartBinderSession(ResourceUri resourceUri)
	{
		var previousBinderSession = GetPreviousBinderSession(resourceUri);
		
		foreach (var fullyQualifiedName in previousBinderSession.ExternalReferenceList)
		{
			if (_map.TryGetValue(fullyQualifiedName, out var resourceUriList))
			{
				for (int i = resourceUriList.Count - 1; i >= 0; i--)
				{
					if (resourceUriList[i] == resourceUri)
						resourceUriList.RemoveAt(i);
				}
			}
		}
	}
	
	Aaa(string fullyQualifiedName)
	{
		if (IsExternallyDefined(fullyQualifiedName))
		{
			binderSession.ExternalReferenceList.Add(fullyQualifiedName);
			_map[fullyQualifiedName].Add(binderSession.ResourceUri);
		}
	}
}


Local variable:
---------------
Walk the scope(s)?


