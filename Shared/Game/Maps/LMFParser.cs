/*
 * File: LMFParser.cs
 * File Created: 25 May 2026
 * Author: BjornBEs
 * -----
 * Last Modified: 29 May 2026
 * Modified By: BjornBEs
 * -----
 */

using System.Runtime.InteropServices;
using System.Text;

namespace Shared.Game.Maps
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public struct LMFHeaderTable
    {
        public byte TableType;
        public byte Length;
        public byte[] Entries;
    }
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    unsafe struct InternalLMFHeader
    {
        public fixed byte Magic[8];
        public fixed byte LMFVersion[3];
        public fixed byte EntityVersion[3];
        public ushort Flags;
        public byte ExtraSectorsCount;
        public fixed byte reserved1[15];
        public fixed byte MapId[20];
        public fixed byte reserved2[12];
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
        public string MapId;
        public LMFHeaderTable Table1;
        public LMFHeaderTable Table2;
        public LMFHeaderTable Table3;
        public string MapName;
    }
    public static class LMFParser
    {
        public static void ParseLMF(string FilePath)
        {
            FileInfo file = new FileInfo(FilePath);
            FileStream stream = file.OpenRead();

            byte[] buffer = new byte[512];
            stream.ReadExactly(buffer, 0, 512);
            InternalLMFHeader internalHeader = MemoryMarshal.Read<InternalLMFHeader>(buffer.AsSpan());
            Console.WriteLine(Marshal.SizeOf<InternalLMFHeader>().ToString());
            LMFHeader header = new LMFHeader();
            unsafe
            {
                header.Magic = new string((sbyte*)internalHeader.Magic, 0, 8, Encoding.ASCII);
                header.LMFVersion = new string((sbyte*)internalHeader.LMFVersion, 0, 3, Encoding.ASCII);
                header.EntityVersion = new string((sbyte*)internalHeader.EntityVersion, 0, 3, Encoding.ASCII);
                header.Flags = internalHeader.Flags;
                header.ExtraSectorsCount = internalHeader.ExtraSectorsCount;
                header.MapId = new string((sbyte*)internalHeader.MapId, 0, 20, Encoding.ASCII);
                header.MapName = new string((sbyte*)internalHeader.MapName, 0, 80, Encoding.UTF32);
            }
            unsafe
            {
                // get a span over just the table region
                byte* tablePtr = internalHeader.Table;
                Span<byte> tableSpan = new Span<byte>(tablePtr, 24);

                int offset = 0;
                LMFHeaderTable[] tables = new LMFHeaderTable[3];

                for (int i = 0; i < 3; i++)
                {
                    if (offset >= 24)
                    {
                        break;
                    }

                    byte type = tableSpan[offset];
                    byte length = tableSpan[offset + 1];

                    tables[i] = new LMFHeaderTable
                    {
                        TableType = type,
                        Length = length,
                        Entries = tableSpan.Slice(offset + 2, length).ToArray()
                    };

                    offset += 2 + length;
                }

                header.Table1 = tables[0];
                header.Table2 = tables[1];
                header.Table3 = tables[2];
            }

            if (header.Magic.Equals("BARE_LMF"))
            {
                // TODO
            }
        }
    }
}
