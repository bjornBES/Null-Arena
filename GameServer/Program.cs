/*
 * File: Program.cs
 * File Created: 18 Apr 2026
 * Author: BjornBEs
 * -----
 * Last Modified: 20 Apr 2026
 * Modified By: BjornBEs
 * -----
 */

using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using GameServer.game;
using Shared.EasyArgs;
using Shared.Game.Maps;
using Shared.Network;

namespace GameServer
{
    internal class Program
    {
        /*
        public static async Task<string[]> CheckMasterServer()
        {
            using Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            socket.Connect("8.8.8.8", 80); // doesn't actually send anything
            using TcpClient client = new TcpClient();
            string masterAddress = NetworkConstants.MasterHost;
            await client.ConnectAsync(masterAddress, NetworkConstants.MasterPort);
            socket.Close();

            NetworkStream stream = client.GetStream();
            byte[] msg = Encoding.UTF8.GetBytes($"HEALTH:");
            await stream.WriteAsync(msg, 0, msg.Length);

            byte[] buf = new byte[4096];
            int n = 0;
            string response = "";
            List<string> server_list = new List<string>();
            while (true)
            {
                n = await stream.ReadAsync(buf, 0, buf.Length);
                response = Encoding.UTF8.GetString(buf, 0, n);
                if (response.StartsWith("OK"))
                {
                    break;
                }
                if (response.StartsWith("DISCONNECTED:"))
                {
                    string server = response.Substring("DISCONNECTED".Length + 1);
                    Console.WriteLine($"disconnecting {server}");
                    server_list.Add(server);
                }
            }
            Console.WriteLine($"Master response: {Encoding.UTF8.GetString(buf, 0, n)}");
            Console.WriteLine($"got {response}");
            // OK:{ServerName}
            string[] responseSegments = response.Split(':');
            return server_list.ToArray();
        }
        */
    }
}
