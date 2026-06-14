/*
 * File: NetworkConstants.cs
 * File Created: 18 Apr 2026
 * Author: BjornBEs
 * -----
 * Last Modified: 25 Apr 2026
 * Modified By: BjornBEs
 * -----
 */

namespace Shared.Network
{
    public static class NetworkConstants
    {
        public const uint CommunityGameServerID = 0x2ED67F95;
        public const uint CommunityMasterServerID = 0x4BF842C9;
        public const uint OperatedGameServerID = 0x8ACFC83F;
        public const uint OperatedMasterServerID = 0xB56A4152;
        public const uint OperatedMasterDataServerID = 0xF0B7B9DD;
        public const int GamePort = 10574;
        public const int GamePortLow = 10319;
        public const int MasterPort = 10575;
        public const int ClientMasterPort = 10576;
        public const string MasterDomainHost = "http://barespiritmaster.duckdns.org";
        public const string MasterHost = "188.228.89.213";

        public const uint ERROR_INVALID_ADDRESS =   0xE_DEADADD;
        public const uint ERROR_INVALID_STATUS =    0xE_ADA0000;
        public const uint ERROR_INVALID_SERVER =    0xE_ADEE000;
        public const uint SUCCESS = 1;
        public const uint OFFLINE_SUCCESS = 2;
    }
}
