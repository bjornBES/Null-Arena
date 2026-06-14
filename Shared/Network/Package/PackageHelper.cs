using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Network.Package
{
    public static class PackageHelper
    {
        // Packet -> byte[]
        public static byte[] Serialize(Packet packet)
        {
            using MemoryStream ms = new MemoryStream();
            using BinaryWriter writer = new BinaryWriter(ms);

            writer.Write((byte)packet.Type); // always first byte = type
            writer.Write(packet.Sequence); // always 2nd qbyte = seq
            packet.Serialize(writer);

            return ms.ToArray();
        }

        // byte[] -> concrete packet
        public static Packet Deserialize(byte[] data)
        {
            using MemoryStream ms = new MemoryStream(data);
            using BinaryReader reader = new BinaryReader(ms);

            PackageType type = (PackageType)reader.ReadByte();
            ulong sequenceNumber = reader.ReadUInt64();

            Packet packet = type switch
            {
                _ => throw new Exception($"Unknown packet type: {type}")
            };
            packet.Sequence = sequenceNumber;

            packet.Deserialize(reader);
            return packet;
        }
    }

}
