using System.Collections.Immutable;

namespace Luthetus.TextEditor.RazorLib.TextEditors.Models.TextEditorModels;

/// <param name="CharList">
/// Idea("Use an Array"|QuestionableIdea): Frequently, <see cref="CharList"/> needs to be converted to a <see cref="string"/>.
///       The code looks like the following example:
///           'new string(modifier.PartitionList.First().CharList.ToArray())'
///       |
///       If the <see cref="CharList"/> were an array of characters, would this be
///       a meaningful optimization (if even an optimization at all).
///       |
///       QuestionableIdea: an array might be an improvement, given the case where one does
///                         convert the entire <see cref="CharList"/> to a string.
///                         |
///                         But, how common is this in reality, especially when caching the result
///                         is still a separate and better option?
///                         |
///                         The questionable nature of this idea comes from text modification.
///                         The ImmutableArray datatype contains methods for insertion, deletion, etc...
///                         Where as with an array these methods are not available (speaking from memory; double check the array API).
///                         |
///                         Regardless of the collection type used, none of them would suffice.
///                         There is a <see cref="CharList"/> and a <see cref="DecorationByteList"/>.
///                         These two collections need to be in sync, i.e. have the same length, because their
///                         indices map to one another.
///                         |
///                         If one relies on the collection type of <see cref="CharList"/>, then that is also to say,
///                         that it un-related to the <see cref="DecorationByteList"/>.
///                         All in all, this means one would be inserting TWICE, and this is bound to
///                         be a nightmare.
///                         |
///                         The <see cref="TextEditorPartition"/> type itself needs to be a collection.
///                         Then, internally arrays could be used as the collection types for <see cref="CharList"/>
///                         and <see cref="DecorationByteList"/>?
///                         |
///                         This couples any collection method, such that one does not insert to <see cref="CharList"/>,
///                         nor <see cref="DecorationByteList"/>, but instead to the <see cref="TextEditorPartition"/>
///                         itself.
///                         |
///                         The double insertion would still need be performed, but it would be isolated logic
///                         internal to the <see cref="TextEditorPartition"/>.
/// <br/><br/>
/// Idea("Use a string"|BadIdea): Frequently, <see cref="CharList"/> needs to be converted to a <see cref="string"/>.
///       The code looks like the following example:
///           'new string(modifier.PartitionList.First().CharList.ToArray())'
///       |
///       If the <see cref="CharList"/> were a string, would this be
///       a meaningful optimization (if even an optimization at all).
///       |
///       This idea comes from the fact that, the collection type for the <see cref="CharList"/>
///       is already immutable.
///       |
///       So a readonly string perhaps is equivalent?
///       |
///       What would insertion operations and etc... look like, when performed directly
///       on a string?
///       |
///       Perhaps, better than making <see cref="CharList"/> a string, is to
///       cache the 'CharList' as a string, via a method that one can invoke
///       to return a string from a <see cref="TextEditorModel"/>.
/// Idea("Span&lt;char&gt;"): Frequently, <see cref="CharList"/> needs to be converted to a <see cref="string"/>.
///       The code looks like the following example:
///           'new string(modifier.PartitionList.First().CharList.ToArray())'
///       |
///       The other ideas regarding this won't work;
///       one does not necessarily convert all of the <see cref="CharList"/> to an array,
///       but instead some "substring" of sorts is being made by joining the
///       characters that span some indices.
/// Idea("Do not implement IEnumerable"): Given that methods with names such as, <see cref="Insert"/>
///                                       and <see cref="InsertRange"/> are being added,
///                                       it feels sensible to implement the entirety of <see cref="IEnumerable{T}"/>.
///                                       |
///                                       However, if one finds all references to <see cref="CharList"/>, and
///                                       <see cref="DecorationByteList"/>, they can determine the minimal
///                                       amount of methods that need be implemented.
///                                       |
///                                       So, start with the minimal amount of methods that need implemented,
///                                       then revisit the idea of implementing <see cref="IEnumerable{T}"/>
/// </param>
public class TextEditorPartition
{
    private TextEditorPartition(ImmutableList<char> charList, List<byte> decorationByteList)
    {
        CharList = charList;
        DecorationByteList = decorationByteList;
    }

    /// <summary>
    /// This acts as a quasi-constructor.
    /// In order to ensure all <see cref="TextEditorPartition"/> changes are performed internally,
    /// the constructor is private.<br/><br/>
    /// 
    /// Otherwise one could have external code 'myPartition = myPartition with { CharList = someCharList };'
    /// This would then break the <see cref="TextEditorPartition"/> because now the <see cref="CharList"/>
    /// is out of sync with the <see cref="DecorationByteList"/>.
    /// </summary>
    public static readonly TextEditorPartition Empty = new(ImmutableList<char>.Empty, new());

    public ImmutableList<char> CharList { get; }
    public List<byte> DecorationByteList { get; }

    /// <summary>
    /// This is the count of characters in THIS particular partition, the <see cref="TextEditorModel.DocumentLength"/>
    /// contains the whole count of characters across all partitions.
    /// </summary>
    public int Count { get; }

    public void Insert()
    {
        throw new NotImplementedException();
    }

    public void InsertRange()
    {
        throw new NotImplementedException();
    }

    public void RemoveAt()
    {
        throw new NotImplementedException();
    }

    public void AddRange()
    {
        throw new NotImplementedException();
    }
}
