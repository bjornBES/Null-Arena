/*
 * File: MasterArguments.cs
 * File Created: 18 Apr 2026
 * Author: BjornBEs
 * -----
 * Last Modified: 20 Apr 2026
 * Modified By: BjornBEs
 * -----
 */

using System.ComponentModel;
using Shared.EasyArgs;

namespace GameServer
{
    public class MasterArguments
    {
        [Arg("-m", "")]
        public bool HasMasterServer { get; set; }

        [Arg("", "--maps")]
        [DefaultValue("")]
        public string Maps { get; set; }

        [Arg("-l", "--local")]
        public bool UseLocalHost{ get; set; }
    }
}
