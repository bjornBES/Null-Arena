using Microsoft.Xna.Framework;

namespace Shared.Game.Maps
{
    public class Map
    {
        public string Name;
        public MapObject[] Objects;
        public MapGameTypes[] MapTypes;
        public MapUsageType[] UageTypes;
        public MapTeamTypes[] TeamTypes;
        public MapObject[] SpawnPoints;
        public float AmbientLightIntensity;
        public Color AmbientLightColor;
        // maybe: BoundingBox, etc.
    }
}
