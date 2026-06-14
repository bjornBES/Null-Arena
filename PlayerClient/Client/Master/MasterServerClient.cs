/*
 * File: MasterServerClient.cs
 * File Created: 18 Apr 2026
 * Author: BjornBEs
 * -----
 * Last Modified: 18 Apr 2026
 * Modified By: BjornBEs
 * -----
 */

using Shared.Network;
using System.Diagnostics;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;

namespace PlayerClient.Client.master
{
    public class MasterServerClient
    {
        public static async Task<List<ServerInfo>> FetchServerList()
        {
            List<ServerInfo> result = new List<ServerInfo>();

            Ping ping = new Ping();

            IPAddress masterAddress = IPAddress.Parse(NetworkConstants.MasterHost);

            PingReply reply = await ping.SendPingAsync(masterAddress);
            Debug.WriteLine($"Ping status for ({masterAddress}): {reply.Status}");
            if (reply.Status != IPStatus.Success)
            {
                return result;
            }

            using Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            socket.Connect("8.8.8.8", 80); // doesn't actually send anything
            IPEndPoint local = (IPEndPoint)socket.LocalEndPoint;
            socket.Close();

            using TcpClient tcp = new TcpClient(new IPEndPoint(local.Address, NetworkConstants.ClientMasterPort));
            await tcp.ConnectAsync(masterAddress, NetworkConstants.MasterPort);

            NetworkStream stream = tcp.GetStream();
            byte[] msg = Encoding.UTF8.GetBytes("LIST");
            await stream.WriteAsync(msg, 0, msg.Length);

            string fullResponse = "";

            while (true)
            {
                byte[] buf = new byte[4096];
                int n = await stream.ReadAsync(buf, 0, buf.Length);
                string response = Encoding.UTF8.GetString(buf, 0, n);
                Debug.WriteLine("Got " + response);
                fullResponse += response;

                if (response.Contains("OK:"))
                {
                    break;
                }
            }

            if (!fullResponse.Contains("EMPTY"))
            {
                foreach (string entry in fullResponse.Split('|'))
                {
                    if (entry.StartsWith("OK:"))
                    {
                        break;
                    }
                    string[] parts = entry.Split(',');
                    result.Add(new ServerInfo(parts[0], parts[1], parts[2])); // (Name, IP:Port, Region)
                }
            }

            return result;
        }
    }
}
