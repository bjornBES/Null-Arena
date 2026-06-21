/*
 * File: PlacedMesh.cs
 * File Created: 20 Jun 2026
 * Author: BjornBEs
 * -----
 * Last Modified: 21 Jun 2026
 * Modified By: BjornBEs
 * -----
 */

using Engine.Game.BVH;
using Engine.Game.Maps;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PlayerClient.Game.Content;

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
