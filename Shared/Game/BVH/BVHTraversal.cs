using Microsoft.Xna.Framework;

namespace Shared.Game.BVH
{
    public static class BVHTraversal
    {
        public static bool RayTest(BVHTree tree, Ray ray, out float distance)
        {
            distance = float.MaxValue;
            return TraverseNode(tree, 0, ray, ref distance);
        }

        private static bool TraverseNode(BVHTree tree, int nodeIndex, Ray ray, ref float distance)
        {
            BVHNode node = tree.Nodes[nodeIndex];

            if (!RayAABB(ray, node.AABBMin, node.AABBMax))
            {
                return false;
            }

            if (node.IsLeaf)
            {
                bool hit = false;
                for (int i = node.TriangleStart; i < node.TriangleStart + node.TriangleCount; i++)
                {
                    BVHTriangle tri = tree.Triangles[i];
                    if (RayTriangle(ray, tri, out float dist) && dist < distance)
                    {
                        distance = dist;
                        hit = true;
                    }
                }
                return hit;
            }

            bool leftHit = TraverseNode(tree, node.LeftChild, ray, ref distance);
            bool rightHit = TraverseNode(tree, node.RightChild, ray, ref distance);
            return leftHit || rightHit;
        }

        private static bool RayAABB(Ray ray, Vector3 min, Vector3 max)
        {
            Vector3 invDir = new Vector3(1f / ray.Direction.X, 1f / ray.Direction.Y, 1f / ray.Direction.Z);

            float tx1 = (min.X - ray.Position.X) * invDir.X;
            float tx2 = (max.X - ray.Position.X) * invDir.X;
            float ty1 = (min.Y - ray.Position.Y) * invDir.Y;
            float ty2 = (max.Y - ray.Position.Y) * invDir.Y;
            float tz1 = (min.Z - ray.Position.Z) * invDir.Z;
            float tz2 = (max.Z - ray.Position.Z) * invDir.Z;

            float tmin = MathHelper.Max(MathHelper.Max(MathF.Min(tx1, tx2), MathF.Min(ty1, ty2)), MathF.Min(tz1, tz2));
            float tmax = MathHelper.Min(MathHelper.Min(MathF.Max(tx1, tx2), MathF.Max(ty1, ty2)), MathF.Max(tz1, tz2));

            return tmax >= 0 && tmin <= tmax;
        }

        private static bool RayTriangle(Ray ray, BVHTriangle tri, out float distance)
        {
            distance = 0f;

            // Möller–Trumbore
            Vector3 edge1 = tri.B - tri.A;
            Vector3 edge2 = tri.C - tri.A;

            Vector3 h = Vector3.Cross(ray.Direction, edge2);
            float a = Vector3.Dot(edge1, h);

            if (MathF.Abs(a) < 1e-6f)
            {
                return false; // parallel
            }

            float f = 1f / a;
            Vector3 s = ray.Position - tri.A;
            float u = f * Vector3.Dot(s, h);

            if (u < 0f || u > 1f)
            {
                return false;
            }

            Vector3 q = Vector3.Cross(s, edge1);
            float v = f * Vector3.Dot(ray.Direction, q);

            if (v < 0f || u + v > 1f)
            {
                return false;
            }

            distance = f * Vector3.Dot(edge2, q);
            return distance > 1e-6f;
        }

        public static bool SphereSweepTest(BVHTree tree, Vector3 sphereCenter, float radius, Vector3 moveDir, float moveDist, out float hitDistance, out Vector3 hitNormal)
        {
            hitDistance = moveDist;
            hitNormal = Vector3.Zero;
            bool hit = false;

            int[] stack = new int[64];
            int stackPtr = 0;
            stack[stackPtr++] = 0;

            while (stackPtr > 0)
            {
                BVHNode node = tree.Nodes[stack[--stackPtr]];

                if (!SweepAABB(sphereCenter, radius, moveDir, moveDist, node.AABBMin, node.AABBMax))
                {
                    continue;
                }

                if (node.IsLeaf)
                {
                    for (int i = node.TriangleStart; i < node.TriangleStart + node.TriangleCount; i++)
                    {
                        if (SphereTriangleSweep(sphereCenter, radius, moveDir, hitDistance, tree.Triangles[i], out float t, out Vector3 n)
                            && t < hitDistance)
                        {
                            hitDistance = t;
                            hitNormal = n;
                            hit = true;
                        }
                    }
                }
                else
                {
                    stack[stackPtr++] = node.LeftChild;
                    stack[stackPtr++] = node.RightChild;
                }
            }

            return hit;
        }

