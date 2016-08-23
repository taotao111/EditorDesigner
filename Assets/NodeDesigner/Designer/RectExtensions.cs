using UnityEngine;
namespace Designer.Runtime
{
	public static class RectExtensions
	{
		public static Vector2 TopLeft(this Rect rect)
		{
			return new Vector2(rect.xMin, rect.yMin);
		}
        public static Vector2 Top(this Rect rect)
        {
            return new Vector2(rect.xMin + rect.width * 0.5f, rect.yMin);
        }
        public static Vector2 TopRight(this Rect rect)
        {
            return new Vector2(rect.xMax, rect.yMin);
        }
        public static Vector2 CenterLeft(this Rect rect)
        {
            return new Vector2(rect.xMin, rect.yMin + rect.height * 0.5f);
        }
        public static Vector2 Center(this Rect rect)
        {
            return new Vector2(rect.xMin + rect.width * 0.5f, rect.yMin + rect.height * 0.5f);
        }
        public static Vector2 CenterRight(this Rect rect)
        {
            return new Vector2(rect.xMax, rect.yMin + rect.height * 0.5f);
        }
        public static Vector2 BottomLeft(this Rect rect)
        {
            return new Vector2(rect.xMin, rect.yMax);
        }
        public static Vector2 Bottom(this Rect rect)
        {
            return new Vector2(rect.xMin + rect.width * 0.5f, rect.yMax);
        }
        public static Vector2 BottomRight(this Rect rect)
        {
            return new Vector2(rect.xMin, rect.yMax);
        }
        public static Rect ScaleSizeBy(this Rect rect, float scale)
		{
			return rect.ScaleSizeBy(scale, rect.center);
		}
		public static Rect ScaleSizeBy(this Rect rect, float scale, Vector2 pivotPoint)
		{
			Rect result = rect;
			result.x -= pivotPoint.x;
			result.y -= pivotPoint.y;
			result.xMin *= scale;
			result.xMax *= scale;
			result.yMin *= scale;
			result.yMax *= scale;
			result.x += pivotPoint.x;
			result.y += pivotPoint.y;
			return result;
		}
		public static Rect ScaleSizeBy(this Rect rect, Vector2 scale)
		{
			return rect.ScaleSizeBy(scale, rect.center);
		}
		public static Rect ScaleSizeBy(this Rect rect, Vector2 scale, Vector2 pivotPoint)
		{
			Rect result = rect;
			result.x -= pivotPoint.x;
			result.y -= pivotPoint.y;
			result.xMin *= scale.x;
			result.xMax *= scale.x;
			result.yMin *= scale.y;
			result.yMax *= scale.y;
			result.x += pivotPoint.x;
			result.y += pivotPoint.y;
			return result;
		}
	}
}
