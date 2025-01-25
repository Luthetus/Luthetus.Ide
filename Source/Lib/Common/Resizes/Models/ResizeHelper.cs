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
            var height = elementDimensions.DimensionAttributeList.Single(
                da => da.DimensionAttributeKind == DimensionAttributeKind.Height);

            var heightOffsetInPixels = height.DimensionUnitList.FirstOrDefault(
                du => du.DimensionUnitKind == DimensionUnitKind.Pixels);

            if (heightOffsetInPixels.Purpose is null)
            {
                heightOffsetInPixels = new DimensionUnit(0, DimensionUnitKind.Pixels, DimensionOperatorKind.Add);

                height.DimensionUnitList.Add(heightOffsetInPixels);
            }

            heightOffsetInPixels.Value -= deltaY;
        }

        // Top
        {
            var top = elementDimensions.DimensionAttributeList.Single(
                da => da.DimensionAttributeKind == DimensionAttributeKind.Top);

            var topOffsetInPixels = top.DimensionUnitList.FirstOrDefault(
                du => du.DimensionUnitKind == DimensionUnitKind.Pixels);

            if (topOffsetInPixels.Purpose is null)
            {
                topOffsetInPixels = new DimensionUnit(0, DimensionUnitKind.Pixels, DimensionOperatorKind.Add);

                top.DimensionUnitList.Add(topOffsetInPixels);
            }

            topOffsetInPixels.Value += deltaY;
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
            var width = elementDimensions.DimensionAttributeList.Single(
                da => da.DimensionAttributeKind == DimensionAttributeKind.Width);

            var widthOffsetInPixels = width.DimensionUnitList.FirstOrDefault(
                du => du.DimensionUnitKind == DimensionUnitKind.Pixels);

            if (widthOffsetInPixels.Purpose is null)
            {
                widthOffsetInPixels = new DimensionUnit(0, DimensionUnitKind.Pixels, DimensionOperatorKind.Add);

                width.DimensionUnitList.Add(widthOffsetInPixels);
            }

            widthOffsetInPixels.Value += deltaX;
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
            var height = elementDimensions.DimensionAttributeList.Single(
                da => da.DimensionAttributeKind == DimensionAttributeKind.Height);

            var heightOffsetInPixels = height.DimensionUnitList.FirstOrDefault(
                du => du.DimensionUnitKind == DimensionUnitKind.Pixels);

            if (heightOffsetInPixels.Purpose is null)
            {
                heightOffsetInPixels = new DimensionUnit(0, DimensionUnitKind.Pixels, DimensionOperatorKind.Add);

                height.DimensionUnitList.Add(heightOffsetInPixels);
            }

            heightOffsetInPixels.Value += deltaY;
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
            var width = elementDimensions.DimensionAttributeList.Single(
                da => da.DimensionAttributeKind == DimensionAttributeKind.Width);

            var widthOffsetInPixels = width.DimensionUnitList.FirstOrDefault(
                du => du.DimensionUnitKind == DimensionUnitKind.Pixels);

            if (widthOffsetInPixels.Purpose is null)
            {
                widthOffsetInPixels = new DimensionUnit(0, DimensionUnitKind.Pixels, DimensionOperatorKind.Add);

                width.DimensionUnitList.Add(widthOffsetInPixels);
            }

            widthOffsetInPixels.Value -= deltaX;
        }

        // Left
        {
            var left = elementDimensions.DimensionAttributeList.Single(
                da => da.DimensionAttributeKind == DimensionAttributeKind.Left);

            var leftOffsetInPixels = left.DimensionUnitList.FirstOrDefault(
                du => du.DimensionUnitKind == DimensionUnitKind.Pixels);

            if (leftOffsetInPixels.Purpose is null)
            {
                leftOffsetInPixels = new DimensionUnit(0, DimensionUnitKind.Pixels, DimensionOperatorKind.Add);

                left.DimensionUnitList.Add(leftOffsetInPixels);
            }

            leftOffsetInPixels.Value += deltaX;
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
            var top = elementDimensions.DimensionAttributeList.Single(
                da => da.DimensionAttributeKind == DimensionAttributeKind.Top);

            var topOffsetInPixels = top.DimensionUnitList.FirstOrDefault(
                du => du.DimensionUnitKind == DimensionUnitKind.Pixels);

            if (topOffsetInPixels.Purpose is null)
            {
                topOffsetInPixels = new DimensionUnit(0, DimensionUnitKind.Pixels, DimensionOperatorKind.Add);

                top.DimensionUnitList.Add(topOffsetInPixels);
            }

            topOffsetInPixels.Value += deltaY;
        }

        // Left
        {
            var left = elementDimensions.DimensionAttributeList.Single(
                da => da.DimensionAttributeKind == DimensionAttributeKind.Left);

            var leftOffsetInPixels = left.DimensionUnitList.FirstOrDefault(
                du => du.DimensionUnitKind == DimensionUnitKind.Pixels);

            if (leftOffsetInPixels.Purpose is null)
            {
                leftOffsetInPixels = new DimensionUnit(0, DimensionUnitKind.Pixels, DimensionOperatorKind.Add);

                left.DimensionUnitList.Add(leftOffsetInPixels);
            }

            leftOffsetInPixels.Value += deltaX;
        }
    }
}