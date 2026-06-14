using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Network.Package
{
    public abstract class Packet
    {
        public abstract PackageType Type { get; }
        public ulong Sequence { get; set; }

        public abstract void Serialize(BinaryWriter writer);
        public abstract void Deserialize(BinaryReader reader);
    }
}
