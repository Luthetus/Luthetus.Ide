// This file is used by Code Analysis to maintain SuppressMessage
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given
// a specific target and scoped to a namespace, type, member, etc.

using System.Diagnostics.CodeAnalysis;

[assembly: SuppressMessage(
	"Assertions",
	"xUnit2013:Do not use equality check to check for collection size.",
	Justification = "When asserting collection sizes that are not empty, one types 'Assert.Equal(3, collection.Count).' So, to use a different method in the case of collection.Count == 0, is upsetting.",
	Scope = "module")]
