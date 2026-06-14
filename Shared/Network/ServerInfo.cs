/*
 * File: ServerInfo.cs
 * File Created: 18 Apr 2026
 * Author: BjornBEs
 * -----
 * Last Modified: 25 Apr 2026
 * Modified By: BjornBEs
 * -----
 */

using System.Net;

namespace Shared.Network
{
    public struct ServerInfo
    {
        public string ServerName;
        public string ServerAddress;
        public int ServerPort;

        public string Region;

        public GameServerType ServerType;

        public int MaxMatches;

        public int ActiveMatches;

        public int PlayersOnline;

        public bool IsOnline;

        public IPAddress Address => IPAddress.Parse(ServerAddress);
        public IPEndPoint EndPoint => new IPEndPoint(Address, ServerPort);
        public bool HasCapacity => ActiveMatches < MaxMatches;

        public ServerInfo(string name, string address, int port, string region, GameServerType serverType)
        {
            ServerType = serverType;
            ServerName = name;
            ServerAddress = address;
            ServerPort = port;
            Region = region;
            _init();
        }

        public ServerInfo(string name, string address, string region, GameServerType serverType)
        {
            ServerType = serverType;
            ServerName = name;
            string[] addressSegments = address.Split(':');
            ServerAddress = addressSegments.First();
            ServerPort = int.Parse(addressSegments.Last());
            Region = region;
            _init();
        }

        private void _init()
        {
            MaxMatches = 1;
            ActiveMatches = 0;
            PlayersOnline = 0;
        }
    }
}
