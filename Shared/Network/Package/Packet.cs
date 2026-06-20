using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Shared.Game.Matchs;

namespace Shared.Network.Package
{
    public abstract class Packet
    {
        public MatchId Id { get; set; }
        public abstract PackageType Type { get; }
        public ulong Sequence { get; set; }

        public abstract void Serialize(BinaryWriter writer);
        public abstract void Deserialize(BinaryReader reader);
    }
}
