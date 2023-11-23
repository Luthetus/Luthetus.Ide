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
    /// <see cref="ResizeHelper.ResizeNorth(ElementDimensions, MouseEventArgs, MouseEventArgs)"/>
    /// </summary>
    [Fact]
    public void ResizeNorth()
    {
        // deltaX
        {
            // positive
            {
                InitializeResizeHelperTests(out var controlElementDimensions);
                InitializeResizeHelperTests(out var testElementDimensions);

                var firstMouseEventArgs = new MouseEventArgs { ClientX = 1000, ClientY = 300, };
                var secondMouseEventArgs = new MouseEventArgs { ClientX = 1100, ClientY = 300, };

                ResizeHelper.ResizeNorth(testElementDimensions, firstMouseEventArgs, secondMouseEventArgs);

                var control = GetPixelMeasurementsTuple(controlElementDimensions);
                var test = GetPixelMeasurementsTuple(testElementDimensions);

                var deltaX = secondMouseEventArgs.ClientX - firstMouseEventArgs.ClientX;
                var deltaY = secondMouseEventArgs.ClientY - firstMouseEventArgs.ClientY;

                Assert.True(deltaX > 0);
                Assert.True(deltaY == 0);

                // No effect
                Assert.Equal(control, test);
            }

            // negative
            {
                InitializeResizeHelperTests(out var controlElementDimensions);
                InitializeResizeHelperTests(out var testElementDimensions);

                var firstMouseEventArgs = new MouseEventArgs { ClientX = 1100, ClientY = 300, };
                var secondMouseEventArgs = new MouseEventArgs { ClientX = 1000, ClientY = 300, };

                ResizeHelper.ResizeNorth(testElementDimensions, firstMouseEventArgs, secondMouseEventArgs);

                var control = GetPixelMeasurementsTuple(controlElementDimensions);
                var test = GetPixelMeasurementsTuple(testElementDimensions);

                var deltaX = secondMouseEventArgs.ClientX - firstMouseEventArgs.ClientX;
                var deltaY = secondMouseEventArgs.ClientY - firstMouseEventArgs.ClientY;

                Assert.True(deltaX < 0);
                Assert.True(deltaY == 0);

                // No effect
                Assert.Equal(control, test);
            }
        }

        // deltaY
        {
            // positive
            {
                InitializeResizeHelperTests(out var controlElementDimensions);
                InitializeResizeHelperTests(out var testElementDimensions);

                var firstMouseEventArgs = new MouseEventArgs { ClientX = 1000, ClientY = 290, };
                var secondMouseEventArgs = new MouseEventArgs { ClientX = 1000, ClientY = 300, };

                ResizeHelper.ResizeNorth(testElementDimensions, firstMouseEventArgs, secondMouseEventArgs);

                var control = GetPixelMeasurementsTuple(controlElementDimensions);
                var test = GetPixelMeasurementsTuple(testElementDimensions);

                var deltaX = secondMouseEventArgs.ClientX - firstMouseEventArgs.ClientX;
                var deltaY = secondMouseEventArgs.ClientY - firstMouseEventArgs.ClientY;

                Assert.True(deltaX == 0);
                Assert.True(deltaY > 0);

                Assert.Equal(control.width, test.width);
                // Height decreases
                Assert.Equal(control.height - deltaY, test.height);
                Assert.Equal(control.left, test.left);
                // Top increases
                Assert.Equal(control.top + deltaY, test.top);
            }

            // negative
            {
                InitializeResizeHelperTests(out var controlElementDimensions);
                InitializeResizeHelperTests(out var testElementDimensions);

                var firstMouseEventArgs = new MouseEventArgs { ClientX = 1000, ClientY = 300, };
                var secondMouseEventArgs = new MouseEventArgs { ClientX = 1000, ClientY = 290, };

                ResizeHelper.ResizeNorth(testElementDimensions, firstMouseEventArgs, secondMouseEventArgs);

                var control = GetPixelMeasurementsTuple(controlElementDimensions);
                var test = GetPixelMeasurementsTuple(testElementDimensions);

                var deltaX = secondMouseEventArgs.ClientX - firstMouseEventArgs.ClientX;
                var deltaY = secondMouseEventArgs.ClientY - firstMouseEventArgs.ClientY;

                Assert.True(deltaX == 0);
                Assert.True(deltaY < 0);

                Assert.Equal(control.width, test.width);
                // Height increases
                Assert.Equal(control.height - deltaY, test.height);
                Assert.Equal(control.left, test.left);
                // Top decreases
                Assert.Equal(control.top + deltaY, test.top);
            }
        }
    }

    /// <summary>
    /// <see cref="ResizeHelper.ResizeEast(ElementDimensions, MouseEventArgs, MouseEventArgs)"/>
    /// </summary>
    [Fact]
    public void ResizeEast()
    {
        // deltaX
        {
            // positive
            {
                // Width increases
                InitializeResizeHelperTests(out var controlElementDimensions);
                InitializeResizeHelperTests(out var testElementDimensions);

                var firstMouseEventArgs = new MouseEventArgs { ClientX = 1000, ClientY = 300, };
                var secondMouseEventArgs = new MouseEventArgs { ClientX = 1100, ClientY = 300, };

                ResizeHelper.ResizeEast(testElementDimensions, firstMouseEventArgs, secondMouseEventArgs);

                var control = GetPixelMeasurementsTuple(controlElementDimensions);
                var test = GetPixelMeasurementsTuple(testElementDimensions);

                var deltaX = secondMouseEventArgs.ClientX - firstMouseEventArgs.ClientX;
                var deltaY = secondMouseEventArgs.ClientY - firstMouseEventArgs.ClientY;

                Assert.True(deltaX > 0);
                Assert.True(deltaY == 0);

                Assert.Equal(control.width + deltaX, test.width);
                Assert.Equal(control.height, test.height);
                Assert.Equal(control.left, test.left);
                Assert.Equal(control.top, test.top);
            }

            // negative
            {
                InitializeResizeHelperTests(out var controlElementDimensions);
                InitializeResizeHelperTests(out var testElementDimensions);

                var firstMouseEventArgs = new MouseEventArgs { ClientX = 1100, ClientY = 300, };
                var secondMouseEventArgs = new MouseEventArgs { ClientX = 1000, ClientY = 300, };

                ResizeHelper.ResizeEast(testElementDimensions, firstMouseEventArgs, secondMouseEventArgs);

                var control = GetPixelMeasurementsTuple(controlElementDimensions);
                var test = GetPixelMeasurementsTuple(testElementDimensions);

                var deltaX = secondMouseEventArgs.ClientX - firstMouseEventArgs.ClientX;
                var deltaY = secondMouseEventArgs.ClientY - firstMouseEventArgs.ClientY;

                Assert.True(deltaX < 0);
                Assert.True(deltaY == 0);

                // Width decreases
                Assert.Equal(control.width + deltaX, test.width);
                Assert.Equal(control.height, test.height);
                Assert.Equal(control.left, test.left);
                Assert.Equal(control.top, test.top);
            }
        }

        // deltaY
        {
            // positive
            {
                // No effect
                InitializeResizeHelperTests(out var controlElementDimensions);
                InitializeResizeHelperTests(out var testElementDimensions);

                var firstMouseEventArgs = new MouseEventArgs { ClientX = 1000, ClientY = 300, };
                var secondMouseEventArgs = new MouseEventArgs { ClientX = 1000, ClientY = 400, };

                ResizeHelper.ResizeEast(testElementDimensions, firstMouseEventArgs, secondMouseEventArgs);

                var control = GetPixelMeasurementsTuple(controlElementDimensions);
                var test = GetPixelMeasurementsTuple(testElementDimensions);

                var deltaX = secondMouseEventArgs.ClientX - firstMouseEventArgs.ClientX;
                var deltaY = secondMouseEventArgs.ClientY - firstMouseEventArgs.ClientY;

                Assert.True(deltaX == 0);
                Assert.True(deltaY > 0);

                // No effect
                Assert.Equal(control, test);
            }

            // negative
            {
                // No effect
                InitializeResizeHelperTests(out var controlElementDimensions);
                InitializeResizeHelperTests(out var testElementDimensions);

                var firstMouseEventArgs = new MouseEventArgs { ClientX = 1000, ClientY = 400, };
                var secondMouseEventArgs = new MouseEventArgs { ClientX = 1000, ClientY = 300, };

                ResizeHelper.ResizeEast(testElementDimensions, firstMouseEventArgs, secondMouseEventArgs);

                var control = GetPixelMeasurementsTuple(controlElementDimensions);
                var test = GetPixelMeasurementsTuple(testElementDimensions);

                var deltaX = secondMouseEventArgs.ClientX - firstMouseEventArgs.ClientX;
                var deltaY = secondMouseEventArgs.ClientY - firstMouseEventArgs.ClientY;

                Assert.True(deltaX == 0);
                Assert.True(deltaY < 0);

                // No effect
                Assert.Equal(control, test);
            }
        }
    }

    /// <summary>
    /// <see cref="ResizeHelper.ResizeSouth(ElementDimensions, MouseEventArgs, MouseEventArgs)"/>
    /// </summary>
    [Fact]
    public void ResizeSouth()
    {
        // deltaX
        {
            // positive
            {
                // No effect
                InitializeResizeHelperTests(out var controlElementDimensions);
                InitializeResizeHelperTests(out var testElementDimensions);

                var firstMouseEventArgs = new MouseEventArgs { ClientX = 1000, ClientY = 300, };
                var secondMouseEventArgs = new MouseEventArgs { ClientX = 1100, ClientY = 300, };

                ResizeHelper.ResizeSouth(testElementDimensions, firstMouseEventArgs, secondMouseEventArgs);

                var control = GetPixelMeasurementsTuple(controlElementDimensions);
                var test = GetPixelMeasurementsTuple(testElementDimensions);

                var deltaX = secondMouseEventArgs.ClientX - firstMouseEventArgs.ClientX;
                var deltaY = secondMouseEventArgs.ClientY - firstMouseEventArgs.ClientY;

                Assert.True(deltaX > 0);
                Assert.True(deltaY == 0);

                // No effect
                Assert.Equal(control, test);
            }

            // negative
            {
                // No effect
                InitializeResizeHelperTests(out var controlElementDimensions);
                InitializeResizeHelperTests(out var testElementDimensions);

                var firstMouseEventArgs = new MouseEventArgs { ClientX = 1100, ClientY = 300, };
                var secondMouseEventArgs = new MouseEventArgs { ClientX = 1000, ClientY = 300, };

                ResizeHelper.ResizeSouth(testElementDimensions, firstMouseEventArgs, secondMouseEventArgs);

                var control = GetPixelMeasurementsTuple(controlElementDimensions);
                var test = GetPixelMeasurementsTuple(testElementDimensions);

                var deltaX = secondMouseEventArgs.ClientX - firstMouseEventArgs.ClientX;
                var deltaY = secondMouseEventArgs.ClientY - firstMouseEventArgs.ClientY;

                Assert.True(deltaX < 0);
                Assert.True(deltaY == 0);

                // No effect
                Assert.Equal(control, test);
            }
        }

        // deltaY
        {
            // positive
            {
                InitializeResizeHelperTests(out var controlElementDimensions);
                InitializeResizeHelperTests(out var testElementDimensions);

                var firstMouseEventArgs = new MouseEventArgs { ClientX = 1000, ClientY = 300, };
                var secondMouseEventArgs = new MouseEventArgs { ClientX = 1000, ClientY = 400, };

                ResizeHelper.ResizeSouth(testElementDimensions, firstMouseEventArgs, secondMouseEventArgs);

                var control = GetPixelMeasurementsTuple(controlElementDimensions);
                var test = GetPixelMeasurementsTuple(testElementDimensions);

                var deltaX = secondMouseEventArgs.ClientX - firstMouseEventArgs.ClientX;
                var deltaY = secondMouseEventArgs.ClientY - firstMouseEventArgs.ClientY;

                Assert.True(deltaX == 0);
                Assert.True(deltaY > 0);

                Assert.Equal(control.width, test.width);
                // Height increases
                Assert.Equal(control.height + deltaY, test.height);
                Assert.Equal(control.left, test.left);
                Assert.Equal(control.top, test.top);
            }

            // negative
            {
                InitializeResizeHelperTests(out var controlElementDimensions);
                InitializeResizeHelperTests(out var testElementDimensions);

                var firstMouseEventArgs = new MouseEventArgs { ClientX = 1000, ClientY = 400, };
                var secondMouseEventArgs = new MouseEventArgs { ClientX = 1000, ClientY = 300, };

                ResizeHelper.ResizeSouth(testElementDimensions, firstMouseEventArgs, secondMouseEventArgs);

                var control = GetPixelMeasurementsTuple(controlElementDimensions);
                var test = GetPixelMeasurementsTuple(testElementDimensions);

                var deltaX = secondMouseEventArgs.ClientX - firstMouseEventArgs.ClientX;
                var deltaY = secondMouseEventArgs.ClientY - firstMouseEventArgs.ClientY;

                Assert.True(deltaX == 0);
                Assert.True(deltaY < 0);

                Assert.Equal(control.width, test.width);
                // Height decreases
                Assert.Equal(control.height + deltaY, test.height);
                Assert.Equal(control.left, test.left);
                Assert.Equal(control.top, test.top);
            }
        }
    }

    /// <summary>
    /// <see cref="ResizeHelper.ResizeWest(ElementDimensions, MouseEventArgs, MouseEventArgs)"/>
    /// </summary>
    [Fact]
    public void ResizeWest()
    {
        // deltaX
        {
            // positive
            {
                InitializeResizeHelperTests(out var controlElementDimensions);
                InitializeResizeHelperTests(out var testElementDimensions);

                var firstMouseEventArgs = new MouseEventArgs { ClientX = 1000, ClientY = 300, };
                var secondMouseEventArgs = new MouseEventArgs { ClientX = 1100, ClientY = 300, };

                ResizeHelper.ResizeWest(testElementDimensions, firstMouseEventArgs, secondMouseEventArgs);

                var control = GetPixelMeasurementsTuple(controlElementDimensions);
                var test = GetPixelMeasurementsTuple(testElementDimensions);

                var deltaX = secondMouseEventArgs.ClientX - firstMouseEventArgs.ClientX;
                var deltaY = secondMouseEventArgs.ClientY - firstMouseEventArgs.ClientY;

                Assert.True(deltaX > 0);
                Assert.True(deltaY == 0);

                // Width decreases
                Assert.Equal(control.width - deltaX, test.width);
                Assert.Equal(control.height, test.height);
                // Left increases
                Assert.Equal(control.left + deltaX, test.left);
                Assert.Equal(control.top, test.top);
            }

            // negative
            {
                InitializeResizeHelperTests(out var controlElementDimensions);
                InitializeResizeHelperTests(out var testElementDimensions);

                var firstMouseEventArgs = new MouseEventArgs { ClientX = 1100, ClientY = 300, };
                var secondMouseEventArgs = new MouseEventArgs { ClientX = 1000, ClientY = 300, };

                ResizeHelper.ResizeWest(testElementDimensions, firstMouseEventArgs, secondMouseEventArgs);

                var control = GetPixelMeasurementsTuple(controlElementDimensions);
                var test = GetPixelMeasurementsTuple(testElementDimensions);

                var deltaX = secondMouseEventArgs.ClientX - firstMouseEventArgs.ClientX;
                var deltaY = secondMouseEventArgs.ClientY - firstMouseEventArgs.ClientY;

                Assert.True(deltaX < 0);
                Assert.True(deltaY == 0);

                // Width increases
                Assert.Equal(control.width - deltaX, test.width);
                Assert.Equal(control.height, test.height);
                // Left decreases
                Assert.Equal(control.left + deltaX, test.left);
                Assert.Equal(control.top, test.top);
            }
        }

        // deltaY
        {
            // positive
            {
                // No effect
                InitializeResizeHelperTests(out var controlElementDimensions);
                InitializeResizeHelperTests(out var testElementDimensions);

                var firstMouseEventArgs = new MouseEventArgs { ClientX = 1000, ClientY = 300, };
                var secondMouseEventArgs = new MouseEventArgs { ClientX = 1000, ClientY = 400, };

                ResizeHelper.ResizeWest(testElementDimensions, firstMouseEventArgs, secondMouseEventArgs);

                var control = GetPixelMeasurementsTuple(controlElementDimensions);
                var test = GetPixelMeasurementsTuple(testElementDimensions);

                var deltaX = secondMouseEventArgs.ClientX - firstMouseEventArgs.ClientX;
                var deltaY = secondMouseEventArgs.ClientY - firstMouseEventArgs.ClientY;

                Assert.True(deltaX == 0);
                Assert.True(deltaY > 0);

                // No effect
                Assert.Equal(control, test);
            }

            // negative
            {
                // No effect
                InitializeResizeHelperTests(out var controlElementDimensions);
                InitializeResizeHelperTests(out var testElementDimensions);

                var firstMouseEventArgs = new MouseEventArgs { ClientX = 1000, ClientY = 400, };
                var secondMouseEventArgs = new MouseEventArgs { ClientX = 1000, ClientY = 300, };

                ResizeHelper.ResizeWest(testElementDimensions, firstMouseEventArgs, secondMouseEventArgs);

                var control = GetPixelMeasurementsTuple(controlElementDimensions);
                var test = GetPixelMeasurementsTuple(testElementDimensions);

                var deltaX = secondMouseEventArgs.ClientX - firstMouseEventArgs.ClientX;
                var deltaY = secondMouseEventArgs.ClientY - firstMouseEventArgs.ClientY;

                Assert.True(deltaX == 0);
                Assert.True(deltaY < 0);

                // No effect
                Assert.Equal(control, test);
            }
        }
    }

    /// <summary>
    /// <see cref="ResizeHelper.ResizeNorthEast(ElementDimensions, MouseEventArgs, MouseEventArgs)"/>
    /// </summary>
    [Fact]
    public void ResizeNorthEast()
    {
        // deltaX
        {
            // positive
            {
                InitializeResizeHelperTests(out var controlElementDimensions);
                InitializeResizeHelperTests(out var testElementDimensions);

                var firstMouseEventArgs = new MouseEventArgs { ClientX = 1000, ClientY = 300, };
                var secondMouseEventArgs = new MouseEventArgs { ClientX = 1100, ClientY = 300, };

                ResizeHelper.ResizeNorthEast(testElementDimensions, firstMouseEventArgs, secondMouseEventArgs);

                var control = GetPixelMeasurementsTuple(controlElementDimensions);
                var test = GetPixelMeasurementsTuple(testElementDimensions);

                var deltaX = secondMouseEventArgs.ClientX - firstMouseEventArgs.ClientX;
                var deltaY = secondMouseEventArgs.ClientY - firstMouseEventArgs.ClientY;

                Assert.True(deltaX > 0);
                Assert.True(deltaY == 0);

                // Width increases
                Assert.Equal(control.width + deltaX, test.width);
                Assert.Equal(control.height, test.height);
                Assert.Equal(control.left, test.left);
                Assert.Equal(control.top, test.top);
            }

            // negative
            {
                InitializeResizeHelperTests(out var controlElementDimensions);
                InitializeResizeHelperTests(out var testElementDimensions);

                var firstMouseEventArgs = new MouseEventArgs { ClientX = 1100, ClientY = 300, };
                var secondMouseEventArgs = new MouseEventArgs { ClientX = 1000, ClientY = 300, };

                ResizeHelper.ResizeNorthEast(testElementDimensions, firstMouseEventArgs, secondMouseEventArgs);

                var control = GetPixelMeasurementsTuple(controlElementDimensions);
                var test = GetPixelMeasurementsTuple(testElementDimensions);

                var deltaX = secondMouseEventArgs.ClientX - firstMouseEventArgs.ClientX;
                var deltaY = secondMouseEventArgs.ClientY - firstMouseEventArgs.ClientY;

                Assert.True(deltaX < 0);
                Assert.True(deltaY == 0);

                // Width decreases
                Assert.Equal(control.width + deltaX, test.width);
                Assert.Equal(control.height, test.height);
                Assert.Equal(control.left, test.left);
                Assert.Equal(control.top, test.top);
            }
        }

        // deltaY
        {
            // positive
            {
                InitializeResizeHelperTests(out var controlElementDimensions);
                InitializeResizeHelperTests(out var testElementDimensions);

                var firstMouseEventArgs = new MouseEventArgs { ClientX = 1000, ClientY = 300, };
                var secondMouseEventArgs = new MouseEventArgs { ClientX = 1000, ClientY = 400, };

                ResizeHelper.ResizeNorthEast(testElementDimensions, firstMouseEventArgs, secondMouseEventArgs);

                var control = GetPixelMeasurementsTuple(controlElementDimensions);
                var test = GetPixelMeasurementsTuple(testElementDimensions);

                var deltaX = secondMouseEventArgs.ClientX - firstMouseEventArgs.ClientX;
                var deltaY = secondMouseEventArgs.ClientY - firstMouseEventArgs.ClientY;

                Assert.True(deltaX == 0);
                Assert.True(deltaY > 0);

                Assert.Equal(control.width, test.width);
                // Height decreases
                Assert.Equal(control.height - deltaY, test.height);
                Assert.Equal(control.left, test.left);
                // Top increases
                Assert.Equal(control.top + deltaY, test.top);
            }

            // negative
            {
                InitializeResizeHelperTests(out var controlElementDimensions);
                InitializeResizeHelperTests(out var testElementDimensions);

                var firstMouseEventArgs = new MouseEventArgs { ClientX = 1000, ClientY = 400, };
                var secondMouseEventArgs = new MouseEventArgs { ClientX = 1000, ClientY = 300, };

                ResizeHelper.ResizeNorthEast(testElementDimensions, firstMouseEventArgs, secondMouseEventArgs);

                var control = GetPixelMeasurementsTuple(controlElementDimensions);
                var test = GetPixelMeasurementsTuple(testElementDimensions);

                var deltaX = secondMouseEventArgs.ClientX - firstMouseEventArgs.ClientX;
                var deltaY = secondMouseEventArgs.ClientY - firstMouseEventArgs.ClientY;

                Assert.True(deltaX == 0);
                Assert.True(deltaY < 0);

                Assert.Equal(control.width, test.width);
                // Height increases
                Assert.Equal(control.height - deltaY, test.height);
                Assert.Equal(control.left, test.left);
                // Top decreases
                Assert.Equal(control.top + deltaY, test.top);
            }
        }
    }

    /// <summary>
    /// <see cref="ResizeHelper.ResizeSouthEast(ElementDimensions, MouseEventArgs, MouseEventArgs)"/>
    /// </summary>
    [Fact]
    public void ResizeSouthEast()
    {
        // deltaX
        {
            // positive
            {
                // Width increases
                InitializeResizeHelperTests(out var controlElementDimensions);
                InitializeResizeHelperTests(out var testElementDimensions);

                var firstMouseEventArgs = new MouseEventArgs { ClientX = 1000, ClientY = 300, };
                var secondMouseEventArgs = new MouseEventArgs { ClientX = 1100, ClientY = 300, };

                ResizeHelper.ResizeSouthEast(testElementDimensions, firstMouseEventArgs, secondMouseEventArgs);

                var control = GetPixelMeasurementsTuple(controlElementDimensions);
                var test = GetPixelMeasurementsTuple(testElementDimensions);

                var deltaX = secondMouseEventArgs.ClientX - firstMouseEventArgs.ClientX;
                var deltaY = secondMouseEventArgs.ClientY - firstMouseEventArgs.ClientY;

                Assert.True(deltaX > 0);
                Assert.True(deltaY == 0);

                Assert.Equal(control.width + deltaX, test.width);
                Assert.Equal(control.height, test.height);
                Assert.Equal(control.left, test.left);
                Assert.Equal(control.top, test.top);
            }

            // negative
            {
                InitializeResizeHelperTests(out var controlElementDimensions);
                InitializeResizeHelperTests(out var testElementDimensions);

                var firstMouseEventArgs = new MouseEventArgs { ClientX = 1100, ClientY = 300, };
                var secondMouseEventArgs = new MouseEventArgs { ClientX = 1000, ClientY = 300, };

                ResizeHelper.ResizeSouthEast(testElementDimensions, firstMouseEventArgs, secondMouseEventArgs);

                var control = GetPixelMeasurementsTuple(controlElementDimensions);
                var test = GetPixelMeasurementsTuple(testElementDimensions);

                var deltaX = secondMouseEventArgs.ClientX - firstMouseEventArgs.ClientX;
                var deltaY = secondMouseEventArgs.ClientY - firstMouseEventArgs.ClientY;

                Assert.True(deltaX < 0);
                Assert.True(deltaY == 0);

                // Width decreases
                Assert.Equal(control.width + deltaX, test.width);
                Assert.Equal(control.height, test.height);
                Assert.Equal(control.left, test.left);
                Assert.Equal(control.top, test.top);
            }
        }

        // deltaY
        {
            // positive
            {
                InitializeResizeHelperTests(out var controlElementDimensions);
                InitializeResizeHelperTests(out var testElementDimensions);

                var firstMouseEventArgs = new MouseEventArgs { ClientX = 1000, ClientY = 300, };
                var secondMouseEventArgs = new MouseEventArgs { ClientX = 1000, ClientY = 400, };

                ResizeHelper.ResizeSouthEast(testElementDimensions, firstMouseEventArgs, secondMouseEventArgs);

                var control = GetPixelMeasurementsTuple(controlElementDimensions);
                var test = GetPixelMeasurementsTuple(testElementDimensions);

                var deltaX = secondMouseEventArgs.ClientX - firstMouseEventArgs.ClientX;
                var deltaY = secondMouseEventArgs.ClientY - firstMouseEventArgs.ClientY;

                Assert.True(deltaX == 0);
                Assert.True(deltaY > 0);

                Assert.Equal(control.width, test.width);
                // Height increases
                Assert.Equal(control.height + deltaY, test.height);
                Assert.Equal(control.left, test.left);
                Assert.Equal(control.top, test.top);
            }

            // negative
            {
                InitializeResizeHelperTests(out var controlElementDimensions);
                InitializeResizeHelperTests(out var testElementDimensions);

                var firstMouseEventArgs = new MouseEventArgs { ClientX = 1000, ClientY = 400, };
                var secondMouseEventArgs = new MouseEventArgs { ClientX = 1000, ClientY = 300, };

                ResizeHelper.ResizeSouthEast(testElementDimensions, firstMouseEventArgs, secondMouseEventArgs);

                var control = GetPixelMeasurementsTuple(controlElementDimensions);
                var test = GetPixelMeasurementsTuple(testElementDimensions);

                var deltaX = secondMouseEventArgs.ClientX - firstMouseEventArgs.ClientX;
                var deltaY = secondMouseEventArgs.ClientY - firstMouseEventArgs.ClientY;

                Assert.True(deltaX == 0);
                Assert.True(deltaY < 0);

                Assert.Equal(control.width, test.width);
                // Height decreases
                Assert.Equal(control.height + deltaY, test.height);
                Assert.Equal(control.left, test.left);
                Assert.Equal(control.top, test.top);
            }
        }
    }

    /// <summary>
    /// <see cref="ResizeHelper.ResizeSouthWest(ElementDimensions, MouseEventArgs, MouseEventArgs)"/>
    /// </summary>
    [Fact]
    public void ResizeSouthWest()
    {
        // deltaX
        {
            // positive
            {
                InitializeResizeHelperTests(out var controlElementDimensions);
                InitializeResizeHelperTests(out var testElementDimensions);

                var firstMouseEventArgs = new MouseEventArgs { ClientX = 1000, ClientY = 300, };
                var secondMouseEventArgs = new MouseEventArgs { ClientX = 1100, ClientY = 300, };

                ResizeHelper.ResizeSouthWest(testElementDimensions, firstMouseEventArgs, secondMouseEventArgs);

                var control = GetPixelMeasurementsTuple(controlElementDimensions);
                var test = GetPixelMeasurementsTuple(testElementDimensions);

                var deltaX = secondMouseEventArgs.ClientX - firstMouseEventArgs.ClientX;
                var deltaY = secondMouseEventArgs.ClientY - firstMouseEventArgs.ClientY;

                Assert.True(deltaX > 0);
                Assert.True(deltaY == 0);

                // Width decreases
                Assert.Equal(control.width - deltaX, test.width);
                Assert.Equal(control.height, test.height);
                // Left increases
                Assert.Equal(control.left + deltaX, test.left);
                Assert.Equal(control.top, test.top);
            }

            // negative
            {
                InitializeResizeHelperTests(out var controlElementDimensions);
                InitializeResizeHelperTests(out var testElementDimensions);

                var firstMouseEventArgs = new MouseEventArgs { ClientX = 1100, ClientY = 300, };
                var secondMouseEventArgs = new MouseEventArgs { ClientX = 1000, ClientY = 300, };

                ResizeHelper.ResizeSouthWest(testElementDimensions, firstMouseEventArgs, secondMouseEventArgs);

                var control = GetPixelMeasurementsTuple(controlElementDimensions);
                var test = GetPixelMeasurementsTuple(testElementDimensions);

                var deltaX = secondMouseEventArgs.ClientX - firstMouseEventArgs.ClientX;
                var deltaY = secondMouseEventArgs.ClientY - firstMouseEventArgs.ClientY;

                Assert.True(deltaX < 0);
                Assert.True(deltaY == 0);

                // Width increases
                Assert.Equal(control.width - deltaX, test.width);
                Assert.Equal(control.height, test.height);
                // Left decreases
                Assert.Equal(control.left + deltaX, test.left);
                Assert.Equal(control.top, test.top);
            }
        }

        // deltaY
        {
            // positive
            {
                InitializeResizeHelperTests(out var controlElementDimensions);
                InitializeResizeHelperTests(out var testElementDimensions);

                var firstMouseEventArgs = new MouseEventArgs { ClientX = 1000, ClientY = 300, };
                var secondMouseEventArgs = new MouseEventArgs { ClientX = 1000, ClientY = 400, };

                ResizeHelper.ResizeSouthWest(testElementDimensions, firstMouseEventArgs, secondMouseEventArgs);

                var control = GetPixelMeasurementsTuple(controlElementDimensions);
                var test = GetPixelMeasurementsTuple(testElementDimensions);

                var deltaX = secondMouseEventArgs.ClientX - firstMouseEventArgs.ClientX;
                var deltaY = secondMouseEventArgs.ClientY - firstMouseEventArgs.ClientY;

                Assert.True(deltaX == 0);
                Assert.True(deltaY > 0);

                Assert.Equal(control.width, test.width);
                // Height increases
                Assert.Equal(control.height + deltaY, test.height);
                Assert.Equal(control.left, test.left);
                Assert.Equal(control.top, test.top);
            }

            // negative
            {
                InitializeResizeHelperTests(out var controlElementDimensions);
                InitializeResizeHelperTests(out var testElementDimensions);

                var firstMouseEventArgs = new MouseEventArgs { ClientX = 1000, ClientY = 400, };
                var secondMouseEventArgs = new MouseEventArgs { ClientX = 1000, ClientY = 300, };

                ResizeHelper.ResizeSouthWest(testElementDimensions, firstMouseEventArgs, secondMouseEventArgs);

                var control = GetPixelMeasurementsTuple(controlElementDimensions);
                var test = GetPixelMeasurementsTuple(testElementDimensions);

                var deltaX = secondMouseEventArgs.ClientX - firstMouseEventArgs.ClientX;
                var deltaY = secondMouseEventArgs.ClientY - firstMouseEventArgs.ClientY;

                Assert.True(deltaX == 0);
                Assert.True(deltaY < 0);

                Assert.Equal(control.width, test.width);
                // Height decreases
                Assert.Equal(control.height + deltaY, test.height);
                Assert.Equal(control.left, test.left);
                Assert.Equal(control.top, test.top);
            }
        }
    }

    /// <summary>
    /// <see cref="ResizeHelper.ResizeNorthWest(ElementDimensions, MouseEventArgs, MouseEventArgs)"/>
    /// </summary>
    [Fact]
    public void ResizeNorthWest()
    {
        // deltaX
        {
            // positive
            {
                InitializeResizeHelperTests(out var controlElementDimensions);
                InitializeResizeHelperTests(out var testElementDimensions);

                var firstMouseEventArgs = new MouseEventArgs { ClientX = 1000, ClientY = 300, };
                var secondMouseEventArgs = new MouseEventArgs { ClientX = 1100, ClientY = 300, };

                ResizeHelper.ResizeNorthWest(testElementDimensions, firstMouseEventArgs, secondMouseEventArgs);

                var control = GetPixelMeasurementsTuple(controlElementDimensions);
                var test = GetPixelMeasurementsTuple(testElementDimensions);

                var deltaX = secondMouseEventArgs.ClientX - firstMouseEventArgs.ClientX;
                var deltaY = secondMouseEventArgs.ClientY - firstMouseEventArgs.ClientY;

                Assert.True(deltaX > 0);
                Assert.True(deltaY == 0);

                // Width decreases
                Assert.Equal(control.width - deltaX, test.width);
                Assert.Equal(control.height, test.height);
                // Left increases
                Assert.Equal(control.left + deltaX, test.left);
                Assert.Equal(control.top, test.top);
            }

            // negative
            {
                InitializeResizeHelperTests(out var controlElementDimensions);
                InitializeResizeHelperTests(out var testElementDimensions);

                var firstMouseEventArgs = new MouseEventArgs { ClientX = 1100, ClientY = 300, };
                var secondMouseEventArgs = new MouseEventArgs { ClientX = 1000, ClientY = 300, };

                ResizeHelper.ResizeNorthWest(testElementDimensions, firstMouseEventArgs, secondMouseEventArgs);

                var control = GetPixelMeasurementsTuple(controlElementDimensions);
                var test = GetPixelMeasurementsTuple(testElementDimensions);

                var deltaX = secondMouseEventArgs.ClientX - firstMouseEventArgs.ClientX;
                var deltaY = secondMouseEventArgs.ClientY - firstMouseEventArgs.ClientY;

                Assert.True(deltaX < 0);
                Assert.True(deltaY == 0);

                // Width increases
                Assert.Equal(control.width - deltaX, test.width);
                Assert.Equal(control.height, test.height);
                // Left decreases
                Assert.Equal(control.left + deltaX, test.left);
                Assert.Equal(control.top, test.top);
            }
        }

        // deltaY
        {
            // positive
            {
                InitializeResizeHelperTests(out var controlElementDimensions);
                InitializeResizeHelperTests(out var testElementDimensions);

                var firstMouseEventArgs = new MouseEventArgs { ClientX = 1000, ClientY = 290, };
                var secondMouseEventArgs = new MouseEventArgs { ClientX = 1000, ClientY = 300, };

                ResizeHelper.ResizeNorthWest(testElementDimensions, firstMouseEventArgs, secondMouseEventArgs);

                var control = GetPixelMeasurementsTuple(controlElementDimensions);
                var test = GetPixelMeasurementsTuple(testElementDimensions);

                var deltaX = secondMouseEventArgs.ClientX - firstMouseEventArgs.ClientX;
                var deltaY = secondMouseEventArgs.ClientY - firstMouseEventArgs.ClientY;

                Assert.True(deltaX == 0);
                Assert.True(deltaY > 0);

                Assert.Equal(control.width, test.width);
                // Height decreases
                Assert.Equal(control.height - deltaY, test.height);
                Assert.Equal(control.left, test.left);
                // Top increases
                Assert.Equal(control.top + deltaY, test.top);
            }

            // negative
            {
                InitializeResizeHelperTests(out var controlElementDimensions);
                InitializeResizeHelperTests(out var testElementDimensions);

                var firstMouseEventArgs = new MouseEventArgs { ClientX = 1000, ClientY = 300, };
                var secondMouseEventArgs = new MouseEventArgs { ClientX = 1000, ClientY = 290, };

                ResizeHelper.ResizeNorthWest(testElementDimensions, firstMouseEventArgs, secondMouseEventArgs);

                var control = GetPixelMeasurementsTuple(controlElementDimensions);
                var test = GetPixelMeasurementsTuple(testElementDimensions);

                var deltaX = secondMouseEventArgs.ClientX - firstMouseEventArgs.ClientX;
                var deltaY = secondMouseEventArgs.ClientY - firstMouseEventArgs.ClientY;

                Assert.True(deltaX == 0);
                Assert.True(deltaY < 0);

                Assert.Equal(control.width, test.width);
                // Height increases
                Assert.Equal(control.height - deltaY, test.height);
                Assert.Equal(control.left, test.left);
                // Top decreases
                Assert.Equal(control.top + deltaY, test.top);
            }
        }
    }

    /// <summary>
    /// <see cref="ResizeHelper.Move(ElementDimensions, MouseEventArgs, MouseEventArgs)"/>
    /// </summary>
    [Fact]
    public void Move()
    {
        // deltaX
        {
            // positive
            {
                InitializeResizeHelperTests(out var controlElementDimensions);
                InitializeResizeHelperTests(out var testElementDimensions);

                var firstMouseEventArgs = new MouseEventArgs { ClientX = 1000, ClientY = 300, };
                var secondMouseEventArgs = new MouseEventArgs { ClientX = 1100, ClientY = 300, };

                ResizeHelper.Move(testElementDimensions, firstMouseEventArgs, secondMouseEventArgs);

                var control = GetPixelMeasurementsTuple(controlElementDimensions);
                var test = GetPixelMeasurementsTuple(testElementDimensions);

                var deltaX = secondMouseEventArgs.ClientX - firstMouseEventArgs.ClientX;
                var deltaY = secondMouseEventArgs.ClientY - firstMouseEventArgs.ClientY;

                Assert.True(deltaX > 0);
                Assert.True(deltaY == 0);

                Assert.Equal(control.width, test.width);
                Assert.Equal(control.height, test.height);
                // Left increases
                Assert.Equal(control.left + deltaX, test.left);
                Assert.Equal(control.top, test.top);
            }

            // negative
            {
                InitializeResizeHelperTests(out var controlElementDimensions);
                InitializeResizeHelperTests(out var testElementDimensions);

                var firstMouseEventArgs = new MouseEventArgs { ClientX = 1100, ClientY = 300, };
                var secondMouseEventArgs = new MouseEventArgs { ClientX = 1000, ClientY = 300, };

                ResizeHelper.Move(testElementDimensions, firstMouseEventArgs, secondMouseEventArgs);

                var control = GetPixelMeasurementsTuple(controlElementDimensions);
                var test = GetPixelMeasurementsTuple(testElementDimensions);

                var deltaX = secondMouseEventArgs.ClientX - firstMouseEventArgs.ClientX;
                var deltaY = secondMouseEventArgs.ClientY - firstMouseEventArgs.ClientY;

                Assert.True(deltaX < 0);
                Assert.True(deltaY == 0);

                Assert.Equal(control.width, test.width);
                Assert.Equal(control.height, test.height);
                // Left decreases
                Assert.Equal(control.left + deltaX, test.left);
                Assert.Equal(control.top, test.top);
            }
        }

        // deltaY
        {
            // positive
            {
                InitializeResizeHelperTests(out var controlElementDimensions);
                InitializeResizeHelperTests(out var testElementDimensions);

                var firstMouseEventArgs = new MouseEventArgs { ClientX = 1000, ClientY = 300, };
                var secondMouseEventArgs = new MouseEventArgs { ClientX = 1000, ClientY = 400, };

                ResizeHelper.Move(testElementDimensions, firstMouseEventArgs, secondMouseEventArgs);

                var control = GetPixelMeasurementsTuple(controlElementDimensions);
                var test = GetPixelMeasurementsTuple(testElementDimensions);

                var deltaX = secondMouseEventArgs.ClientX - firstMouseEventArgs.ClientX;
                var deltaY = secondMouseEventArgs.ClientY - firstMouseEventArgs.ClientY;

                Assert.True(deltaX == 0);
                Assert.True(deltaY > 0);

                Assert.Equal(control.width, test.width);
                Assert.Equal(control.height, test.height);
                Assert.Equal(control.left, test.left);
                // Top increases
                Assert.Equal(control.top + deltaY, test.top);
            }

            // negative
            {
                InitializeResizeHelperTests(out var controlElementDimensions);
                InitializeResizeHelperTests(out var testElementDimensions);

                var firstMouseEventArgs = new MouseEventArgs { ClientX = 1000, ClientY = 400, };
                var secondMouseEventArgs = new MouseEventArgs { ClientX = 1000, ClientY = 300, };

                ResizeHelper.Move(testElementDimensions, firstMouseEventArgs, secondMouseEventArgs);

                var control = GetPixelMeasurementsTuple(controlElementDimensions);
                var test = GetPixelMeasurementsTuple(testElementDimensions);

                var deltaX = secondMouseEventArgs.ClientX - firstMouseEventArgs.ClientX;
                var deltaY = secondMouseEventArgs.ClientY - firstMouseEventArgs.ClientY;

                Assert.True(deltaX == 0);
                Assert.True(deltaY < 0);

                Assert.Equal(control.width, test.width);
                Assert.Equal(control.height, test.height);
                Assert.Equal(control.left, test.left);
                // Top decreases
                Assert.Equal(control.top + deltaY, test.top);
            }
        }
    }

    private void InitializeResizeHelperTests(out ElementDimensions elementDimensions)
    {
        elementDimensions = DialogRecord.ConstructDefaultDialogDimensions();

        // Width
        {
            var width = elementDimensions.DimensionAttributeBag.Single(
                da => da.DimensionAttributeKind == DimensionAttributeKind.Width);

            var widthOffsetInPixels = width.DimensionUnitBag.FirstOrDefault(
                du => du.DimensionUnitKind == DimensionUnitKind.Pixels);

            if (widthOffsetInPixels is null)
            {
                widthOffsetInPixels = new DimensionUnit
                {
                    Value = 0,
                    DimensionOperatorKind = DimensionOperatorKind.Add,
                    DimensionUnitKind = DimensionUnitKind.Pixels
                };

                width.DimensionUnitBag.Add(widthOffsetInPixels);
            }
        }

        // Height
        {
            var height = elementDimensions.DimensionAttributeBag.Single(
                da => da.DimensionAttributeKind == DimensionAttributeKind.Height);

            var heightOffsetInPixels = height.DimensionUnitBag.FirstOrDefault(
                du => du.DimensionUnitKind == DimensionUnitKind.Pixels);

            if (heightOffsetInPixels is null)
            {
                heightOffsetInPixels = new DimensionUnit
                {
                    Value = 0,
                    DimensionOperatorKind = DimensionOperatorKind.Add,
                    DimensionUnitKind = DimensionUnitKind.Pixels
                };

                height.DimensionUnitBag.Add(heightOffsetInPixels);
            }
        }

        // Left
        {
            var left = elementDimensions.DimensionAttributeBag.Single(
                da => da.DimensionAttributeKind == DimensionAttributeKind.Left);

            var leftOffsetInPixels = left.DimensionUnitBag.FirstOrDefault(
                du => du.DimensionUnitKind == DimensionUnitKind.Pixels);

            if (leftOffsetInPixels is null)
            {
                leftOffsetInPixels = new DimensionUnit
                {
                    Value = 0,
                    DimensionOperatorKind = DimensionOperatorKind.Add,
                    DimensionUnitKind = DimensionUnitKind.Pixels
                };

                left.DimensionUnitBag.Add(leftOffsetInPixels);
            }
        }

        // Top
        {
            var top = elementDimensions.DimensionAttributeBag.Single(
                da => da.DimensionAttributeKind == DimensionAttributeKind.Top);

            var topOffsetInPixels = top.DimensionUnitBag.FirstOrDefault(
                du => du.DimensionUnitKind == DimensionUnitKind.Pixels);

            if (topOffsetInPixels is null)
            {
                topOffsetInPixels = new DimensionUnit
                {
                    Value = 0,
                    DimensionOperatorKind = DimensionOperatorKind.Add,
                    DimensionUnitKind = DimensionUnitKind.Pixels
                };

                top.DimensionUnitBag.Add(topOffsetInPixels);
            }
        }
    }

    public (double width, double height, double left, double top) GetPixelMeasurementsTuple(ElementDimensions elementDimensions)
    {
        var width = elementDimensions.DimensionAttributeBag
            .Single(da => da.DimensionAttributeKind == DimensionAttributeKind.Width)
            .DimensionUnitBag
            .First(du => du.DimensionUnitKind == DimensionUnitKind.Pixels)
            .Value;

        var height = elementDimensions.DimensionAttributeBag
            .Single(da => da.DimensionAttributeKind == DimensionAttributeKind.Height)
            .DimensionUnitBag
            .First(du => du.DimensionUnitKind == DimensionUnitKind.Pixels)
            .Value;

        var left = elementDimensions.DimensionAttributeBag
            .Single(da => da.DimensionAttributeKind == DimensionAttributeKind.Left)
            .DimensionUnitBag
            .First(du => du.DimensionUnitKind == DimensionUnitKind.Pixels)
            .Value;

        var top = elementDimensions.DimensionAttributeBag
            .Single(da => da.DimensionAttributeKind == DimensionAttributeKind.Top)
            .DimensionUnitBag
            .First(du => du.DimensionUnitKind == DimensionUnitKind.Pixels)
            .Value;

        return (width, height, left, top);
    }
}