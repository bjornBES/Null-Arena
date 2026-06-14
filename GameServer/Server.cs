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
using Shared.Network;

namespace GameServer
{
    public class Server
    {
        public MasterArguments MasterArgs { get; set; }
        public IPEndPoint LocalAddress { get; set; }
        public ServerInfo ThisServerInfo { get; set; }
        public MasterServerLink MasterServerLink { get; set; }
        public NetworkManager GameServer { get; set; }
        public GameServerType GameType { get; set; }

        public static async Task<Server> CreateAsync(string[] args, GameServerType gameType)
        {
            Server server = new Server();
            server.GameType = gameType;
            {
                using Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
                socket.Connect("8.8.8.8", 80); // doesn't actually send anything
                IPEndPoint? local = (IPEndPoint?)socket.LocalEndPoint;
                if (local == null)
                {
#if DEBUG
                    Console.WriteLine("couldn't get local address using 8.8.8.8:80");
#endif
                    GameServerError.DisplayError("Error something is wrong", ErrorCodes.ErrorLocalFailed, new SocketException(10014, "Couldn't get local address"));
                    return server;
                }
                server.LocalAddress = local;
                socket.Close();
            }
            MasterArguments masterArgs = EasyArgs.Parse<MasterArguments>(args);
            Task initRun = Task.Run(MapRegistry.Init);
            server.MasterServerLink = new MasterServerLink();
            ServerInfo? serverInfo = await server.MasterServerLink.Initialize(server);
            if (!serverInfo.HasValue)
            {
                GameServerError.DisplayError("", ErrorCodes.ErrorServerInfo, new DataException("Didn't get Server info of this server"));
                return server;
            }
            server.ThisServerInfo = serverInfo.Value;
            server.GameServer = new NetworkManager();
            await server.GameServer.Initialize(serverInfo.Value);
            await initRun;
            return server;
        }
    }
}
