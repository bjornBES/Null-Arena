using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PlayerClient.Game.Content;
using Shared.Game.BVH;
using Shared.Game.Maps;

namespace PlayerClient.Game.Gameplay.MapSystem
{
    public class PlacedMesh
    {
        public MapObject MapObject;
        public MeshBuffer Mesh;
        public Texture2D Texture;
        public Matrix ModelMatrix;
        public Vector3 Position;
        public Quaternion Rotation;
        public Vector3 Scale;
        public BVHTriangle[] Triangles;
    }
}
