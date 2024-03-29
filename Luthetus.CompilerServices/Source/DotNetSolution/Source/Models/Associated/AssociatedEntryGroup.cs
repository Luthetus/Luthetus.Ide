﻿using Luthetus.TextEditor.RazorLib.CompilerServices.Syntax.Tokens;
using System.Collections.Immutable;

namespace Luthetus.CompilerServices.Lang.DotNetSolution.Models.Associated;

public record AssociatedEntryGroup : IAssociatedEntry
{
    public AssociatedEntryGroup(
        OpenAssociatedGroupToken openAssociatedGroupToken,
        ImmutableArray<IAssociatedEntry> associatedEntryList,
        CloseAssociatedGroupToken? closeAssociatedGroupToken)
    {
        OpenAssociatedGroupToken = openAssociatedGroupToken;
        AssociatedEntryList = associatedEntryList;
        CloseAssociatedGroupToken = closeAssociatedGroupToken;
    }

    public OpenAssociatedGroupToken OpenAssociatedGroupToken { get; }
    public ImmutableArray<IAssociatedEntry> AssociatedEntryList { get; init; }
    public CloseAssociatedGroupToken? CloseAssociatedGroupToken { get; }

    public AssociatedEntryKind AssociatedEntryKind => AssociatedEntryKind.Group;
}
