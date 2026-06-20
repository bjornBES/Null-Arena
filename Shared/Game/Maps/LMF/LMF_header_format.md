# BARE Level Map Format (LMF) Specification

## Version 0.1

---

## Overview

The Level Map Format (LMF) is a binary format for storing arena level data. The format is sector-based where each sector is **512 bytes**. The file is structured as a sequence of sectors starting at LBA 0.

File extension: `.lmf`  
Magic: `BARE_LMF`  
Endianness: Little-endian

---

## Sector Layout

```
LBA 0             File header (always present, always 512 bytes)
LBA 1             Extension header sector 0 (present if extra_sectors_count > 0)
LBA 1+N           Extension header sector N (spans extra_sectors_count sectors total)
LBA defined       Extension data sectors (defined by extension descriptor entries)
LBA defined       Entity data (LBA stored in the 0x0000 terminator entry, or LBA 1 if no extension header)
```

### Finding Entity Data

```
if extra_sectors_count == 0
    entity data is at LBA 1

if extra_sectors_count > 0
    read extra_sectors_count sectors starting at LBA 1
    parse 16-byte descriptors until type == 0x0000
    entity data is at the LBA stored in the 0x0000 entry
```

---

## LBA 0 — File Header (512 bytes)

| Bytes   | Field               | Type         | Description |
|---------|---------------------|--------------|-------------|
| 0–7     | magic               | `uint8[8]`   | Always `BARE_LMF` |
| 8–10    | version             | `uint8[3]`   | Format version, current `0.1` |
| 11–13   | entity_version      | `uint8[3]`   | Entity format version, current `1\0` |
| 14–15   | flags               | `uint16`     | Reserved for future use |
| 16      | extra_sectors_count | `uint8`      | Number of sectors the extension header spans. 0 = no extension header |
| 17–20   | ambient_light_int   | `float`      | Global ambient light intensity |
| 21–31   | reserved            | `uint8[11]`  | Zero, reserved for future use |
| 32–51   | map_id              | `uint8[20]`  | Unique map identifier, null-terminated ASCII |
| 52–63   | ambient_light_color | `float[3]`   | Global ambient light color |
| 64–87   | tables              | `table_t[3]` | Map types, usages, player configs (see below) |
| 88–95   | reserved            | `uint8[8]`   | Zero, padding |
| 96–415  | map_name            | `uint32[80]` | Map display name, UTF-32 encoded, null-terminated |
| 416–511 | reserved            | `uint8[96]`  | Zero, reserved for future use |

---

### table_t

Each table is a compact array of byte-sized entries:

```
uint8  table_type       // type of table
uint8  length           // number of entries, max 6 enforced in logic
uint8  entries[length]  // entry values
```

Max size per table: `2 + 6 = 8 bytes`

#### Table 1 — Map Types
Game modes this map supports. One byte per mode:

| Value  | Mode |
|--------|------|
| `0x00` | Deathmatch |
| `0x01` | Capture Point |
| `0xFF` | Debug |

#### Table 2 — Map Usage
Where the map can appear. One byte per usage type:

| Value  | Usage |
|--------|-------|
| `0x00` | Casual |
| `0x01` | Ranked |
| `0x02` | Custom |
| `0xFF` | Any |

#### Table 3 — Player Configurations
Valid player counts. One byte per configuration, split into nibbles:

```
bits 0–3  team 1 player count (0–15)
bits 4–7  team 2 player count (0–15)
```

| Value  | Configuration |
|--------|---------------|
| `0x00` | Any |
| `0x22` | 2v2 |
| `0x33` | 3v3 |
| `0x31` | 3v1 |

---

## Extension Header (LBA 1+)

Present only if `extra_sectors_count > 0` in LBA 0. Spans `extra_sectors_count` sectors starting at LBA 1.

The extension header is a table of 16-byte descriptors parsed sequentially. The `0x0000` entry is always last and serves as both the terminator and the pointer to entity data. Maximum 32 descriptors fit in a single 512-byte sector.

### Extension Descriptor (16 bytes)

```
uint16 type          // what kind of data this entry describes
uint64 LBA           // starting sector of the data
uint32 sector_count  // real count = sector_count + 1, range 1–4294967296 sectors
uint16 reserved      // zero
```

### Known Extension Types

| Type     | Description |
|----------|-------------|
| `0x0000` | Terminator + entity data pointer. LBA points to entity data. Always last entry. |
| `0x0001` | Extended map name (overflow for long UTF-32 names) |
| `0x0002` | String table (per-file entity name strings, UTF-32) |
| `0x0003` | Extended entity data (type-specific extra fields keyed by entity_id) |
| `0x0004` | Mesh Id table (per-file mash id strings, ASCII) |
| `0x0005` | Texture Id table (per-file texture id strings, ASCII) |

Unknown types are skipped by the parser using `(sector_count + 1) * 512` bytes from `LBA`.

### Example

```
extra_sectors_count = 2

LBA 1–2: extension header
  descriptor 0: type=0x0001  LBA=3   sector_count=4   → extended map name at LBA 3, 5 sectors
  descriptor 1: type=0x0002  LBA=8   sector_count=1   → string table at LBA 8, 2 sectors
  descriptor 2: type=0x0003  LBA=10  sector_count=8   → extended entity data at LBA 10, 9 sectors
  descriptor 3: type=0x0000  LBA=19  sector_count=0   → entity data at LBA 19 ← terminator

entity data starts at LBA 16
```

```
extra_sectors_count = 0

no extension header
entity data starts at LBA 1
```

---

## String Table (Extension Sector 0x0002)

Stores per-map strings referenced by `name_index` in entity optional fields.

### Layout

```
uint16 count                  // number of strings
uint32 offsets[count]         // byte offset from start of sector to each UTF-32 string
uint32 strings[]              // UTF-32 null-terminated strings packed sequentially
```

Lookup is O(1) — index into offset table, seek to string start.

This is the same for Mesh table (0x0004) and Texture table (0x0005).

---

## Version History

| Version | Description |
|---------|-------------|
| 0.1     | Initial version. Fixed 512-byte header, extension header support, UTF-32 map name, table_t for types/usage/players, entity version field |
