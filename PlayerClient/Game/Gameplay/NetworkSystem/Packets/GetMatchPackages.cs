/*
 * File: GetMatchPackages.cs
 * File Created: 21 Jun 2026
 * Author: BjornBEs
 * -----
 * Last Modified: 21 Jun 2026
 * Modified By: BjornBEs
 * -----
 */

using PlayerClient.Game.Gameplay.Matches;
using Shared.Ncode.Packages;

namespace PlayerClient.Game.Gameplay.NetworkSystem.Packets
{
    // CtS
    public class FindMatchPackages : Packet
    {
        public override uint Type => (uint)PackageType.FindMatch;

        public override void Deserialize(BinaryReader reader)
        {
        }

        public override void Serialize(BinaryWriter writer)
        {
        }
    }
    // CtS
    public class ConnectMatchPackages : Packet
    {
        public override uint Type => (uint)PackageType.ConnectMatch;

        public MatchId MatchId { get; set; }


        public override void Deserialize(BinaryReader reader)
        {
        }

        public override void Serialize(BinaryWriter writer)
        {
        }
    }
    // StC
    public class GetMatchPackages : Packet
    {
        public override uint Type => (uint)PackageType.GetMatch;

        public MatchId MatchId { get; set; }
        public int MapIdLength { get; set; }
        public char[] MapId { get; set; }


        public override void Deserialize(BinaryReader reader)
        {
        }

        public override void Serialize(BinaryWriter writer)
        {
        }
    }
}