/*
 * File: LMFHeaderTable.cs
 * File Created: 25 May 2026
 * Author: BjornBEs
 * -----
 * Last Modified: 29 May 2026
 * Modified By: BjornBEs
 * -----
 */

using System.Runtime.InteropServices;

namespace Shared.Game.Maps.LMF
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct LMFHeaderTable
    {
        public byte TableType;
        public byte Length;
        public byte[] Entries;
    }
}
