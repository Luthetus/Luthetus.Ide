using Luthetus.Common.RazorLib.Contexts.Models;
using Luthetus.Common.RazorLib.Keys.Models;
using Luthetus.Common.RazorLib.Panels.Models;

namespace Luthetus.Common.RazorLib.Panels.States;

public class PanelsStateActionsTests
{
    [Fact]
    public void RegisterPanelGroupAction()
    {
        /*
        public record RegisterPanelGroupAction(PanelGroup PanelGroup);
         */

        throw new NotImplementedException();
    }

    [Fact]
    public void DisposePanelGroupAction()
    {
        /*
        public record DisposePanelGroupAction(Key<PanelGroup> PanelGroupKey);
         */

        throw new NotImplementedException();
    }

    [Fact]
    public void RegisterPanelTabAction()
    {
        /*
        public record RegisterPanelTabAction(Key<PanelGroup> PanelGroupKey, PanelTab PanelTab, bool InsertAtIndexZero);
         */

        throw new NotImplementedException();
    }

    [Fact]
    public void DisposePanelTabAction()
    {
        /*
        public record DisposePanelTabAction(Key<PanelGroup> PanelGroupKey, Key<PanelTab> PanelTabKey);
         */

        throw new NotImplementedException();
    }

    [Fact]
    public void SetActivePanelTabAction()
    {
        /*
        public record SetActivePanelTabAction(Key<PanelGroup> PanelGroupKey, Key<PanelTab> PanelTabKey);
         */

        throw new NotImplementedException();
    }

    [Fact]
    public void SetPanelTabAsActiveByContextRecordKeyAction()
    {
        /*
        public record SetPanelTabAsActiveByContextRecordKeyAction(Key<ContextRecord> ContextRecordKey);
         */

        throw new NotImplementedException();
    }

    [Fact]
    public void SetDragEventArgsAction()
    {
        /*
        public record SetDragEventArgsAction((PanelTab PanelTab, PanelGroup PanelGroup)? DragEventArgs);
         */

        throw new NotImplementedException();
    }
}