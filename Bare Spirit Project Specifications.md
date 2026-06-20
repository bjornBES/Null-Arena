# Bare Spirit Project Specifications

* **Specification v1**

> **Purpose:** Defines the structure, uses and definitions of the Bare Spirit project and its software.
> This serves as the single source of truth for all implementations,
> except if otherwise said by the author BjornBEs [Contact](#contact).

---

## 1 Definition

Here are the definitions of what Bare Spirit is, if any of these definitions are broken and or are not done with good faith in mind, then the project and its software is no longer Bare Spirit.

### 1.1 What is Bare Spirit not

- The Bare Spirit project and its software is not and will never be owned by any cooperation or other entities other than the author BjornBEs.
- The Bare Spirit project and its software is not and will never be a fully AI driven piece of project/software.
- The Bare Spirit project and its software is not and will never be fully closed source.
- The Bare Spirit project and its software is not and will never be fully vibe-coded.
- The Bare Spirit project and its software will never collect user data beyond what is strictly necessary for gameplay and server discovery.

### 1.2 What is Bare Spirit

- Bare Spirit is a project that values ownership — players and communities own their servers and their experience.
- Bare Spirit is a project that values transparency in its client software.
- Bare Spirit is a project that values privacy, keeping player IP and location data protected to the best of its ability.
- Bare Spirit is a project built with intent — every system has a reason for being the way it is.

---

## 2 Projects Under Bare Spirit

### 2.1 Null Arena

Null Arena is the first game developed and published under the Bare Spirit project.

> **Purpose:** Null Arena is a fast-paced small-scale hero shooter targeting LAN and self-hosted internet play.
> Players host their own game servers. There are no central game servers owned by Bare Spirit or the author.

#### 2.1.1 What is Null Arena not

- Null Arena is not and will never be a live-service game in the traditional sense — no battle passes, no paid content, no artificial engagement mechanics.
- Null Arena is not dependent on central servers owned by Bare Spirit to function — the game must remain playable as long as players can host their own servers.

#### 2.1.2 What is Null Arena

- Null Arena is a self-hostable multiplayer game supporting 1v1 to 3v3 arena play.
- Null Arena is a hero shooter with distinct character kits built around movement and positioning.
- Null Arena is a game where the server is authoritative — the server owns simulation, clients send intent only.

---

## 3 Architecture

### 3.1 Components

Null Arena consists of three distinct components:

- **Game Client** — The player-facing application. Open source.
- **Game Server** — Hosts a single match instance. Closed binary.
- **Master Server** — Handles server discovery. Closed binary.

### 3.2 Server Authority

The game server owns all simulation. Clients send raw input only — movement direction, aim angle, and button flags. Clients never send resolved position or velocity. This is a core invariant of Null Arena and must not be violated.

### 3.3 Port Assignments

| Component | Port Range | Protocol |
|---|---|---|
| Game Server instances | 10319 – 10574 | UDP |
| Master Server | 10575 | UDP |

The range 10319–10575 is contiguous by design for convenient router port-forwarding rules.

### 3.4 Open and Closed Source Split

| Component | Source Availability |
|---|---|
| Game Client | Open source |
| Game Server | Closed binary |
| Master Server | Closed binary |

The game server and master server are distributed as flat binaries to reduce the ease of extracting player IP addresses and location data from the master server protocol.

### 3.5 Security

- Master server traffic is encrypted to protect player IP and location data.
- The encryption scheme is defined by the author BjornBEs and is not specified further in this document.
- Closed server binaries serve as an additional obfuscation layer and are not a substitute for encryption.

### 3.6 Bare Spirit operated servers

Some servers will be operated by Bare Spirit,
these servers exist as a convenience entry point
for players who don't want to self-host or join community servers,
these servers will also operate under different rules and guidelines than community servers.
The number of operated servers scales with the amount of players playing Null Arena.

### 3.6.1 What Bare Spirit operated servers are not

- Bare Spirit operated servers are not **fully opened sourced software**, these servers will be closed Source.
- Bare Spirit operated servers are not **community-owned or self-hostable**, these servers will not be in the communities hand, server software will be released to the community if Bare Spirit ceases to operate them.
- Bare Spirit operated servers are not a **replacement for community servers**, these servers are here for players who don't want to self-host or join community servers. 
- Bare Spirit operated servers are not **required for Null Arena to function**, these servers will not be changing any game functions of Null Arena.

### 3.6.2 What Bare Spirit operated servers are

- Bare Spirit operated servers are **clearly labeled as operated servers**, in the server browser
- Bare Spirit operated servers are bound by a **defined data retention policy** [see more here](#363-what-data-is-bare-spirit-operated-servers-holding)
- Bare Spirit operated servers will shut down gracefully and will **delete all player data** if Bare Spirit ceases to operate them
- Bare Spirit operated servers will have **increased security** because of the data they hold.

### 3.6.3 What Data is Bare Spirit operated servers holding

Player data on Bare Spirit operated servers only stores relevant player data,
like player ID, Player display name and the Client IP these are called Session data.

#### Session data

Session data is only stored from player connection to player disconnection and will be stored in a proprietary piece of software.
the stored data is split into 3 categories which are Moderation functions, Gameplay functions and Server side functions,
each category holds different kinds of data

| Category | Stored data |
|---|---|
| Moderation functions | Player ID, Player display name, Client IP |
| Gameplay functions | Player ID, Client IP |
| Server side functions | Client IP |

#### Ban System

Bare Spirit operated servers maintain a tiered ban system, that are ranging from 1 day to 50 years,
all bans are issued manually by Bare Spirit and are variable in duration scaled to the offense,
all ban tiers can be appealed via contacting Bare Spirit with a written explanation about the offence reviewed by Bare Spirit.
Repeat offenses escalate the duration of the ban.

---

## 4 Vision & Goals

The goal of Bare Spirit and Null Arena is to ship a polished small-scale hero shooter that:

- Runs on self-hosted hardware including low-power devices such as a Raspberry Pi.
- Gives communities full ownership of their servers and game experience.
- Has no dependency on any infrastructure owned or operated by Bare Spirit to remain functional.
- Is fast, readable, and maintainable — no magic, no unnecessary abstraction.

---

## 5 Roadmap

### 5.1 Null Arena

- **Phase 0** — Single-player movement and core game loop functional.
- **Phase 1** — LAN multiplayer functional with at least two players.
- **Phase 2** — Master server and server discovery functional.
- **Phase 3** — First playable hero kit implemented.
- **Phase 4** — Full 1v1 match playable end to end.
- **Phase 5** — First public LAN release.
- **Phase 6** — Internet-facing play with encryption.
- **Phase 7** — Full hero roster for initial release.
- **Phase N** — Live service and community hosting.

> **Note:** A playable state is defined as a state where a full match can be played from lobby to end screen without manual intervention.

---

## Contact

Here are the only and real social accounts and contact methods to reach the author BjornBEs.

- [Github](link)

> This file will only be edited by the author BjornBEs. No one else will edit this file.