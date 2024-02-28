namespace Luthetus.TextEditor.Tests.Basis.Partitions.Models;

public abstract class Track_Tests_Base
{
    #region Add
    public abstract void Add();
    public abstract void Add_Causes_Expansion();
    public abstract void Add_Four_InARow();
    #endregion

    #region Insert
    public abstract void Insert();
    public abstract void Insert_Causes_Expansion();
    public abstract void Insert_Four_InARow();
    #endregion

    #region Remove
    public abstract void Remove();
    public abstract void Remove_Causes_Empty_Partition();
    public abstract void Remove_Four_InARow();
    #endregion
}
