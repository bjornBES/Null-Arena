/*
 * File: GameMatch.cs
 * File Created: 13 Jun 2026
 * Author: BjornBEs
 * -----
 * Last Modified: 13 Jun 2026
 * Modified By: BjornBEs
 * -----
 */

using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using Shared.Game.BVH;
using Shared.Game.Matchs;
using Shared.Game.Simulator;
using Shared.Network;
using Shared.Network.Package;

namespace GameServer.game
{
    public class GameMatch
    {
        public bool IsPlaying { get; set; }
        public int PlayerCount { get; set; }
        public int PlayingPlayerCount { get; set; }

        public MatchId MatchId;
        public Dictionary<IPEndPoint, PlayerState> Players;
        public BVHTree MapBVH;
        public int TickCount;

        public void WakeUp(ServerInfo serverInfo)
        {
            IsPlaying = true;
            Players = new Dictionary<IPEndPoint, PlayerState>(serverInfo.ServerConfig.MaxPlayers);
        }

        public void WAKEUPWakeupGrabABrushAndPutALittleMakeUp(ServerInfo serverInfo)
        {
            WakeUp(serverInfo);
        }

        public void STOP()
        {
            IsPlaying = false;
        }

        public void Tick()
        {

        }

        public void EnqueuePacket(Packet packet)
        {

        }
    }
}
