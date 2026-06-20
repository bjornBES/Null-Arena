/*
 * File: PacketDespatcher.cs
 * File Created: 20 Jun 2026
 * Author: BjornBEs
 * -----
 * Last Modified: 20 Jun 2026
 * Modified By: BjornBEs
 * -----
 */

using Shared.Core.Game.Matches;
using Shared.Core.Network.Package;
using Shared.Core.Network.Package.Matches;

namespace PlayerClient.Game.Gameplay.NetworkSystem
{
    public static class PacketDespatcher
    {
        public static event Action<GetMatchPackages> OnMatchStartPackages;

        public static void DispatchHandler(Packet packet)
        {
            switch (packet.Type)
            {
                case PackageType.Input:
                    break;
                case PackageType.FindMatch:
                    break;
                case PackageType.ConnectMatch:
                    break;
                case PackageType.GetMatch:
                    {
                        OnMatchStartPackages?.Invoke(packet as GetMatchPackages);
                        break;
                    }
            }
        }
    }
}
