using Microsoft.Xna.Framework;

namespace Shared.Game.Maps
{
    public class MapObject
    {
        public uint Id;
        public EntityType EntityType;
        public string EntityName;
        public string Mesh;
        public string Texture;
        public Vector3 Position;
        public Quaternion Rotation;
        public Vector3 Scale;
        public TeamValue Team;
        public byte[] Data;
        public EntityMetadata Meta;  // spawn team, trigger radius, etc.
    }
}
