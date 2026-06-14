/*
 * File: MasterArguments.cs
 * File Created: 18 Apr 2026
 * Author: BjornBEs
 * -----
 * Last Modified: 20 Apr 2026
 * Modified By: BjornBEs
 * -----
 */

using Shared.EasyArgs;

namespace GameServer
{
    public class MasterArguments
    {
        [Arg("-m", "")]
        public bool HasMasterServer { get; set; }
    }
}
