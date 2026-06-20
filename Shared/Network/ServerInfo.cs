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

        public int MaxMatches => ServerConfig.MaxMatches;

        public int ActiveMatches => ServerConfig.ActiveMatches;

        public int PlayersOnline => ServerConfig.PlayersOnline;

        public bool IsOnline;

        public ServerConfig ServerConfig;

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

        public ServerInfo(string name, string address, string region, GameServerType serverType, int maxMatches, int activeMatches, int playersOnline)
        {
            ServerType = serverType;
            ServerName = name;
            string[] addressSegments = address.Split(':');
            ServerAddress = addressSegments.First();
            ServerPort = int.Parse(addressSegments.Last());
            Region = region;
            ServerConfig = new ServerConfig();
            ServerConfig.MaxMatches = maxMatches;
            ServerConfig.ActiveMatches = activeMatches;
            ServerConfig.PlayersOnline = playersOnline;
            ServerConfig.MaxPlayers = 0;
            ServerConfig.IsPasswordProtected = false;
            ServerConfig.Version = 0;
        }

        private void _init()
        {
            ServerConfig = new ServerConfig();
            ServerConfig.MaxMatches = 1;
            ServerConfig.ActiveMatches = 0;
            ServerConfig.PlayersOnline = 0;
            ServerConfig.MaxPlayers = 0;
            ServerConfig.IsPasswordProtected = false;
            ServerConfig.Version = 0;
        }
    }
    public struct ServerConfig
    {
        /// <summary>
        /// Maximum amount of matches that can run on this server
        /// </summary>
        public int MaxMatches;

        /// <summary>
        /// Amount of matches running on this server
        /// </summary>
        public int ActiveMatches;

        /// <summary>
        /// Amount of players online on this server
        /// </summary>
        public int PlayersOnline;

        /// <summary>
        /// Max players per match
        /// </summary>
        public int MaxPlayers;

        /// <summary>
        /// Indicates whether a server has a password
        /// </summary>
        public bool IsPasswordProtected;

        /// <summary>
        /// The version/protocol the server is using
        /// </summary>
        public ServerVersion Version;
    }
}
