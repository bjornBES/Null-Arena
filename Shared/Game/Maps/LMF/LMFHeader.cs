/*
 * File: LMFHeader.cs
 * File Created: 25 May 2026
 * Author: BjornBEs
 * -----
 * Last Modified: 29 May 2026
 * Modified By: BjornBEs
 * -----
 */

using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;

namespace Shared.Game.Maps.LMF
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    unsafe struct InternalLMFHeader
    {
        public fixed byte Magic[8];
        public fixed byte LMFVersion[3];
        public fixed byte EntityVersion[3];
        public ushort Flags;
        public byte ExtraSectorsCount;
        public float AmbientLightIntensity;
        public fixed byte reserved1[11];
        public fixed byte MapId[20];
        public fixed float AmbientLightColor[3];
        public fixed byte Table[24];
        public fixed byte reserved3[8];
        public fixed uint MapName[80];
        public fixed byte reserved4[96];
    }
    public struct LMFHeader
    {
        public string Magic;
        public string LMFVersion;
        public string EntityVersion;
        public ushort Flags;
        public byte ExtraSectorsCount;
        public float AmbientLightIntensity;
        public string MapId;
        public Color AmbientLightColor;
        public LMFHeaderTable Table1;
        public LMFHeaderTable Table2;
        public LMFHeaderTable Table3;
        public string MapName;
    }
}
