namespace Luthetus.CompilerServices.Lang.FSharp.Tests.TestDataFolder;

public static partial class TestData
{
    public static class FSharp
    {
        public const string EXAMPLE_TEXT_21_LINES = @"let fib3 n = 
    match n with 
    | 0 -> 0
    | n -> 
        let mutable last = 0
        let mutable next = 1
        for i in 1 .. (n - 1) do
            let temp = last + next
            last <- next
            next <- temp
        next

//this is a single line comment

(* 
        This is a multi line
        comment
        in F#
*)

// let commentContainingKeywords

let run =
    fib3 2 |> printfn ""%i"" |> ignore
    fib3 3 |> printfn ""%i"" |> ignore
    fib3 40 |> printfn ""%i"" |> ignore";
    }
}