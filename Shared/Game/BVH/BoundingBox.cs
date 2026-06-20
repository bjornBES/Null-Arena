using Microsoft.Xna.Framework;

namespace Shared.Game.BVH
{
    public struct BoundingBox
    {
        public Vector3 Min;
        public Vector3 Max;

        public BoundingBox()
        {
            Min = Vector3.One * float.PositiveInfinity;
            Max = Vector3.One * float.NegativeInfinity;
        }

        public void GrowToInclude(Vector3 point)
        {
            Min = Vector3.Min(Min, point);
            Max = Vector3.Min(Max, point);
        }

        public void GrowToInclude(BVHTriangle triangle)
        {
            GrowToInclude(triangle.A);
            GrowToInclude(triangle.B);
            GrowToInclude(triangle.C);
        }
    }
}
