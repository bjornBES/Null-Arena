/*
 * File: PackageType.cs
 * File Created: 25 May 2026
 * Author: BjornBEs
 * -----
 * Last Modified: 21 Jun 2026
 * Modified By: BjornBEs
 * -----
 */

namespace Shared.Ncode.Packages
{
    public enum PackageType : uint
    {
        // Client To Server
        FindMatch,

        // Server To Client
        FoundMatch,

        // Client To Server
        ConnectMatch,

        // Server To Client
        GetMatch,

        // Client To Server
        Input,

        // Server To Client
        ServerPlayerState,
    }
}
