/*
 * File: MatchManager.cs
 * File Created: 13 Jun 2026
 * Author: BjornBEs
 * -----
 * Last Modified: 13 Jun 2026
 * Modified By: BjornBEs
 * -----
 */

using Shared.Network.Package;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;

namespace GameServer.game
{
    public record MatchID(ulong Id);
    public class MatchManager
    {
        public int TickCount;
        public Dictionary<MatchID, GameMatch> Matches = new Dictionary<MatchID, GameMatch>();

        public void Tick()
        {

        }

        public async void EnqueuePacket(MatchID id, Packet packet)
        {
            if (Matches.TryGetValue(id, out GameMatch? match))
            {
                match.EnqueuePacket(packet);
            }
        }
    }
}
