using Microsoft.Xna.Framework;

namespace Shared.Game.BVH
{
    public struct BVHNode
    {
        public BoundingBox Bounds => new BoundingBox() { Max = AABBMax, Min = AABBMin };
        public Vector3 AABBMin; //=> Bounds.Min;
        public Vector3 AABBMax; //=> Bounds.Max;

        public int LeftChild;   // index into BVHNode array, -1 if leaf
        public int RightChild;  // index into BVHNode array, -1 if leaf

        public int TriangleStart; // index into triangle array, -1 if not leaf
        public int TriangleCount; // 0 if not leaf

        public bool IsLeaf => TriangleCount > 0;
    }
}
