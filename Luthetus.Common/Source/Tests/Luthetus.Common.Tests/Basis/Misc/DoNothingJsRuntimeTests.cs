using Microsoft.JSInterop;

namespace Luthetus.Common.RazorLib.Misc;

public class DoNothingJsRuntimeTests
{
    [Fact]
    public void InvokeAsyncA()
    {
        /*
        public ValueTask<TValue> InvokeAsync<TValue>(string identifier, object?[]? args)
        {
            return default;
        }
         */

        throw new NotImplementedException();
    }

    [Fact]
    public void InvokeAsyncB()
    {
        /*
        public ValueTask<TValue> InvokeAsync<TValue>(string identifier, CancellationToken cancellationToken, object?[]? args)
        {
            return default;
        }
         */

        throw new NotImplementedException();
    }
}