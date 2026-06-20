/*
 * File: GameClient.cs
 * File Created: 18 Apr 2026
 * Author: BjornBEs
 * -----
 * Last Modified: 18 Apr 2026
 * Modified By: BjornBEs
 * -----
 */

using Shared.Network;
using Shared.Network.Package;
using System.Net.Sockets;

namespace PlayerClient.Client.game
{
    public class GameClient
    {
        UdpClient _socket;
        CancellationTokenSource _tokenSource;
        Action<Packet> _callback;
        public GameClient()
        {
        }

        public void Initialize()
        {
            _socket = new UdpClient();
        }

        public uint Connect(ServerInfo server, Action<Packet> callback)
        {
            if (server.ServerType == GameServerType.Offline)
            {
                return NetworkConstants.SUCCESS;
            }
            _callback = callback;
            _socket.Connect(server.EndPoint);

            _tokenSource = new CancellationTokenSource();

            Task.Run(receiveLoop);
            return NetworkConstants.SUCCESS;
        }

        private async void receiveLoop()
        {
            UdpReceiveResult result = await _socket.ReceiveAsync(_tokenSource.Token);
            Packet packet = PackageHelper.Deserialize(result.Buffer);
            _callback.Invoke(packet);
        }

        public void Disconnect()
        {
            _tokenSource.Cancel();
        }

        public async void SendPackage(Packet packet)
        {
           byte[] bytes = PackageHelper.Serialize(packet);
            await _socket.SendAsync(bytes, bytes.Length);
        }
    }
}
