# BARE Level Map Format — Entity Specification

## Entity Version 1

---

## Overview

Entity data begins at the LBA stored in the `0x0000` terminator descriptor of the extension header, or at LBA 1 if `extra_sectors_count` is 0 in the file header.

Each entity record is **dynamically sized** but always aligned to the nearest **16-byte boundary**. Valid entity sizes are 16, 32, 48, 64 bytes. The maximum entity size is **0x40 (64) bytes**.

Type-specific extra data is never stored inline in the entity record. It lives in the extended entity data extension sector (`0x0003`), keyed by `entity_id`.

---

## Fixed Fields (bytes 0–45, 46 bytes)

| Bytes | Field       | Type       | Description |
|-------|-------------|------------|-------------|
| 0–3   | entity_id   | `uint32`   | Unique entity identifier within the map |
| 4     | entity_type | `uint8`    | Entity type (see Entity Types below) |
| 5     | flags       | `uint8`    | Controls which optional fields are present |
| 6–7   | reserved    | `uint16`   | Zero, alignment padding |
| 8–19  | position    | `float[3]` | World position X, Y, Z |
| 20–31 | rotation    | `float[3]` | Rotation in degrees X, Y, Z |
| 32–43 | scale       | `float[3]` | Per-axis scale X, Y, Z |
| 44–45 | texture_id  | `uint16`   | Asset registry handle. `0xFFFF` = none |

---

## Optional Fields (bytes 46–63, up to 18 bytes)

Optional fields are controlled by the flags byte at byte 5. Fields are written and read in bit order 0→7. Each field is only present if its corresponding flag bit is set. Field sizes are fixed per bit and known at parse time from the entity version.

### Flags (entity version 1)

| Bit | Field      | Type     | Size    | Description |
|-----|------------|----------|---------|-------------|
| 0   | name_index | `uint16` | 2 bytes | Index into the per-file string table (extension sector `0x0002`) |
| 1   | team       | `uint8`  | 1 byte  | Team assignment |
| 2–7 | —          | —        | —       | Reserved, TBA |

### Team Values

| Value  | Description |
|--------|-------------|
| `0x00` | No team |
| `0x01` | Team 1 |
| `0x02` | Team 2 |
| `0xFF` | Reserved |

---

## Alignment

Fixed fields occupy 46 bytes. Optional fields follow from byte 46. The total entity size is padded with zero bytes to the nearest 16-byte boundary:

| Optional fields present | Raw size | Aligned size |
|-------------------------|----------|--------------|
| None                    | 46 bytes | 48 bytes     |
| Name only               | 48 bytes | 48 bytes     |
| Team only               | 47 bytes | 48 bytes     |
| Name + team             | 49 bytes | 64 bytes     |

---

## Entity Types

| Value  | Type           | Server loads? | Description |
|--------|----------------|---------------|-------------|
| `0x00` | `BOX`          | Yes           | Static collidable box (wall, floor, crate, platform) |
| `0x01` | `PLAYER_SPAWN` | Yes           | Player spawn point |
| `0x02` | `ITEM_SPAWN`   | Yes           | Item spawn point |
| `0x03` | `BILLBOARD`    | No            | Static world-space sprite (torch, sign, decal) |
| `0x04` | `LIGHT`        | No            | Light source |
| `0xFF` | Reserved       | —             | —           |

---

## Extended Entity Data (Extension Sector 0x0003)

Type-specific extra data lives in the `0x0003` extension sector and is joined to entity records by `entity_id` after parsing.

### Sector Layout

```
uint32      count               // number of data_entry records
uint32      offsets[count]
data_entry  entries[]
```

### data_entry

```
uint32  entity_id               // matches entity_id in the entity record
uint8   entity_data_length      // byte length of data that follows
uint8   data[entity_data_length]
```

`entity_data_length` is `uint8` giving a maximum of 255 bytes of extra data per entity. If a future entity type requires more, a new extension type will be defined.

Unknown entries are skipped forward-compatibly using `entity_data_length`.

### Extra Data by Entity Type (version 1)

#### `ITEM_SPAWN` (0x02)

| Offset | Field     | Type    | Description |
|--------|-----------|---------|-------------|
| 0      | item_type | `uint8` | Item to spawn |

##### Item Types

| Value  | Item |
|--------|------|
| `0x00` | Health |
| `0x01` | Ammo |
| `0x02` | Weapon |

#### `LIGHT` (0x04)

| Offset | Field     | Type    | Description |
|--------|-----------|---------|-------------|
| 0–3    | intensity | `float` | Light intensity |
| 4–7    | radius    | `float` | Light radius in world units |

---

## Asset Registry

`texture_id` is a `uint16` handle into a global asset registry external to the map file. The asset registry is managed by the asset system and exists outside the LMF format. `0xFFFF` indicates no texture.

---

## Parser Logic

```
// 1. find entity data LBA
if extra_sectors_count == 0
    entity_lba = 1
else
    read extension header
    parse descriptors until type == 0x0000
    entity_lba = 0x0000 entry LBA
    cache LBA of 0x0002 (string table) and 0x0003 (extended entity data) if present

// 2. parse entity records
seek to entity_lba * 512
while not end of entity data
    read 46 bytes fixed fields
    if flag bit 0 set → read uint16 name_index
    if flag bit 1 set → read uint8 team
    compute total size = 46 + optional_size
    align up to nearest 16 bytes
    seek to next entity

// 3. join extended entity data
if 0x0003 present
    seek to its LBA
    read count
    for each data_entry
        match entity_id to parsed entity
        parse data by entity_type
```

---

## Version History

| Version | Description |
|---------|-------------|
| 1       | Initial version. Fixed 46-byte base, flags-controlled optional fields (name, team), 16-byte alignment, max 64 bytes per entity, type-specific extra data in extension sector 0x0003 |
