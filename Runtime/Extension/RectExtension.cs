using System;

using UnityEngine;

namespace HUtil.Runtime.Extension
{
    public static class RectExtension
    {

        public static (Rect left, Rect right) SliceVertical(this Rect rect, float leftWidth){
            return (rect.SliceLeft(leftWidth), rect.SliceRight(rect.width - leftWidth));
        }

        public static (Rect left, Rect right) SliceVerticalRatio(this Rect rect, float leftRatio){
            return (rect.SliceLeftRatio(leftRatio), rect.SliceRightRatio(1 - leftRatio));
        }

        public static Rect SliceLeft(this Rect rect, float width)
        {
            return new Rect(rect.x, rect.y, width, rect.height);
        }

        public static Rect SliceRight(this Rect rect, float width)
        {
            return new Rect(rect.x + rect.width - width, rect.y, width, rect.height);
        }

        public static Rect SliceLeftRatio(this Rect rect, float ratio)
        {
            return new Rect(rect.x, rect.y, rect.width * ratio, rect.height);
        }

        public static Rect SliceRightRatio(this Rect rect, float ratio)
        {
            return new Rect(rect.x + rect.width * ratio, rect.y, rect.width * (1 - ratio), rect.height);
        }
    }
}