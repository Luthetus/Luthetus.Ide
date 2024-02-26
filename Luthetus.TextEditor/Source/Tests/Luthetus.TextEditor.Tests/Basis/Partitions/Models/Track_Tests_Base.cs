namespace Luthetus.TextEditor.Tests.Basis.Partitions.Models;

public abstract class Track_Tests_Base
{
    #region Add
    public abstract void Add_Start();
    public abstract void Add_Middle();
    public abstract void Add_End();
    public abstract void Add_Causes_Expansion();
    public abstract void Add_Four_InARow();
    #endregion

    #region Insert
    public abstract void Insert_Start();
    public abstract void Insert_Middle();
    public abstract void Insert_End();
    public abstract void Insert_Causes_Expansion();
    public abstract void Insert_Four_InARow();
    #endregion
}
