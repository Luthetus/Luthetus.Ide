namespace Luthetus.CompilerServices.Lang.DotNetSolution.Obsolete.RewriteForImmutability;

/// <summary>
/// The word "ViewModel" drives me crazy when reading code considering
/// the existence of a "Model".
/// <br/><br/>
/// I'm going to call this "View" for now. I'm still annoyed because of MVC views
/// but I suppose "View" will suffice for now, and a refactor-rename can be done
/// when I finalize what I want to call this instead of "ViewModel".
/// <br/><br/>
/// Furthermore, I can't stand having an IEnumerable&lt;TItem&gt; be a variable named
/// tItems. Then the foreach loop creates a iteration variable named 'tItem'.
/// These varible names only differ by an 's' character as to pluralize them.
/// I usually just deal with using these namings because I don't want to type tItemList
/// when its actually an array. Also, in C# the compiler will type check and give me an error
/// if I make a typo and use 'tItems' where I meant to use 'tItem'. But if I have this habit
/// I'm going to be really annoyed when I write JavaScript or some other language that doesn't
/// type check me.
/// <br/><br/>
/// I like the idea of finding a short word in English which means "container" but isn't a
/// common datatype. The word 'box' seems perfect for this. I have a box of TItem so I can
/// name the variable 'tItemBox'. Then the iteration variable in my foreach loop can be
/// tItem. But I can't sleep at night when I think about this naming because the word 'box'
/// is synonymous with reference types.
/// <br/><br/>
/// So maybe I'll call the IEnumerable variable 'tItemBag'. But once again I'm being
/// confusing because in MVC you can access the ViewBag. Also, 'bag' almost sounds
/// unorganized, or untyped. Just throw whatever you want in it.
/// <br/><br/>
/// Biscuits and gravy.
/// </summary>
public class DotNetSolutionView
{
}
