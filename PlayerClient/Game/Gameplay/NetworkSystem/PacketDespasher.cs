using Shared.Network.Package;
using Shared.Network.Package.matchs;

namespace PlayerClient.Game.Gameplay.NetworkSystem
{
    public static class PacketDespasher
    {
        public static event Action<MatchMapPackages> OnMatchMapPackages;

        public static void DispaschHandler(Packet packet)
        {
            switch (packet.Type)
            {
                case PackageType.Input:
                    break;
                case PackageType.FindMatch:
                    break;
                case PackageType.ConnectMatch:
                    break;
                case PackageType.GetMatchMap:
                    {
                        OnMatchMapPackages?.Invoke(packet as MatchMapPackages);
                        break;
                    }
            }
        }
    }
}
