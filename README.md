## .:[ Join Our Discord For Support ]:.

<a href="https://discord.com/invite/U7AuQhu"><img src="https://discord.com/api/guilds/651838917687115806/widget.png?style=banner2"></a>

# [CS2] Anti-Block-GoldKingZ (1.0.3)

Anti-BodyBlock Client Side (Support HeadBoost + Vips Flags) + Anti-NadeBlock (Support Specific Nades/Team Bounce)

![antibodyblock](https://github.com/user-attachments/assets/85217774-b475-4b9f-a2b6-465dfc0abbeb)

<img width="600" height="340" alt="antinadeblock" src="https://github.com/user-attachments/assets/4890a28f-f554-4b94-8ad8-7a5f38ed1013" />


---

## 📦 Dependencies

[![Metamod:Source](https://img.shields.io/badge/Metamod:Source-2d2d2d?logo=sourceengine)](https://www.sourcemm.net)

[![CounterStrikeSharp](https://img.shields.io/badge/CounterStrikeSharp-83358F)](https://github.com/roflmuffin/CounterStrikeSharp)

[![MySQL](https://img.shields.io/badge/MySQL-4479A1?logo=mysql&logoColor=white)](https://dev.mysql.com/doc/connector-net/en/) [Included in zip]

[![LiteDB](https://img.shields.io/badge/LiteDB-512BD4?logo=dotnet&logoColor=white)](https://www.litedb.org/) [Included in zip]

[![JSON](https://img.shields.io/badge/JSON-000000?logo=json)](https://www.newtonsoft.com/json) [Included in zip]

---

## 📥 Installation

### Plugin Installation
1. Download the latest `Anti-Block-GoldKingZ.x.x.x.zip` release
2. Extract contents to your `csgo` directory
3. Configure settings in `Anti-Block-GoldKingZ/config/config.json`
4. Restart your server

---

## ⚙️ Configuration

> [!IMPORTANT]
> **Main Configuration**  
> `../Anti-Block-GoldKingZ/config/config.json`  

## 🛠️ `config/config.json`
<details open>
<summary><b>Reload Anti-BodyBlock Config</b> (Click to expand 🔽)</summary>

| Property | Description | Values | Required |
|----------|-------------|--------|----------|
| `Reload_AntiBlock_CommandsInGame` | Commands to reload plugin | `Console_Commands: css_reloadantiblock, css_reloadab`<br>`Chat_Commands:` | - |
| `Reload_AntiBlock_Flags` | Restricted flags for reload command | `SteamIDs: 76561198206086993, STEAM_0:1:507335558`<br>`Flags: @css/root, @css/admin`<br>`Groups: #css/root, #css/admin` | `Reload_AntiBlock_CommandsInGame` |
| `Reload_AntiBlock_Hide` | Hide chat after reload command | `0`-No<br>`1`-Only after success<br>`2`-Always hide | `Reload_AntiBlock_CommandsInGame` |
</details>

<details>
<summary><b>Anti-NadeBlock Config</b> (Click to expand 🔽)</summary>

| Property | Description | Values | Required |
|----------|-------------|--------|----------|
| `AntiNadeBlock_Enable` | Use Anti-NadeBlock filter | `0`-Disabled (Default CS2: pass teammates, bounce enemies)<br>`1`-All nades bounce teammates, pass enemies<br>`2`-All nades pass teammates and enemies<br>`3`-All nades bounce teammates and enemies<br>`4`-Use filter `AntiNadeBlock_To_Teammates` & `AntiNadeBlock_To_Enemies` | - |
| `AntiNadeBlock_To_Teammates` | Per-nade behavior when thrown at a teammate | Array of `<nade_name>:p` (pass) or `<nade_name>:b` (block/bounce)<br>Nade names: `hegrenade` `flashbang` `smokegrenade` `decoy` `molotov` `incendiary` | `AntiNadeBlock_Enable=4` |
| `AntiNadeBlock_To_Enemies` | Per-nade behavior when thrown at an enemy | Array of `<nade_name>:p` (pass) or `<nade_name>:b` (block/bounce)<br>Nade names: `hegrenade` `flashbang` `smokegrenade` `decoy` `molotov` `incendiary` | `AntiNadeBlock_Enable=4` |
</details>

<details>
<summary><b>Anti-BodyBlock Config</b> (Click to expand 🔽)</summary>

| Property | Description | Values | Required |
|----------|-------------|--------|----------|
| `AntiBodyBlock_DisableOnWarmUp` | Disable Anti-BodyBlock on WarmUp | `true`/`false` | - |
| `AntiBodyBlock_OnRoundStart` | Enable Anti-BodyBlock on round start | `true`/`false` | - |
| `AntiBodyBlock_OnRoundStartDuration` | Duration (seconds) to keep active on round start | `0`-Always active<br>`1`+ seconds | `AntiBodyBlock_OnRoundStart=true` |
</details>

<details>
<summary><b>Anti-BodyBlock Client Side Config</b> (Click to expand 🔽)</summary>

| Property | Description | Values | Required |
|----------|-------------|--------|----------|
| `AntiBodyBlock_Mode` | How client side command behaves | `0`-Disabled<br>`1`-Toggle mode<br>`2`-Timed mode | - |
| `AntiBodyBlock_CommandsInGame` | Commands to execute Anti-BodyBlock client side | `Console_Commands: css_noblock, css_nb`<br>`Chat_Commands:` | `AntiBodyBlock_Mode=1 or 2` |
| `AntiBodyBlock_Flags` | Restrict who can use commands | `SteamIDs: 76561198206086993, STEAM_0:1:507335558`<br>`Flags: @css/root, @css/admin`<br>`Groups: #css/root, #css/admin`<br>Empty = Allow everyone | `AntiBodyBlock_CommandsInGame` |
| `AntiBodyBlock_Hide` | Hide chat after execute command | `0`-No<br>`1`-Only after success<br>`2`-Always hide | `AntiBodyBlock_CommandsInGame` |
| `AntiBodyBlock_Teams` | Which teams can phase through each other | `1`-T only<br>`2`-CT only<br>`3`-Both separately<br>`4`-All players cross-team | `AntiBodyBlock_Mode=1 or 2` |
| `AntiBodyBlock_HeadBoost` | Allow standing on top of head | `true`/`false` | `AntiBodyBlock_Mode=1 or 2` |
| `AntiBodyBlock_Mode_1_Default` | Default value for new players | `true`-On<br>`false`-Off | `AntiBodyBlock_Mode=1` |
| `AntiBodyBlock_Mode_2_Duration` | How long (seconds) client side stays active | e.g. `10` | `AntiBodyBlock_Mode=2` |
| `AntiBodyBlock_Mode_2_Cooldown` | Cooldown (seconds) after using command | `0`-No cooldown<br>`1`+ seconds | `AntiBodyBlock_Mode=2` |
| `AntiBodyBlock_Mode_2_Cooldown_ImmunityFlags` | Who is immune from cooldown | `SteamIDs: 76561198206086993, STEAM_0:1:507335558`<br>`Flags: @css/root, @css/admin`<br>`Groups: #css/root, #css/admin`<br>Empty = Everyone immune | `AntiBodyBlock_Mode_2_Cooldown≠0` |
| `AntiBodyBlock_Mode_2_ResetCooldownOnNewRound` | Reset cooldown on new round start | `true`-Yes<br>`false`-On map end | `AntiBodyBlock_Mode_2_Cooldown≠0` |
| `AntiBodyBlock_Mode_2_MaxUsage` | Max times a player can use command | `0`-Unlimited<br>`1`+ times | `AntiBodyBlock_Mode=2` |
| `AntiBodyBlock_Mode_2_MaxUsage_ImmunityFlags` | Who is immune from max usage limit | `SteamIDs: 76561198206086993, STEAM_0:1:507335558`<br>`Flags: @css/root, @css/admin`<br>`Groups: #css/root, #css/admin`<br>Empty = Everyone immune | `AntiBodyBlock_Mode_2_MaxUsage≠0` |
| `AntiBodyBlock_Mode_2_ResetMaxUsageOnNewRound` | Reset max usage on new round start | `true`-Yes<br>`false`-On map end | `AntiBodyBlock_Mode_2_MaxUsage≠0` |
</details>

<details>
<summary><b>Locally Config</b> (Click to expand 🔽)</summary>

| Property | Description | Values | Required |
|----------|-------------|--------|----------|
| `Cookies_Enable` | Save player data locally by cookies | `0`-No<br>`1`-On disconnect (Warning Performance)<br>`2`-On map change (Recommended) | - |
| `Cookies_AutoRemovePlayerOlderThanXDays` | Auto delete inactive players (days) | `0`-Don't delete<br>`1`+ days | `Cookies_Enable=1 or 2` |
</details>

<details>
<summary><b>MySql Config</b> (Click to expand 🔽)</summary>

| Property | Description | Values | Required |
|----------|-------------|--------|----------|
| `MySql_Enable` | Save player data to MySQL | `0`-No<br>`1`-On disconnect (Warning Performance)<br>`2`-On map change (Recommended) | - |
| `MySql_ConnectionTimeout` | Connection timeout (seconds) | e.g. `30` | `MySql_Enable=1 or 2` |
| `MySql_RetryAttempts` | Retry attempts on failure | e.g. `3` | `MySql_Enable=1 or 2` |
| `MySql_RetryDelay` | Delay between retries (seconds) | e.g. `2` | `MySql_Enable=1 or 2` |
| `MySql_Servers` | MySQL server configurations | Array of server objects | `MySql_Enable=1 or 2` |
| `MySql_AutoRemovePlayerOlderThanXDays` | Auto delete inactive players (days) | `0`-Don't delete<br>`1`+ days | `MySql_Enable=1 or 2` |
</details>

<details>
<summary><b>Utilities Config</b> (Click to expand 🔽)</summary>

| Property | Description | Values | Required |
|----------|-------------|--------|----------|
| `AutoUpdateSignatures` | Auto update signatures in gamedata.json | `true`/`false` | - |
| `EnableDebug` | Enable debug plugin in server console | `true`/`false` | - |
</details>

## 📜 Changelog

<details>
<summary><b>📋 View Version History</b> (Click to expand 🔽)</summary>

### [1.0.3]
- Fix Cookies_Enable
- Fix MySql_Enable
- Fix Reload Plugin Overlap
- Rework On Custom GameData
- Added AntiNadeBlock_Enable 4 Modes
- Added AntiNadeBlock_To_Teammates
- Added AntiNadeBlock_To_Enemies  

### [1.0.2]
- Remove AntiBlockNades_IfThrowToTeamMates
- Remove AntiBlockNades_IfThrowToEnemyTeam
- Remove AntiBlockNades_TheseNades
- Added Reload_AntiBlock_CommandsInGame
- Added Reload_AntiBlock_Flags
- Added Reload_AntiBlock_Hide
- Added AntiBodyBlock_DisableOnWarmUp
- Added AntiBodyBlock_DisableOnWarmUp
- Added AntiBodyBlock_OnRoundStart
- Added AntiBodyBlock_OnRoundStartDuration
- Added AntiBodyBlock_Mode
- Added AntiBodyBlock_CommandsInGame
- Added AntiBodyBlock_Flags
- Added AntiBodyBlock_Hide
- Added AntiBodyBlock_Teams
- Added AntiBodyBlock_HeadBoost
- Added AntiBodyBlock_Mode_1_Default
- Added AntiBodyBlock_Mode_2_Duration
- Added AntiBodyBlock_Mode_2_Cooldown
- Added AntiBodyBlock_Mode_2_Cooldown_ImmunityFlags
- Added AntiBodyBlock_Mode_2_ResetCooldownOnNewRound
- Added AntiBodyBlock_Mode_2_MaxUsage
- Added AntiBodyBlock_Mode_2_MaxUsage_ImmunityFlags
- Added AntiBodyBlock_Mode_2_ResetMaxUsageOnNewRound
- Added Cookies_Enable
- Added Cookies_AutoRemovePlayerOlderThanXDays
- Added MySql_Enable
- Added MySql_ConnectionTimeout
- Added MySql_RetryAttempts
- Added MySql_RetryDelay
- Added MySql_AutoRemovePlayerOlderThanXDays
- Added AutoUpdateSignatures

### [1.0.1]
- Added AntiBodyBlock_OnStartRoundDurationXInSecs

### [1.0.0]
- Initial plugin release

</details>

---
