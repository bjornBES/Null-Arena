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
using Microsoft.Xna.Framework;
using Shared.Game.Maps.LMF.Extension;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Shared.Game.Maps.LMF
{
    public enum ExtensionType
    {
        Terminator,
        ExtendedMapName,
        StringTable,
        ExtendedEntityData,
        MeshTable,
        TextureTable,
    }
    public static class LMFParser
    {
        public static Map ParseLMF(string FilePath)
        {
            Map result = new Map();
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
                if (header.LMFVersion == "0.1")
                {
                    header.Flags = internalHeader.Flags;
                    header.ExtraSectorsCount = internalHeader.ExtraSectorsCount;
                    header.MapId = new string((sbyte*)internalHeader.MapId, 0, 20, Encoding.ASCII);
                    header.MapName = new string((sbyte*)internalHeader.MapName, 0, 80, Encoding.UTF32);
                    header.AmbientLightIntensity = internalHeader.AmbientLightIntensity;
                }
            }
            unsafe
            {
                float* colorPtr = internalHeader.AmbientLightColor;
                float r = colorPtr[0];
                float g = colorPtr[1];
                float b = colorPtr[2];
                header.AmbientLightColor = new Color(r, g, b);
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

            if (!header.Magic.Equals("BARE_LMF"))
            {

            }

            List<MapObject> objects = new List<MapObject>();
            result.Name = header.MapName;
            result.AmbientLightIntensity = header.AmbientLightIntensity;
            result.AmbientLightColor = header.AmbientLightColor;
            {
                int tableIndex = 1;
                LMFHeaderTable headerTable = header.Table1;
            repeat:
                byte[] entryData = headerTable.Entries;
                switch (headerTable.TableType)
                {
                    case 0x01:
                        result.MapTypes = Array.ConvertAll(headerTable.Entries, (b) => (MapGameTypes)b);
                        break;
                    case 0x02:
                        result.UageTypes = Array.ConvertAll(headerTable.Entries, (b) => (MapUsageType)b);
                        break;
                    case 0x03:
                        // TODO make it better.
                        result.TeamTypes = Array.ConvertAll(headerTable.Entries, (b) => (MapTeamTypes)b);
                        break;
                    default:
                        break;
                }
                tableIndex++;
                if (tableIndex == 2)
                {
                    headerTable = header.Table2;
                    goto repeat;
                }
                else if (tableIndex == 3)
                {
                    headerTable = header.Table3;
                    goto repeat;
                }
            }

            {
                int lmfEntriesOffset = 512;
                Dictionary<ExtensionType, ExtensionTable> tables = new Dictionary<ExtensionType, ExtensionTable>();
                if (header.ExtraSectorsCount != 0)
                {
                    tables = parseLMFExtensionHeaders(stream, header);
                    TerminatorExtension terminator = (TerminatorExtension)tables[ExtensionType.Terminator];
                    lmfEntriesOffset = (int)terminator.ByteOffset;
                }
                result.Objects = parseLMFObjectEntries(stream, lmfEntriesOffset, tables);
                foreach (MapObject mapObject in result.Objects)
                {
                    switch (mapObject.EntityType)
                    {
                        case EntityType.ItemSpawn:
                            break;
                        case EntityType.Light:
                            mapObject.Meta = new EntityLightData();
                            mapObject.Meta.Data = mapObject.Data;
                            break;
                        default:
                            break;
                    }
                }
            }

            return result;
        }

        private static Dictionary<ExtensionType, ExtensionTable> parseLMFExtensionHeaders(FileStream stream, LMFHeader header)
        {
            int extensionSize = header.ExtraSectorsCount * 512;
            byte[] buffer = new byte[extensionSize + 1];
            stream.Position = 512;
            stream.ReadExactly(buffer, 0, extensionSize);
            Dictionary<ExtensionType, ExtensionTable> tables = new Dictionary<ExtensionType, ExtensionTable>();
            if (header.LMFVersion == "0.1")
            {
                List<(ExtensionType type, ulong LBA, int count)> values = new List<(ExtensionType type, ulong LBA, int count)>();
                for (int i = 0; i < buffer.Length;)
                {
                    ExtensionType type = (ExtensionType)BitConverter.ToUInt16(buffer, i);
                    i += 2;
                    ulong lba = BitConverter.ToUInt64(buffer, i);
                    i += 8;
                    int count = BitConverter.ToInt32(buffer, i) * 512;
                    i += 4;
                    // reserved
                    i += 2;


                    values.Add((type, lba, count));
                    if (type == ExtensionType.Terminator)
                    {
                        break;
                    }
                }

                bool stopParsing = false;
                foreach ((ExtensionType type, ulong LBA, int count) in values)
                {
                    if (stopParsing == true)
                    {
                        break;
                    }
                    ExtensionTable? extensionTable = null;
                    switch (type)
                    {
                        case ExtensionType.Terminator:
                            TerminatorExtension terminator = new TerminatorExtension()
                            {
                                ByteOffset = LBA * 512,
                                ByteCount = count,
                            };
                            extensionTable = terminator;
                            stopParsing = true;
                            break;
                        case ExtensionType.ExtendedMapName:
                            continue;
                        case ExtensionType.StringTable:
                            {
                                StringExtensionTable stringExtension = new StringExtensionTable();
                                if (tables.ContainsKey(type))
                                {
                                    stringExtension = (StringExtensionTable)tables[type];
                                    tables.Remove(type);
                                }
                                int offset = (int)(LBA * 512);
                                byte[] stringBytes = new byte[count];
                                stream.Position = offset;
                                stream.ReadExactly(stringBytes, 0, count);
                                extensionTable = parseLMFExtensionString(stream, stringExtension, offset, stringBytes, Encoding.UTF32);
                                break;
                            }
                        case ExtensionType.ExtendedEntityData:
                            {
                                ExtendedEntityExtensionTable stringExtension = new ExtendedEntityExtensionTable();
                                if (tables.ContainsKey(type))
                                {
                                    stringExtension = (ExtendedEntityExtensionTable)tables[type];
                                    tables.Remove(type);
                                }
                                int offset = (int)(LBA * 512);
                                byte[] stringBytes = new byte[count];
                                stream.Position = offset;
                                stream.ReadExactly(stringBytes, 0, count);
                                extensionTable = parseLMFExtensionEntityData(stream, stringExtension, offset, stringBytes);
                                break;
                            }
                        case ExtensionType.MeshTable:
                            {
                                StringExtensionTable stringExtension = new StringExtensionTable();
                                if (tables.ContainsKey(type))
                                {
                                    stringExtension = (StringExtensionTable)tables[type];
                                    tables.Remove(type);
                                }
                                int offset = (int)(LBA * 512);
                                byte[] stringBytes = new byte[count];
                                stream.Position = offset;
                                stream.ReadExactly(stringBytes, 0, count);
                                extensionTable = parseLMFExtensionString(stream, stringExtension, offset, stringBytes, Encoding.UTF8);
                                break;
                            }
                        case ExtensionType.TextureTable:
                            {
                                StringExtensionTable stringExtension = new StringExtensionTable();
                                if (tables.ContainsKey(type))
                                {
                                    stringExtension = (StringExtensionTable)tables[type];
                                    tables.Remove(type);
                                }
                                int offset = (int)(LBA * 512);
                                byte[] stringBytes = new byte[count];
                                stream.Position = offset;
                                stream.ReadExactly(stringBytes, 0, count);
                                extensionTable = parseLMFExtensionString(stream, stringExtension, offset, stringBytes, Encoding.UTF8);
                                break;
                            }
                        default:
                            continue;
                    }
                    if (extensionTable == null)
                    {
                        break;
                    }
                    tables.Add(type, extensionTable);
                }
            }

            return tables;
        }

        private static ExtensionTable? parseLMFExtensionString(FileStream stream, StringExtensionTable old, int offset, byte[] stringBytes, Encoding encoding)
        {
            StringExtensionTable result = old;

            ushort count = BitConverter.ToUInt16(stringBytes);
            List<uint> offsets = new List<uint>();
            for (int i = 0; i < count; i++)
            {
                int offsetOffset = i * 4;
                uint stringOffset = BitConverter.ToUInt32(stringBytes, offsetOffset + 2);
                offsets.Add((uint)(stringOffset + offset));
            }

            for (int i = 0; i < offsets.Count; i++)
            {
                uint stringOffset = offsets[i];
                int length = 0;
                if (i + 1 != offsets.Count)
                {
                    uint nextOffset = offsets[i + 1];
                    length = (int)(nextOffset - stringOffset);
                }
                else
                {
                    break;
                }
                stream.Position = stringOffset;
                byte[] stringContens = new byte[length];
                stream.ReadExactly(stringContens, 0, length);
                string str = encoding.GetString(stringContens);
                result.StringTable.Add(str);
            }

            return result;
        }
        private static ExtensionTable? parseLMFExtensionEntityData(FileStream stream, ExtendedEntityExtensionTable old, int offset, byte[] stringBytes)
        {
            ExtendedEntityExtensionTable result = old;

            uint count = BitConverter.ToUInt32(stringBytes);
            List<uint> offsets = new List<uint>();
            for (int i = 0; i < count; i++)
            {
                int offsetOffset = i * 4;
                uint stringOffset = BitConverter.ToUInt32(stringBytes, offsetOffset + 4);
                offsets.Add((uint)(stringOffset + offset));
            }

            for (int i = 0; i < offsets.Count; i++)
            {
                ExtendedEntity data = new ExtendedEntity();
                uint entryOffset = offsets[i];
                stream.Position = entryOffset;
                byte[] entityContens = new byte[5];
                stream.ReadExactly(entityContens, 0, 5);
                data.Id = BitConverter.ToUInt32(entityContens);
                data.Length = entityContens[4];
                data.Data = new byte[data.Length];
                stream.ReadExactly(data.Data, 0, data.Length);
                result.EntityDataTable.Add(data);
                if (!result.EntityDataDictionary.ContainsKey(data.Id))
                {
                    result.EntityDataDictionary.Add(data.Id, data);
                }
                else
                {
                    ExtendedEntity entry = result.EntityDataDictionary[data.Id];
                    List<byte> entityData = entry.Data.ToList();
                    entry.Length += data.Length;
                    entityData.AddRange(data.Data);
                    entry.Data = entityData.ToArray();
                }

            }

            return result;
        }

        private static MapObject[] parseLMFObjectEntries(FileStream stream, int offset, Dictionary<ExtensionType, ExtensionTable> tables)
        {
            List<MapObject> result = new List<MapObject>();
            byte[] buffer = new byte[512];
            stream.Position = offset;

            bool running = true;
            while (running)
            {
                stream.ReadExactly(buffer, 0, 512);
                for (int i = 0; i < buffer.Length;)
                {
                    StringExtensionTable extensionTable;
                    MapObject mapObject = new MapObject();
                    int startIndex = i % 64;
                    mapObject.Id = BitConverter.ToUInt32(buffer, i);
                    i += 4;
                    if (mapObject.Id == 0)
                    {
                        running = false;
                        break;
                    }
                    mapObject.EntityType = (EntityType)buffer[i];
                    i++;
                    byte flags = buffer[i];
                    i++;

                    extensionTable = (StringExtensionTable)tables[ExtensionType.MeshTable];
                    mapObject.Mesh = extensionTable.StringTable[BitConverter.ToUInt16(buffer, i)];
                    i += 2;

                    Vector3 postiton = new Vector3();
                    postiton.X = BitConverter.ToSingle(buffer, i);
                    i += 4;
                    postiton.Y = BitConverter.ToSingle(buffer, i);
                    i += 4;
                    postiton.Z = BitConverter.ToSingle(buffer, i);
                    i += 4;
                    mapObject.Position = postiton;

                    Vector3 rotation = new Vector3();
                    rotation.X = BitConverter.ToSingle(buffer, i);
                    i += 4;
                    rotation.Y = BitConverter.ToSingle(buffer, i);
                    i += 4;
                    rotation.Z = BitConverter.ToSingle(buffer, i);
                    i += 4;
                    mapObject.Rotation = Quaternion.CreateFromAxisAngle(rotation, 0);

                    Vector3 scale = new Vector3();
                    scale.X = BitConverter.ToSingle(buffer, i);
                    i += 4;
                    scale.Y = BitConverter.ToSingle(buffer, i);
                    i += 4;
                    scale.Z = BitConverter.ToSingle(buffer, i);
                    i += 4;
                    mapObject.Scale = scale;

                    extensionTable = (StringExtensionTable)tables[ExtensionType.TextureTable];
                    ushort textureId = BitConverter.ToUInt16(buffer, i);
                    mapObject.Texture = extensionTable.StringTable[textureId];
                    i += 2;

                    if ((flags & 0x01) != 0)
                    {
                        extensionTable = (StringExtensionTable)tables[ExtensionType.StringTable];
                        mapObject.EntityName = extensionTable.StringTable[BitConverter.ToUInt16(buffer, i)];
                        i += 2;
                    }
                    if ((flags & 0x02) != 0)
                    {
                        mapObject.Team = (TeamValue)buffer[i];
                        i++;
                    }

                    if (tables.ContainsKey(ExtensionType.ExtendedEntityData))
                    {
                        ExtendedEntityExtensionTable extendedEntity = (ExtendedEntityExtensionTable)tables[ExtensionType.ExtendedEntityData];
                        if (extendedEntity.EntityDataDictionary.ContainsKey(mapObject.Id))
                        {
                            mapObject.Data = extendedEntity.EntityDataDictionary[mapObject.Id].Data;
                        }
                    }

                    i += 16 - (i % 16);
                    result.Add(mapObject);
                }
            }
            return result.ToArray();
        }
    }
}
