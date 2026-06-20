/*
 * File: MasterServerClient.cs
 * File Created: 18 Apr 2026
 * Author: BjornBEs
 * -----
 * Last Modified: 20 Jun 2026
 * Modified By: BjornBEs
 * -----
 */

using System.Diagnostics;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Text;
using Shared.Core.Network;
using Shared.Network;
using Timer = System.Timers.Timer;

namespace PlayerClient.Client.master
{
    public class MasterServerClient
    {
        private UdpClient client;
        IPAddress masterAddress;
        public MasterServerClient()
        {
            if (string.IsNullOrEmpty(Game1.ClientArguments.MasterServerIp))
            {
                masterAddress = IPAddress.Parse(NetworkConstants.MasterHost);
            }
            else
            {
                masterAddress = IPAddress.Parse(Game1.ClientArguments.MasterServerIp);
            }

            using Socket socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);
            socket.Connect("8.8.8.8", 80); // doesn't actually send anything
            IPEndPoint local = (IPEndPoint)socket.LocalEndPoint;
            socket.Close();

            client = new UdpClient(new IPEndPoint(local.Address, NetworkConstants.ClientMasterPort));
        }

        public async Task<uint> ConnectAsync()
        {
            uint ping = await SendPingAsync();
            if (ping != NetworkConstants.SUCCESS)
            {
                return ping;
            }

            client.Connect(masterAddress, NetworkConstants.MasterPort);
            return NetworkConstants.SUCCESS;
        }

        public async Task<uint> SendPingAsync()
        {
            Ping ping = new Ping();
            PingReply reply = await ping.SendPingAsync(masterAddress);
            Debug.WriteLine($"Ping status for ({masterAddress}): {reply.Status}");
            if (reply.Status != IPStatus.Success)
            {
                return NetworkConstants.ERROR_INVALID_SERVER;
            }
            return NetworkConstants.SUCCESS;
        }

        private async Task SendCommandAsync(string command, params string[] args)
        {
            string arguments = string.Join(":", args);
            byte[] msg = Encoding.UTF8.GetBytes($"{command}:{arguments}");
            await client.SendAsync(msg, msg.Length);
        }

        private async Task<string> ReceiveDataAsync()
        {
            using CancellationTokenSource cts = new CancellationTokenSource();
            using Timer timer = new Timer(1000);
            timer.Elapsed += (sender, e) =>
            {
                cts.Cancel();
            };
            timer.AutoReset = false;
            timer.Start();
            UdpReceiveResult data = await client.ReceiveAsync(cts.Token);
            timer.Stop();
            if (cts.IsCancellationRequested)
            {
                return "";
            }
            return Encoding.UTF8.GetString(data.Buffer, 0, data.Buffer.Length);
        }

        public async Task<List<ServerInfo>> FetchServerListAsync()
        {
            List<ServerInfo> result = new List<ServerInfo>();

            await SendCommandAsync("LIST");

            string fullResponse = "";

            while (true)
            {
                string response = await ReceiveDataAsync();

                if (response.Contains("OK"))
                {
                    break;
                }
            }

            while (true)
            {
                string response = await ReceiveDataAsync();
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
                    if (parts.Length != 4)
                    {
                        Console.WriteLine("List is invalid");
                        break;
                    }
                    if (!Enum.TryParse(parts[3], out GameServerType type))
                    {
                        Console.WriteLine("List is invalid");
                        break;
                    }
                    result.Add(new ServerInfo(parts[0], parts[1], parts[2], type));
                }
            }

            return result;
        }
    }
}
