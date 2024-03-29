﻿using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.TextEditor.RazorLib.Lexes.Models;

namespace Luthetus.CompilerServices.Lang.FSharp;

public class FSharpResource : LuthCompilerServiceResource
{
    public FSharpResource(ResourceUri resourceUri, FSharpCompilerService fSharpCompilerService)
        : base(resourceUri, fSharpCompilerService)
    {
    }
}