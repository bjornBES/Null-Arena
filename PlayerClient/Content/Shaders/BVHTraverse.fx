/*
 * File: BVHTraverse.fx
 * File Created: 16 Jun 2026
 * Author: BjornBEs
 * -----
 * Last Modified: 16 Jun 2026
 * Modified By: BjornBEs
 * -----
 */

#if OPENGL
#define SV_POSITION POSITION
#define VS_SHADERMODEL vs_3_0
#define PS_SHADERMODEL ps_3_0
#define CS_SHADERMODEL cs_5_0
#else
#define VS_SHADERMODEL vs_4_0_level_9_1
#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

#define GroupSize 64

struct BVHTriangle
{
    float3 A;
    float3 B;
    float3 C;
};

struct BoundingBox
{
    float3 Min;
    float3 Max;
};

struct BVHNode
{
    BoundingBox Bounds;

    int LeftChild;
    int RightChild;
    
    int TriangleStart;
    int TriangleCount;
};

struct Ray
{
    float3 Origin;
    float3 Direction;
};

struct RayHitResult
{
    bool DidHit;
    float Distance;
};
    
StructuredBuffer<BVHNode> NodesIn;
StructuredBuffer<BVHTriangle> TrianglesIn;
StructuredBuffer<Ray> RayIn;
RWStructuredBuffer<float> ResultOut;
static const float epsilon = 0.001;

bool IsLeaf(BVHNode node)
{
    return node.TriangleCount > 0;
}

bool RayAABB(Ray ray, BoundingBox bounds)
{
    float3 invDir = float3(1.0 / ray.Direction.x, 1.0 / ray.Direction.y, 1.0 / ray.Direction.z);

    float tx1 = (bounds.Min.x - ray.Origin.x) * invDir.x;
    float tx2 = (bounds.Max.x - ray.Origin.x) * invDir.x;
    float ty1 = (bounds.Min.y - ray.Origin.y) * invDir.y;
    float ty2 = (bounds.Max.y - ray.Origin.y) * invDir.y;
    float tz1 = (bounds.Min.z - ray.Origin.z) * invDir.z;
    float tz2 = (bounds.Max.z - ray.Origin.z) * invDir.z;

    float tmin = max(max(min(tx1, tx2), min(ty1, ty2)), min(tz1, tz2));
    float tmax = min(min(max(tx1, tx2), max(ty1, ty2)), max(tz1, tz2));

    return tmax >= 0 && tmin <= tmax;
}

bool RayTriangle(Ray ray, BVHTriangle tri, inout float distance)
{
    distance = 0.0;

    // Möller–Trumbore
    float3 edge1 = tri.B - tri.A;
    float3 edge2 = tri.C - tri.A;

    float3 h = cross(ray.Direction, edge2);
    float a = dot(edge1, h);

    if (abs(a) < epsilon)
    {
        return false; // parallel
    }

    float f = 1.0 / a;
    float3 s = ray.Origin - tri.A;
    float u = f * dot(s, h);

    if (u < 0.0 || u > 1.0)
    {
        return false;
    }

    float3 q = cross(s, edge1);
    float v = f * dot(ray.Direction, q);

    if (v < 0.0 || u + v > 1.0)
    {
        return false;
    }

    distance = f * dot(edge2, q);
    return distance > epsilon;
}

RayHitResult TraverseNodes(Ray ray)
{
    int stack[32];
    int stackPtr = 0;
    
    stack[stackPtr++] = 0;
    RayHitResult hit;
    hit.Distance = 3.402823466e+38;
    hit.DidHit = false;
    while (stackPtr > 0)
    {
        BVHNode node = NodesIn[stack[--stackPtr]];

        if (!RayAABB(ray, node.Bounds))
        {
            continue;
        }

        if (IsLeaf(node))
        {
            for (int i = node.TriangleStart; i < node.TriangleStart + node.TriangleCount; i++)
            {
                float dist;
                if (RayTriangle(ray, TrianglesIn[i], dist) && dist < hit.Distance)
                {
                    hit.Distance = dist;
                    hit.DidHit = true;
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

[numthreads(GroupSize, 1, 1)]
void CS(uint3 localID : SV_GroupThreadID, uint3 groupID : SV_GroupID, uint localIndex : SV_GroupIndex, uint3 globalID : SV_DispatchThreadID)
{
    float outDistance = 0.0;
    bool result = false;
    RayHitResult hit = TraverseNodes(RayIn[globalID.x]);
    ResultOut[globalID.x] = hit.DidHit ? hit.Distance : -1.0;
    return;
}

technique Tech0
{
    pass Pass0
    {
        ComputeShader = compile cs_5_0 CS();
    }
}
