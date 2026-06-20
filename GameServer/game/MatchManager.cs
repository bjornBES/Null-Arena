/*
 * File: MatchManager.cs
 * File Created: 13 Jun 2026
 * Author: BjornBEs
 * -----
 * Last Modified: 13 Jun 2026
 * Modified By: BjornBEs
 * -----
 */

using System.Net;
using Shared.Game.BVH;
using Shared.Game.Matchs;
using Shared.Game.Simulator;
using Shared.Network;
using Shared.Network.Package;

namespace GameServer.game
{
    public class MatchManager
    {
        Dictionary<MatchId, GameMatch> matches = new Dictionary<MatchId, GameMatch>();
        ulong matchIndex = 0;
        ServerInfo serverInfo;

        public MatchManager(ServerInfo _serverInfo)
        {
            serverInfo = _serverInfo;
            matches = new Dictionary<MatchId, GameMatch>(serverInfo.ServerConfig.MaxMatches);
        }

        public bool HasFreeMatchSlot()
        {
            if (matches.Count < matches.Capacity)
            {
                return true;
            }
            return false;
        }

        public MatchId? GetFreeMatch(string mapId)
        {
            if (matches.Count >= matches.Capacity)
            {
                return null;
            }
            MatchId? id;
            GameMatch? gameMatch = CreateMatch(mapId, out id);
            if (gameMatch == null || id == null)
            {
                return null;
            }
            gameMatch.WAKEUPWakeupGrabABrushAndPutALittleMakeUp(serverInfo); // aka make-up
            return id;
        }

        public GameMatch? CreateMatch(string mapId, out MatchId? id)
        {
            if (matches.Count >= matches.Capacity)
            {
                id = null;
                return null;
            }
            id = new MatchId(matchIndex);
            matchIndex++;
            GameMatch gameMatch = new GameMatch()
            {
                MatchId = id,
            };
            matches.Add(id, gameMatch);
            return gameMatch;
        }

        public void FreeMatch(MatchId id)
        {
            matches[id].STOP();
            matches.Remove(id);
        }

        public async Task TickAsync()
        {
            Task task = Task.Run(() =>
            {
                foreach (GameMatch match in matches.Values)
                {
                    match.Tick();
                }
            });
            serverInfo.ServerConfig.PlayersOnline = 0;
            serverInfo.ServerConfig.ActiveMatches = 0;
            foreach (GameMatch match in matches.Values)
            {
                serverInfo.ServerConfig.PlayersOnline += match.PlayerCount;
                if (match.IsPlaying)
                {
                    serverInfo.ServerConfig.ActiveMatches++;
                }
            }
            await task;
        }

        public async void EnqueuePacket(MatchId id, Packet packet)
        {
            if (matches.TryGetValue(id, out GameMatch? match))
            {
                match.EnqueuePacket(packet);
            }
        }
    }
}
