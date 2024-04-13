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

            if (heightOffsetInPixels is null)
            {
                heightOffsetInPixels = new DimensionUnit
                {
                    Value = 0,
                    DimensionOperatorKind = DimensionOperatorKind.Add,
                    DimensionUnitKind = DimensionUnitKind.Pixels
                };

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

            if (topOffsetInPixels is null)
            {
                topOffsetInPixels = new DimensionUnit
                {
                    Value = 0,
                    DimensionOperatorKind = DimensionOperatorKind.Add,
                    DimensionUnitKind = DimensionUnitKind.Pixels
                };

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

            if (widthOffsetInPixels is null)
            {
                widthOffsetInPixels = new DimensionUnit
                {
                    Value = 0,
                    DimensionOperatorKind = DimensionOperatorKind.Add,
                    DimensionUnitKind = DimensionUnitKind.Pixels
                };

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

            if (heightOffsetInPixels is null)
            {
                heightOffsetInPixels = new DimensionUnit
                {
                    Value = 0,
                    DimensionOperatorKind = DimensionOperatorKind.Add,
                    DimensionUnitKind = DimensionUnitKind.Pixels
                };

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

            if (widthOffsetInPixels is null)
            {
                widthOffsetInPixels = new DimensionUnit
                {
                    Value = 0,
                    DimensionOperatorKind = DimensionOperatorKind.Add,
                    DimensionUnitKind = DimensionUnitKind.Pixels
                };

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

            if (leftOffsetInPixels is null)
            {
                leftOffsetInPixels = new DimensionUnit
                {
                    Value = 0,
                    DimensionOperatorKind = DimensionOperatorKind.Add,
                    DimensionUnitKind = DimensionUnitKind.Pixels
                };

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

            if (topOffsetInPixels is null)
            {
                topOffsetInPixels = new DimensionUnit
                {
                    Value = 0,
                    DimensionOperatorKind = DimensionOperatorKind.Add,
                    DimensionUnitKind = DimensionUnitKind.Pixels
                };

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

            if (leftOffsetInPixels is null)
            {
                leftOffsetInPixels = new DimensionUnit
                {
                    Value = 0,
                    DimensionOperatorKind = DimensionOperatorKind.Add,
                    DimensionUnitKind = DimensionUnitKind.Pixels
                };

                left.DimensionUnitList.Add(leftOffsetInPixels);
            }

            leftOffsetInPixels.Value += deltaX;
        }
    }
}