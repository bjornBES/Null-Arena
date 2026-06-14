/*
 * File: MasterServerLink.cs
 * File Created: 13 Jun 2026
 * Author: BjornBEs
 * -----
 * Last Modified: 13 Jun 2026
 * Modified By: BjornBEs
 * -----
 */

using Shared;
using Shared.Network;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace GameServer
{
    public class MasterServerLink
    {
        private CancellationTokenSource _listener_cts;
        public MasterServerLink()
        {
        }

        public async Task<ServerInfo?> Initialize(Server server)
        {
            ServerInfo serverInfo = new ServerInfo("", server.LocalAddress.Address.ToString(), NetworkConstants.GamePort, "", server.GameType);

            if (server.MasterArgs.HasMasterServer)
            {
                serverInfo.ServerName = await RegisterWithMaster(serverInfo.EndPoint, "EU_DK", server);
            }
            else
            {
                serverInfo.ServerName = "EU_DK_server0";
            }

            _listener_cts = new CancellationTokenSource();
            _ = Task.Run(() => acceptLoop(serverInfo, _listener_cts.Token));

            serverInfo.MaxMatches = 1;
            await UpdateServerInfoGTM(serverInfo, server);

            return serverInfo;
        }

        private async Task acceptLoop(ServerInfo serverInfo, CancellationToken ct)
        {
            TcpListener listener = new TcpListener(IPAddress.Any, serverInfo.ServerPort);
            listener.Start();
            while (!ct.IsCancellationRequested)
            {
                try
                {
                    TcpClient client = await listener.AcceptTcpClientAsync();
                    Console.WriteLine("Got new client");
                    NetworkStream stream = client.GetStream();

                    byte[] buf = new byte[4096];
                    int n = await stream.ReadAsync(buf, 0, buf.Length);
                    string response = Encoding.UTF8.GetString(buf, 0, n);
                    Console.WriteLine($"got {response}");

                    string[] responseSegments = response.Split(':');
                    string server_name = responseSegments.Last();
                    Console.WriteLine($"command:{responseSegments.First()} {server_name} == {serverInfo.ServerName} = {serverInfo.ServerName.Equals(server_name)}");
                    if (serverInfo.ServerName.Equals(server_name))
                    {
                        string command = responseSegments.First();
                        if (command.Equals("HEALTH"))
                        {
                            Console.WriteLine($"writing");
                            byte[] msg = Encoding.UTF8.GetBytes($"OK");
                            await stream.WriteAsync(msg, 0, msg.Length);
                        }
                        else if (command.Equals("DISCONNECTED"))
                        {
                            Console.WriteLine("master server has send disconnected command");
                        }
                        else
                        {
                            byte[] msg = Encoding.UTF8.GetBytes($"ERROR:{serverInfo.ServerName}:ERROR WRONG COMMAND");
                            await stream.WriteAsync(msg, 0, msg.Length);
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                    break;
                }
            }
        }

        private async Task<TcpClient> connectToMaster(Server server)
        {
            TcpClient client = new TcpClient();
            IPAddress masterAddress = IPAddress.Parse(NetworkConstants.MasterHost);
            if (server.MasterArgs.HasMasterServer)
            {
                await client.ConnectAsync(masterAddress, NetworkConstants.MasterPort);
            }
            return client;
        }
        private async Task<string> receiveDataFromMaster(NetworkStream masterStream, byte[]? buffer = null)
        {
            byte[] buf;
            if (buffer == null)
            {
                buf = new byte[4096];
            }
            else
            {
                buf = buffer;
            }
            int n = await masterStream.ReadAsync(buf, 0, buf.Length);
            string response = Encoding.UTF8.GetString(buf, 0, n);
            Console.WriteLine($"Master response: {response}");
            return response;
        }

        async Task<bool> checkMasterResponse(string response)
        {
            string[] responseSegments = response.Split(':');
            string code = responseSegments.First();
            if (code.Equals("OK"))
            {
                return true;
            }
            return false;
        }

        public async Task<string> RegisterWithMaster(IPEndPoint address, string region_tag, Server server)
        {
            TcpClient master = await connectToMaster(server);

            NetworkStream stream = master.GetStream();
            byte[] msg = Encoding.UTF8.GetBytes($"REGISTER:{address.Address}:{address.Port}:{region_tag}:{server.GameType}");
            await stream.WriteAsync(msg, 0, msg.Length);

            string response = await receiveDataFromMaster(stream);
            // OK:{ServerName}
            if (!await checkMasterResponse(response))
            {
                Console.WriteLine("Something fucked up");
            }
            string[] responseSegments = response.Split(':');
            string server_name = responseSegments[1];
            return server_name;
        }

        public async Task<bool> UnregisterWithMaster(ServerInfo serverInfo, Server server)
        {
            TcpClient master = await connectToMaster(server);

            NetworkStream stream = master.GetStream();
            byte[] msg = Encoding.UTF8.GetBytes($"UNREGISTER:{serverInfo.ServerName}");
            await stream.WriteAsync(msg, 0, msg.Length);

            string response = await receiveDataFromMaster(stream);
            if (!await checkMasterResponse(response))
            {

            }
            // OK:{ServerName}
            string[] responseSegments = response.Split(':');
            string server_name = responseSegments[1];
            return string.Equals(serverInfo.ServerName, server_name);
        }

        public async Task<string[]> CheckMasterServer(Server server)
        {
            TcpClient master = await connectToMaster(server);

            NetworkStream stream = master.GetStream();
            byte[] msg = Encoding.UTF8.GetBytes($"HEALTH:");
            await stream.WriteAsync(msg, 0, msg.Length);

            byte[] buf = new byte[4096];
            string response = "";
            List<string> server_list = new List<string>();
            while (true)
            {
                response = await receiveDataFromMaster(stream, buf);
                if (response.StartsWith("OK"))
                {
                    break;
                }
                if (response.StartsWith("DISCONNECTED:"))
                {
                    string serverName = response.Substring("DISCONNECTED:".Length);
                    Console.WriteLine($"disconnecting {serverName}");
                    server_list.Add(serverName);
                }
            }
            // OK
            if (!await checkMasterResponse(response))
            {

            }
            string[] responseSegments = response.Split(':');
            return server_list.ToArray();
        }

        public async Task<bool> UpdateServerInfoGTM(ServerInfo serverInfo, Server server)
        {
            if (!server.MasterArgs.HasMasterServer)
            {
                return true;
            }
            TcpClient master = await connectToMaster(server);

            NetworkStream stream = master.GetStream();
            byte[] msg = Encoding.UTF8.GetBytes($"SYNC:{serverInfo.ServerName}:{serverInfo.MaxMatches}");
            await stream.WriteAsync(msg, 0, msg.Length);

            string response = await receiveDataFromMaster(stream);
            // OK:{ServerName}
            if (!await checkMasterResponse(response))
            {

            }
            string[] responseSegments = response.Split(':');
            string server_name = responseSegments[1];
            return string.Equals(serverInfo.ServerName, server_name);
        }
    }
}
