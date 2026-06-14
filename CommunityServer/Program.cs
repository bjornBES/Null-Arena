/*
 * File: Program.cs
 * File Created: 13 Jun 2026
 * Author: BjornBEs
 * -----
 * Last Modified: 13 Jun 2026
 * Modified By: BjornBEs
 * -----
 */

using GameServer;

namespace CommunityServer
{
    public class Program
    {
        public static async Task<int> Main(string[] args)
        {
            Server server = await Server.CreateAsync(args, GameServerType.CommunityGameServers);

            await server.GameServer.Start();

            return 0;
        }
    }
}
