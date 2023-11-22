using Luthetus.Common.RazorLib.Dialogs.Models;
using Luthetus.Common.RazorLib.Resizes.Models;
using Luthetus.Common.RazorLib.Dimensions.Models;
using Microsoft.AspNetCore.Components.Web;

namespace Luthetus.Common.Tests.Basis.Resizes.Models;

/// <summary>
/// <see cref="ResizeHelper"/>
/// </summary>
public class ResizeHelperTests
{
    /// <summary>
    /// <see cref="ResizeHelper.ResizeNorth(ElementDimensions, MouseEventArgs)"/>
    /// </summary>
    [Fact]
    public void ResizeNorth()
    {
        var elementDimensions = DialogRecord.ConstructDefaultDialogDimensions();

        var mouseEventArgs = new MouseEventArgs { /* ... */ };

        // deltaX
        {
            // positive
            {
                // No effect
                throw new NotImplementedException();
            }

            // negative
            {
                // No effect
                throw new NotImplementedException();
            }
        }

        // deltaY
        {
            // positive
            {
                // Height decreases
                // Top increases
                throw new NotImplementedException();
            }

            // negative
            {
                // Height increases
                // Top decreases
                throw new NotImplementedException();
            }
        }
    }

    /// <summary>
    /// <see cref="ResizeHelper.ResizeEast(ElementDimensions, MouseEventArgs)"/>
    /// </summary>
    [Fact]
    public void ResizeEast()
    {
        var elementDimensions = DialogRecord.ConstructDefaultDialogDimensions();

        var mouseEventArgs = new MouseEventArgs { /* ... */ };

        // deltaX
        {
            // positive
            {
                // Width increases
                throw new NotImplementedException();
            }

            // negative
            {
                // Width decreases
                throw new NotImplementedException();
            }
        }

        // deltaY
        {
            // positive
            {
                // No effect
                throw new NotImplementedException();
            }

            // negative
            {
                // No effect
                throw new NotImplementedException();
            }
        }
    }

    /// <summary>
    /// <see cref="ResizeHelper.ResizeSouth(ElementDimensions, MouseEventArgs)"/>
    /// </summary>
    [Fact]
    public void ResizeSouth()
    {
        var elementDimensions = DialogRecord.ConstructDefaultDialogDimensions();

        var mouseEventArgs = new MouseEventArgs { /* ... */ };

        // deltaX
        {
            // positive
            {
                // No effect
                throw new NotImplementedException();
            }

            // negative
            {
                // No effect
                throw new NotImplementedException();
            }
        }

        // deltaY
        {
            // positive
            {
                // Height increases
                throw new NotImplementedException();
            }

            // negative
            {
                // Height decreases
                throw new NotImplementedException();
            }
        }
    }

    /// <summary>
    /// <see cref="ResizeHelper.ResizeWest(ElementDimensions, MouseEventArgs)"/>
    /// </summary>
    [Fact]
    public void ResizeWest()
    {
        var elementDimensions = DialogRecord.ConstructDefaultDialogDimensions();

        var mouseEventArgs = new MouseEventArgs { /* ... */ };

        // deltaX
        {
            // positive
            {
                // Width decreases
                // Left increases
                throw new NotImplementedException();
            }

            // negative
            {
                // Width increases
                // Left decreases
                throw new NotImplementedException();
            }
        }

        // deltaY
        {
            // positive
            {
                // No effect
                throw new NotImplementedException();
            }

            // negative
            {
                // No effect
                throw new NotImplementedException();
            }
        }
    }

    /// <summary>
    /// <see cref="ResizeHelper.ResizeNorthEast(ElementDimensions, MouseEventArgs)"/>
    /// </summary>
    [Fact]
    public void ResizeNorthEast()
    {
        var elementDimensions = DialogRecord.ConstructDefaultDialogDimensions();

        var mouseEventArgs = new MouseEventArgs { /* ... */ };

        // deltaX
        {
            // positive
            {
                // Width increases
                throw new NotImplementedException();
            }

            // negative
            {
                // Width decreases
                throw new NotImplementedException();
            }
        }

        // deltaY
        {
            // positive
            {
                // Height decreases
                // Top increases
                throw new NotImplementedException();
            }

            // negative
            {
                // Height increases
                // Top decreases
                throw new NotImplementedException();
            }
        }
    }

    /// <summary>
    /// <see cref="ResizeHelper.ResizeSouthEast(ElementDimensions, MouseEventArgs)"/>
    /// </summary>
    [Fact]
    public void ResizeSouthEast()
    {
        var elementDimensions = DialogRecord.ConstructDefaultDialogDimensions();

        var mouseEventArgs = new MouseEventArgs { /* ... */ };

        // deltaX
        {
            // positive
            {
                // Width increases
                throw new NotImplementedException();
            }

            // negative
            {
                // Width decreases
                throw new NotImplementedException();
            }
        }

        // deltaY
        {
            // positive
            {
                // Height increases
                throw new NotImplementedException();
            }

            // negative
            {
                // Height decreases
                throw new NotImplementedException();
            }
        }
    }

    /// <summary>
    /// <see cref="ResizeHelper.ResizeSouthWest(ElementDimensions, MouseEventArgs)"/>
    /// </summary>
    [Fact]
    public void ResizeSouthWest()
    {
        var elementDimensions = DialogRecord.ConstructDefaultDialogDimensions();

        var mouseEventArgs = new MouseEventArgs { /* ... */ };

        // deltaX
        {
            // positive
            {
                // Width decreases
                // Left increases
                throw new NotImplementedException();
            }

            // negative
            {
                // Width increases
                // Left decreases
                throw new NotImplementedException();
            }
        }

        // deltaY
        {
            // positive
            {
                // Height increases
                throw new NotImplementedException();
            }

            // negative
            {
                // Height decreases
                throw new NotImplementedException();
            }
        }
    }

    /// <summary>
    /// <see cref="ResizeHelper.ResizeNorthWest(ElementDimensions, MouseEventArgs)"/>
    /// </summary>
    [Fact]
    public void ResizeNorthWest()
    {
        var elementDimensions = DialogRecord.ConstructDefaultDialogDimensions();

        var mouseEventArgs = new MouseEventArgs { /* ... */ };

        // deltaX
        {
            // positive
            {
                // Width decreases
                // Left increases
                throw new NotImplementedException();
            }

            // negative
            {
                // Width increases
                // Left decreases
                throw new NotImplementedException();
            }
        }

        // deltaY
        {
            // positive
            {
                // Height decreases
                // Top increases
                throw new NotImplementedException();
            }

            // negative
            {
                // Height increases
                // Top decreases
                throw new NotImplementedException();
            }
        }
    }

    /// <summary>
    /// <see cref="ResizeHelper.Move(ElementDimensions, MouseEventArgs)"/>
    /// </summary>
    [Fact]
    public void Move()
    {
        var elementDimensions = DialogRecord.ConstructDefaultDialogDimensions();

        var mouseEventArgs = new MouseEventArgs { /* ... */ };

        // deltaX
        {
            // positive
            {
                // Left increases
                throw new NotImplementedException();
            }

            // negative
            {
                // Left decreases
                throw new NotImplementedException();
            }
        }

        // deltaY
        {
            // positive
            {
                // Top increases
                throw new NotImplementedException();
            }

            // negative
            {
                // Top decreases
                throw new NotImplementedException();
            }
        }
    }
}