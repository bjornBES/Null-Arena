/*
 * File: PacketDespatcher.cs
 * File Created: 20 Jun 2026
 * Author: BjornBEs
 * -----
 * Last Modified: 21 Jun 2026
 * Modified By: BjornBEs
 * -----
 */

using PlayerClient.Game.Gameplay.NetworkSystem.Packets;
using Shared.Ncode.Packages;

namespace PlayerClient.Game.Gameplay.NetworkSystem
{
    public static class PacketDespatcher
    {
        public static event Action<GetMatchPackages> OnMatchStartPackages;

        public static void DispatchHandler(Packet packet)
        {
            switch ((PackageType)packet.Type)
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