        private static bool SweepAABB(Vector3 center, float radius, Vector3 moveDir, float moveDist, Vector3 boxMin, Vector3 boxMax)
        {
            // Expand box by radius, then ray test through expanded box
            Vector3 expandedMin = boxMin - new Vector3(radius);
            Vector3 expandedMax = boxMax + new Vector3(radius);

            Vector3 invDir = new Vector3(1f / moveDir.X, 1f / moveDir.Y, 1f / moveDir.Z);

            float tx1 = (expandedMin.X - center.X) * invDir.X;
            float tx2 = (expandedMax.X - center.X) * invDir.X;
            float ty1 = (expandedMin.Y - center.Y) * invDir.Y;
            float ty2 = (expandedMax.Y - center.Y) * invDir.Y;
            float tz1 = (expandedMin.Z - center.Z) * invDir.Z;
            float tz2 = (expandedMax.Z - center.Z) * invDir.Z;

            float tmin = MathHelper.Max(MathHelper.Max(MathF.Min(tx1, tx2), MathF.Min(ty1, ty2)), MathF.Min(tz1, tz2));
            float tmax = MathHelper.Min(MathHelper.Min(MathF.Max(tx1, tx2), MathF.Max(ty1, ty2)), MathF.Max(tz1, tz2));

            return tmax >= 0 && tmin <= tmax && tmin <= moveDist;
        }
        public static bool SphereTriangleSweep(Vector3 sphereCenter, float radius, Vector3 moveDir, float moveDist, BVHTriangle tri, out float hitDistance, out Vector3 hitNormal)
        {
            hitDistance = 0f;
            hitNormal = Vector3.Zero;

            // Triangle plane
            Vector3 edge1 = tri.B - tri.A;
            Vector3 edge2 = tri.C - tri.A;
            Vector3 normal = Vector3.Normalize(Vector3.Cross(edge1, edge2));

            // Distance from sphere center to plane
            float distToPlane = Vector3.Dot(sphereCenter - tri.A, normal);

            // Velocity component along normal
            float denom = Vector3.Dot(moveDir, normal);

            if (MathF.Abs(denom) < 1e-6f)
            {
                // Moving parallel to plane, no sweep collision via plane approach
                return false;
            }

            // Distance along move where sphere surface touches plane
            float t = (radius - distToPlane) / denom;

            if (denom > 0f)
            {
                t = (-radius - distToPlane) / denom;
            }

            if (t < 0f || t > moveDist)
            {
                return false; // hit point outside this move segment
            }

            // Point where sphere center would be at time of contact
            Vector3 contactCenter = sphereCenter + moveDir * t;

            // Project that point onto the triangle plane, then check if it's inside the triangle
            Vector3 planePoint = contactCenter - normal * (Vector3.Dot(contactCenter - tri.A, normal));

            if (!PointInTriangle(planePoint, tri.A, tri.B, tri.C))
            {
                return false; // would miss the actual triangle, only hits the infinite plane
            }

            hitDistance = t;
            hitNormal = normal;
            return true;
        }

        private static bool PointInTriangle(Vector3 p, Vector3 a, Vector3 b, Vector3 c)
        {
            Vector3 v0 = c - a, v1 = b - a, v2 = p - a;

            float dot00 = Vector3.Dot(v0, v0);
            float dot01 = Vector3.Dot(v0, v1);
            float dot02 = Vector3.Dot(v0, v2);
            float dot11 = Vector3.Dot(v1, v1);
            float dot12 = Vector3.Dot(v1, v2);

            float invDenom = 1f / (dot00 * dot11 - dot01 * dot01);
            float u = (dot11 * dot02 - dot01 * dot12) * invDenom;
            float v = (dot00 * dot12 - dot01 * dot02) * invDenom;

            return (u >= 0) && (v >= 0) && (u + v <= 1);
        }
    }
}
