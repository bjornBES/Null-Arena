/*
 * File: NetworkManager.cs
 * File Created: 13 Jun 2026
 * Author: BjornBEs
 * -----
 * Last Modified: 13 Jun 2026
 * Modified By: BjornBEs
 * -----
 */

using Shared.Network;
using Shared.Network.Package;
using System.Net;
using System.Net.Sockets;

namespace GameServer.game
{
    public class NetworkManager
    {
        public ServerInfo ServerInfo;

        public const int TickRate = 60;
        public const int BroadcastInterval = 3;
        public int TickCount = 0;

        IPEndPoint _ipEndPoint;
        UdpClient _server;

        Queue<InputPackage> newestInputPackages;
        public async Task Initialize(ServerInfo serverInfo)
        {
        }

        public async Task Start()
        {
            Timer gameTickTimer = new Timer(HandleTick, null, 0, 1/TickRate);
            while (true)
            {
                UdpReceiveResult result = await _server.ReceiveAsync();
                Console.WriteLine($"Got package from {result.RemoteEndPoint.Address}");
                _ = Task.Run(() => HandleClient(result));
            }
        }

        private void HandleClient(UdpReceiveResult result)
        {
            Packet packet = PackageHelper.Deserialize(result.Buffer);
            switch (packet.Type)
            {
                case PackageType.Input:
                    newestInputPackages.Enqueue((InputPackage)packet);
                    break;
                default:
                    break;
            }
        }

        private void HandleTick(object? arg)
        {



            if (TickCount % BroadcastInterval == 0)
            {

            }
        }
    }
}
