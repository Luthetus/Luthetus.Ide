namespace Luthetus.TextEditor.Tests.Basis.Partitions.Models;

public abstract class Track_Tests_Base
{
    #region Add
    public abstract void Add_At_Start();
    public abstract void Add_At_Middle();
    public abstract void Add_At_End();
    public abstract void Add_Causes_Expansion();
    public abstract void Add_Four_InARow();
    #endregion

    #region Insert
    public abstract void Insert_At_Start();
    public abstract void Insert_At_Middle();
    public abstract void Insert_At_End();
    public abstract void Insert_Causes_Expansion();
    public abstract void Insert_Four_InARow();
    #endregion

    #region Remove
    public abstract void Remove_At_Start();
    public abstract void Remove_At_Middle();
    public abstract void Remove_At_End();
    public abstract void Remove_Causes_Empty_Partition();
    public abstract void Remove_Four_InARow();
    #endregion
}
