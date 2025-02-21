namespace Luthetus.Common.RazorLib.ListExtensions;

/// <summary>
/// https://github.com/dotnet/runtime/issues/31156
/// LINQ FindIndex extension method
/// </summary>
public static class IReadOnlyListExtensions
{
	public static int FindIndex<T>(this IReadOnlyList<T> source, Func<T, bool> predicate)
	{
	   int i = 0;
	   foreach (var item in source)
	   {
	      if (predicate.Invoke(item))
	         return i;
	      i++;
	   }
	   return -1;
	}
}

