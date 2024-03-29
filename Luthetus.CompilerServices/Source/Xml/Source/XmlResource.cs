﻿using Luthetus.TextEditor.RazorLib.CompilerServices;
using Luthetus.TextEditor.RazorLib.Lexes.Models;

namespace Luthetus.CompilerServices.Lang.Xml;

public sealed class XmlResource : LuthCompilerServiceResource
{
    public XmlResource(ResourceUri resourceUri, XmlCompilerService xmlCompilerService)
        : base(resourceUri, xmlCompilerService)
    {
    }
}