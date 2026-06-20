using Microsoft.Xna.Framework;

namespace Shared.Game.BVH
{
    public static class BVHBuilder
    {
        private const int MaxTrianglesPerLeaf = 4;

        public static BVHTree Build(BVHTriangle[] triangles)
        {
            List<BVHNode> nodes = new List<BVHNode>();
            List<BVHTriangle> sortedTriangles = new List<BVHTriangle>(triangles);

            BuildNode(nodes, sortedTriangles, 0, sortedTriangles.Count);

            return new BVHTree
            {
                Nodes = nodes.ToArray(),
                Triangles = sortedTriangles.ToArray()
            };
        }

        private static int BuildNode(List<BVHNode> nodes, List<BVHTriangle> triangles, int start, int count)
        {
            int nodeIndex = nodes.Count;
            nodes.Add(new BVHNode());

            BVHNode node = new BVHNode();
            ComputeAABB(triangles, start, count, out node.AABBMin, out node.AABBMax);

            if (count <= MaxTrianglesPerLeaf)
            {
                node.LeftChild = -1;
                node.RightChild = -1;
                node.TriangleStart = start;
                node.TriangleCount = count;
                nodes[nodeIndex] = node;
                return nodeIndex;
            }

            int axis = LongestAxis(node.Bounds);
            int mid = Partition(triangles, start, count, axis);

            if (mid == start || mid == start + count)
            {
                mid = start + count / 2;
            }

            node.LeftChild = BuildNode(nodes, triangles, start, mid - start);
            node.RightChild = BuildNode(nodes, triangles, mid, start + count - mid);
            node.TriangleStart = -1;
            node.TriangleCount = 0;
            nodes[nodeIndex] = node;

            return nodeIndex;
        }

        private static void ComputeAABB(List<BVHTriangle> triangles, int start, int count, out Vector3 min, out Vector3 max)
        {
            min = new Vector3(float.MaxValue);
            max = new Vector3(float.MinValue);

            for (int i = start; i < start + count; i++)
            {
                BVHTriangle t = triangles[i];
                min = Vector3.Min(min, Vector3.Min(t.A, Vector3.Min(t.B, t.C)));
                max = Vector3.Max(max, Vector3.Max(t.A, Vector3.Max(t.B, t.C)));
            }
        }
        private static int LongestAxis(BoundingBox boundingBox)
        {
            Vector3 size = boundingBox.Max - boundingBox.Min;
            if (size.X >= size.Y && size.X >= size.Z)
            {
                return 0;
            }

            if (size.Y >= size.Z)
            {
                return 1;
            }

            return 2;
        }

        private static int Partition(List<BVHTriangle> triangles, int start, int count, int axis)
        {
            float mid = 0f;
            for (int i = start; i < start + count; i++)
            {
                BVHTriangle t = triangles[i];
                Vector3 centroid = (t.A + t.B + t.C) / 3f;
                mid += axis == 0 ? centroid.X : axis == 1 ? centroid.Y : centroid.Z;
            }
            mid /= count;

            int left = start;
            int right = start + count - 1;

            while (left <= right)
            {
                BVHTriangle t = triangles[left];
                Vector3 centroid = (t.A + t.B + t.C) / 3f;
                float val = axis == 0 ? centroid.X : axis == 1 ? centroid.Y : centroid.Z;

                if (val < mid)
                {
                    left++;
                }
                else
                {
                    (triangles[left], triangles[right]) = (triangles[right], triangles[left]);
                    right--;
                }
            }

            return left;
        }
    }
}
