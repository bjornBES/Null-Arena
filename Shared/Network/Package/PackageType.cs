/*
 * File: PackageType.cs
 * File Created: 25 May 2026
 * Author: BjornBEs
 * -----
 * Last Modified: 25 May 2026
 * Modified By: BjornBEs
 * -----
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Network.Package
{
    public enum PackageType
    {
        Input,
        FindMatch,
        ConnectMatch,
        GetMatchMap,
    }
}
