using Luthetus.Common.RazorLib.Dimensions.Models;
using Microsoft.AspNetCore.Components.Web;

namespace Luthetus.Common.RazorLib.Resizes.Models;

public class ResizeHelper
{
    public static void ResizeNorth(
        ElementDimensions elementDimensions,
        MouseEventArgs firstMouseEventArgs,
        MouseEventArgs secondMouseEventArgs)
    {
        var deltaY = secondMouseEventArgs.ClientY - firstMouseEventArgs.ClientY;

        // Height
        {
        	elementDimensions.HeightDimensionAttribute.Decrement(
        		deltaY,
        		DimensionUnitKind.Pixels,
        		new DimensionUnit(0, DimensionUnitKind.Pixels, DimensionOperatorKind.Add));
        }

        // Top
        {
            elementDimensions.TopDimensionAttribute.Increment(
        		deltaY,
        		DimensionUnitKind.Pixels,
        		new DimensionUnit(0, DimensionUnitKind.Pixels, DimensionOperatorKind.Add));
        }
    }

    public static void ResizeEast(
        ElementDimensions elementDimensions,
        MouseEventArgs firstMouseEventArgs,
        MouseEventArgs secondMouseEventArgs)
    {
        var deltaX = secondMouseEventArgs.ClientX - firstMouseEventArgs.ClientX;

        // Width
        {
            elementDimensions.WidthDimensionAttribute.Increment(
        		deltaX,
        		DimensionUnitKind.Pixels,
        		new DimensionUnit(0, DimensionUnitKind.Pixels, DimensionOperatorKind.Add));
        }
    }

    public static void ResizeSouth(
        ElementDimensions elementDimensions,
        MouseEventArgs firstMouseEventArgs,
        MouseEventArgs secondMouseEventArgs)
    {
        var deltaY = secondMouseEventArgs.ClientY - firstMouseEventArgs.ClientY;

        // Height
        {
            elementDimensions.HeightDimensionAttribute.Increment(
        		deltaY,
        		DimensionUnitKind.Pixels,
        		new DimensionUnit(0, DimensionUnitKind.Pixels, DimensionOperatorKind.Add));
        }
    }

    public static void ResizeWest(
        ElementDimensions elementDimensions,
        MouseEventArgs firstMouseEventArgs,
        MouseEventArgs secondMouseEventArgs)
    {
        var deltaX = secondMouseEventArgs.ClientX - firstMouseEventArgs.ClientX;

        // Width
        {
            elementDimensions.WidthDimensionAttribute.Decrement(
        		deltaX,
        		DimensionUnitKind.Pixels,
        		new DimensionUnit(0, DimensionUnitKind.Pixels, DimensionOperatorKind.Add));
        }

        // Left
        {
            elementDimensions.LeftDimensionAttribute.Increment(
        		deltaX,
        		DimensionUnitKind.Pixels,
        		new DimensionUnit(0, DimensionUnitKind.Pixels, DimensionOperatorKind.Add));
        }
    }

    public static void ResizeNorthEast(
        ElementDimensions elementDimensions,
        MouseEventArgs firstMouseEventArgs,
        MouseEventArgs secondMouseEventArgs)
    {
        ResizeNorth(elementDimensions, firstMouseEventArgs, secondMouseEventArgs);
        ResizeEast(elementDimensions, firstMouseEventArgs, secondMouseEventArgs);
    }

    public static void ResizeSouthEast(
        ElementDimensions elementDimensions,
        MouseEventArgs firstMouseEventArgs,
        MouseEventArgs secondMouseEventArgs)
    {
        ResizeSouth(elementDimensions, firstMouseEventArgs, secondMouseEventArgs);
        ResizeEast(elementDimensions, firstMouseEventArgs, secondMouseEventArgs);
    }

    public static void ResizeSouthWest(
        ElementDimensions elementDimensions,
        MouseEventArgs firstMouseEventArgs,
        MouseEventArgs secondMouseEventArgs)
    {
        ResizeSouth(elementDimensions, firstMouseEventArgs, secondMouseEventArgs);
        ResizeWest(elementDimensions, firstMouseEventArgs, secondMouseEventArgs);
    }

    public static void ResizeNorthWest(
        ElementDimensions elementDimensions,
        MouseEventArgs firstMouseEventArgs,
        MouseEventArgs secondMouseEventArgs)
    {
        ResizeNorth(elementDimensions, firstMouseEventArgs, secondMouseEventArgs);
        ResizeWest(elementDimensions, firstMouseEventArgs, secondMouseEventArgs);
    }

    public static void Move(
        ElementDimensions elementDimensions,
        MouseEventArgs firstMouseEventArgs,
        MouseEventArgs secondMouseEventArgs)
    {
        var deltaY = secondMouseEventArgs.ClientY - firstMouseEventArgs.ClientY;
        var deltaX = secondMouseEventArgs.ClientX - firstMouseEventArgs.ClientX;

        // Top
        {
            elementDimensions.TopDimensionAttribute.Increment(
        		deltaY,
        		DimensionUnitKind.Pixels,
        		new DimensionUnit(0, DimensionUnitKind.Pixels, DimensionOperatorKind.Add));
        }

        // Left
        {
            elementDimensions.LeftDimensionAttribute.Increment(
        		deltaX,
        		DimensionUnitKind.Pixels,
        		new DimensionUnit(0, DimensionUnitKind.Pixels, DimensionOperatorKind.Add));
        }
    }
}