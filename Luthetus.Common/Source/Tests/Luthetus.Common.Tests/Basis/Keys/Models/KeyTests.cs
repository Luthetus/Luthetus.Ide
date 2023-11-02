namespace Luthetus.Common.RazorLib.Keys.Models;

public record struct KeyTests<T>(Guid Guid)
{
    [Fact]
    public void Empty()
    {
        /*
        public static readonly Key<T> Empty = new Key<T>(Guid.Empty);
         */

        throw new NotImplementedException();
    }

    [Fact]
    public void NewKey()
    {
        /*
        public static Key<T> NewKey()
         */

        throw new NotImplementedException();
    }
}