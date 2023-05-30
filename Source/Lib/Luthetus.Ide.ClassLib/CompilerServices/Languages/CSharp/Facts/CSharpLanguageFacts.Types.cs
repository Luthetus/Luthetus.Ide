namespace Luthetus.Ide.ClassLib.CompilerServices.Languages.CSharp.Facts;

public partial class CSharpLanguageFacts
{
    public class Types
    {
        public static readonly (string name, Type type) Void = ("void", typeof(void));
        public static readonly (string name, Type type) Int = ("int", typeof(int));
        public static readonly (string name, Type type) String = ("string", typeof(string));
        public static readonly (string name, Type type) Bool = ("bool", typeof(bool));
        public static readonly (string name, Type type) Var = ("var", typeof(void));
    }
}