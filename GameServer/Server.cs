/*
 * File: Server.cs
 * File Created: 13 Jun 2026
 * Author: BjornBEs
 * -----
 * Last Modified: 13 Jun 2026
 * Modified By: BjornBEs
 * -----
 */

using System.Data;
using System.Net;
using System.Net.Sockets;
using GameServer.game;
using Shared;
using Shared.EasyArgs;
using Shared.Game.Maps;
using Shared.Game.Matchs;
using Shared.Network;
using Shared.Network.Package;

namespace GameServer
{
    public class Server
    {
        public const int TickRate = 60;
        public const int BroadcastInterval = 3;
        private int tickCount = 0;
        private UdpClient gameServer { get; set; }
        public MasterArguments MasterArgs { get; set; }
        public IPEndPoint LocalAddress { get; set; }
        public ServerInfo ThisServerInfo { get; set; }
        public MasterServerLink MasterServerLink { get; set; }
        public GameServerType GameType { get; set; }

        MatchManager matchManager { get; set; }

        public static async Task<Server> CreateAsync(string[] args, GameServerType gameType)
        {
            Server server = new Server();
            server.GameType = gameType;
            server.MasterArgs = EasyArgs.Parse<MasterArguments>(args);
            {
                if (server.MasterArgs.UseLocalHost)
                {
                    server.LocalAddress = new IPEndPoint(IPAddress.Loopback, 0);
                }
                else
                {
                    NetworkFunctions.GetLocalIP();
                    IPEndPoint local = NetworkFunctions.GetLocalIP(() =>
                    {
                        GameServerError.DisplayError("Error something is wrong", ErrorCodes.ErrorLocalFailed, new SocketException(10014, "Couldn't get local address"));
                    });
                    server.LocalAddress = local;
                }
            }
            Task initRun = Task.Run(MapRegistry.Init);
            server.MasterServerLink = new MasterServerLink();
            ServerInfo? serverInfo = await server.MasterServerLink.Initialize(server);
            if (!serverInfo.HasValue)
            {
                GameServerError.DisplayError("", ErrorCodes.ErrorServerInfo, new DataException("Didn't get Server info of this server"));
                return server;
            }
            server.ThisServerInfo = serverInfo.Value;
            server.matchManager = new MatchManager(server.ThisServerInfo);
            await server.MasterServerLink.UpdateServerInfoGTM(server.ThisServerInfo, server);
            server.gameServer = new UdpClient(server.ThisServerInfo.EndPoint);
            await initRun;
            return server;
        }

        public async Task WaitLoop()
        {
            Timer gameTickTimer = new Timer(async (o) =>
            {
                tickCount++;
                if ((tickCount % BroadcastInterval) == 0)
                {
                    _ = matchManager.TickAsync();
                }
            }, null, 0, 1 / TickRate);
            while (true)
            {
                UdpReceiveResult result = await gameServer.ReceiveAsync();
                Console.WriteLine($"Got package from {result.RemoteEndPoint.Address}");
                _ = Task.Run(() => HandleClient(result));
            }
        }

        private void HandleClient(UdpReceiveResult result)
        {
            Packet packet = PackageHelper.Deserialize(result.Buffer);
            switch (packet.Type)
            {
                case PackageType.FindMatch:
                    {
                        if (!matchManager.HasFreeMatchSlot())
                        {
                            // send with empty
                            break;
                        }

                        string mapId = Random.Shared.GetItems(MapRegistry.AvailableMaps, 1)[1];

                        MatchId? matchIDNull = matchManager.GetFreeMatch(mapId);
                        if (matchIDNull == null)
                        {
                            // send empty
                            break;
                        }
                        // send match id
                    }
                    break;
                case PackageType.ConnectMatch:
                case PackageType.Input:
                    matchManager.EnqueuePacket(packet.Id, packet);
                    break;
                default:
                    break;
            }
        }
    }
}
