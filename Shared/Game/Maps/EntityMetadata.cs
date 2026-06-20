namespace Shared.Game.Maps
{
    public abstract class EntityMetadata
    {
        public byte[] Data;
    }

    public class EntityLightData : EntityMetadata
    {
        public float Intensity => BitConverter.ToSingle(Data, 0);
        public float Radius => BitConverter.ToSingle(Data, 4);
    }
}
