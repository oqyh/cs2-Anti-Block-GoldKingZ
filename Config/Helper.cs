using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Drawing;
using System.Security.Cryptography;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API.Modules.Utils;
using CounterStrikeSharp.API.Modules.Cvars;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Core.Translations;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Anti_Block_GoldKingZ.Config;
using System.Globalization;
using CounterStrikeSharp.API.Modules.Entities;
using System.Runtime.InteropServices;
using System.Numerics;
using CounterStrikeSharp.API.Modules.Entities.Constants;

namespace Anti_Block_GoldKingZ;

public class Helper
{
    private static readonly Dictionary<string, string> Nades_Maps = new(StringComparer.OrdinalIgnoreCase)
    {
        { "hegrenade",    "hegrenade"    },
        { "he",           "hegrenade"    },
        { "flashbang",    "flashbang"    },
        { "flash",        "flashbang"    },
        { "smokegrenade", "smokegrenade" },
        { "smoke",        "smokegrenade" },
        { "decoy",        "decoy"        },
        { "molotov",      "molotov"      },
        { "incendiary",   "molotov"   },
        { "inc",          "molotov"   }
    };

    public static Dictionary<string, bool> NadesParse(List<string> entries)
    {
        var result = new Dictionary<string, bool>(StringComparer.OrdinalIgnoreCase);
        if (entries == null) return result;

        foreach (var raw in entries)
        {
            if (string.IsNullOrWhiteSpace(raw)) continue;

            var parts = raw.Split(':', 2, StringSplitOptions.TrimEntries);
            if (parts.Length != 2) continue;

            string nameRaw = parts[0].Trim().ToLowerInvariant();
            string action  = parts[1].Trim().ToLowerInvariant();

            if (!Nades_Maps.TryGetValue(nameRaw, out var canonical)) continue;

            bool bounce = action switch
            {
                "b" or "block" or "bounce"                   => true,
                "p" or "pass"  or "passthrough" or "through" => false,
                _ => false
            };

            result[canonical] = bounce;
        }
        return result;
    }

    public static string? DesignerToCanonical(string? designerName)
    {
        if (string.IsNullOrEmpty(designerName)) return null;
        string stripped = designerName.Replace("_projectile", "").ToLowerInvariant();
        return Nades_Maps.TryGetValue(stripped, out var canonical) ? canonical : null;
    }
    
    public static void RegisterCssCommands(string[]? commands, string description, CommandInfo.CommandCallback callback)
    {
        if (commands == null || commands.Length == 0) return;

        foreach (var cmd in commands)
        {
            if (string.IsNullOrEmpty(cmd)) continue;
            MainPlugin.Instance.AddCommand(cmd, description, callback);
        }
    }


    public static void RemoveCssCommands(string[]? commands, CommandInfo.CommandCallback callback)
    {
        if (commands == null || commands.Length == 0) return;

        foreach (var cmd in commands)
        {
            if (string.IsNullOrEmpty(cmd)) continue;
            MainPlugin.Instance.RemoveCommand(cmd, callback);
        }
    }

    public static void AdvancedPlayerPrintToChat(CCSPlayerController player, CounterStrikeSharp.API.Modules.Commands.CommandInfo commandInfo, string message, params object[] args)
    {
        if (string.IsNullOrEmpty(message)) return;

        for (int i = 0; i < args.Length; i++)
        {
            message = message.Replace($"{{{i}}}", args[i]?.ToString() ?? "");
        }

        if (Regex.IsMatch(message, "{nextline}", RegexOptions.IgnoreCase))
        {
            string[] parts = Regex.Split(message, "{nextline}", RegexOptions.IgnoreCase);
            foreach (string part in parts)
            {
                string trimmedPart = part.Trim();
                trimmedPart = trimmedPart.ReplaceColorTags();
                if (!string.IsNullOrEmpty(trimmedPart))
                {
                    if (commandInfo != null && commandInfo.CallingContext == CounterStrikeSharp.API.Modules.Commands.CommandCallingContext.Console)
                    {
                        player.PrintToConsole(" " + trimmedPart);
                    }
                    else
                    {
                        player.PrintToChat(" " + trimmedPart);
                    }
                }
            }
        }
        else
        {
            message = message.ReplaceColorTags();
            if (commandInfo != null && commandInfo.CallingContext == CounterStrikeSharp.API.Modules.Commands.CommandCallingContext.Console)
            {
                player.PrintToConsole(message);
            }
            else
            {
                player.PrintToChat(message);
            }
        }
    }
    public static void AdvancedPlayerPrintToConsole(CCSPlayerController player, string message, params object[] args)
    {
        if (string.IsNullOrEmpty(message)) return;

        for (int i = 0; i < args.Length; i++)
        {
            message = message.Replace($"{{{i}}}", args[i].ToString());
        }
        if (Regex.IsMatch(message, "{nextline}", RegexOptions.IgnoreCase))
        {
            string[] parts = Regex.Split(message, "{nextline}", RegexOptions.IgnoreCase);
            foreach (string part in parts)
            {
                string trimmedPart = part.Trim();
                trimmedPart = trimmedPart.ReplaceColorTags();
                if (!string.IsNullOrEmpty(trimmedPart))
                {
                    player.PrintToConsole(" " + trimmedPart);
                }
            }
        }
        else
        {
            message = message.ReplaceColorTags();
            player.PrintToConsole(message);
        }
    }
    public static void AdvancedServerPrintToChatAll(string message, params object[] args)
    {
        if (string.IsNullOrEmpty(message)) return;

        for (int i = 0; i < args.Length; i++)
        {
            message = message.Replace($"{{{i}}}", args[i].ToString());
        }
        if (Regex.IsMatch(message, "{nextline}", RegexOptions.IgnoreCase))
        {
            string[] parts = Regex.Split(message, "{nextline}", RegexOptions.IgnoreCase);
            foreach (string part in parts)
            {
                string trimmedPart = part.Trim();
                trimmedPart = trimmedPart.ReplaceColorTags();
                if (!string.IsNullOrEmpty(trimmedPart))
                {
                    Server.PrintToChatAll(" " + trimmedPart);
                }
            }
        }
        else
        {
            message = message.ReplaceColorTags();
            Server.PrintToChatAll(message);
        }
    }
    
    public static bool IsPlayerInGroupPermission(CCSPlayerController player, string groups)
    {
        if (string.IsNullOrEmpty(groups) || player == null || !player.IsValid)
            return false;

        return groups.Split('|')
            .Select(segment => segment.Trim())
            .Any(trimmedSegment => Permission_CheckPermissionSegment(player, trimmedSegment));
    }

    private static bool Permission_CheckPermissionSegment(CCSPlayerController player, string segment)
    {
        if (string.IsNullOrEmpty(segment)) return false;

        int colonIndex = segment.IndexOf(':');
        if (colonIndex == -1 || colonIndex == 0) return false;

        string prefix = segment.Substring(0, colonIndex).Trim().ToLower();
        string values = segment.Substring(colonIndex + 1).Trim();

        return prefix switch
        {
            "steamid" or "steamids" or "steam" or "steams" => Permission_CheckSteamIds(player, values),
            "flag" or "flags" => Permission_CheckFlags(player, values),
            "group" or "groups" => Permission_CheckGroups(player, values),
            _ => false
        };
    }

    private static bool Permission_CheckSteamIds(CCSPlayerController player, string steamIds)
    {
        if (string.IsNullOrEmpty(steamIds)) return false;

        steamIds = steamIds.Replace("[", "").Replace("]", "");

        var (steam2, steam3, steam32, steam64) = player.SteamID.GetPlayerSteamID();
        var steam3NoBrackets = steam3.Trim('[', ']');

        return steamIds
            .Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(id => id.Trim())
            .Any(trimmedId =>
                string.Equals(trimmedId, steam2, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(trimmedId, steam3NoBrackets, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(trimmedId, steam32, StringComparison.OrdinalIgnoreCase) ||
                string.Equals(trimmedId, steam64, StringComparison.OrdinalIgnoreCase)
            );
    }

    private static bool Permission_CheckFlags(CCSPlayerController player, string flags)
    {
        if (player == null || !player.IsValid ||
            player.Connected != PlayerConnectedState.Connected ||
            player.IsBot || player.IsHLTV)
            return false;

        if (string.IsNullOrEmpty(flags))
            return false;

        var playerData = AdminManager.GetPlayerAdminData(player);
        if (playerData == null)
            return false;

        var requiredFlags = flags
            .Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(f => f.Trim())
            .ToList();

        if (playerData._flags != null &&
            requiredFlags.Any(reqFlag =>
                playerData._flags.Contains(reqFlag, StringComparer.OrdinalIgnoreCase)))
            return true;

        var allFlags = playerData.GetAllFlags();
        return allFlags != null &&
            requiredFlags.Any(reqFlag =>
                allFlags.Contains(reqFlag, StringComparer.OrdinalIgnoreCase));
    }

    private static bool Permission_CheckGroups(CCSPlayerController player, string groups)
    {
        if (player == null || !player.IsValid ||
            player.Connected != PlayerConnectedState.Connected ||
            player.IsBot || player.IsHLTV)
            return false;

        if (string.IsNullOrEmpty(groups))
            return false;

        var playerData = AdminManager.GetPlayerAdminData(player);
        if (playerData == null || playerData.Groups == null || !playerData.Groups.Any())
            return false;

        return groups
            .Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(g => g.Trim())
            .Any(reqGroup => playerData.Groups.Contains(reqGroup, StringComparer.OrdinalIgnoreCase));
    }

    public static List<CCSPlayerController> GetPlayersController(bool IncludeBots = false, bool IncludeHLTV = false, bool IncludeNone = true, bool IncludeSPEC = true, bool IncludeCT = true, bool IncludeT = true)
    {
        return Utilities
            .FindAllEntitiesByDesignerName<CCSPlayerController>("cs_player_controller")
            .Where(p =>
                p != null &&
                p.IsValid &&
                p.Connected == PlayerConnectedState.Connected &&
                (IncludeBots || !p.IsBot) &&
                (IncludeHLTV || !p.IsHLTV) &&
                ((IncludeCT && p.TeamNum == (byte)CsTeam.CounterTerrorist) ||
                (IncludeT && p.TeamNum == (byte)CsTeam.Terrorist) ||
                (IncludeNone && p.TeamNum == (byte)CsTeam.None) ||
                (IncludeSPEC && p.TeamNum == (byte)CsTeam.Spectator)))
            .ToList();
    }
    public static int GetPlayersCount(bool IncludeBots = false, bool IncludeHLTV = false, bool IncludeSPEC = true, bool IncludeCT = true, bool IncludeT = true)
    {
        return Utilities.GetPlayers().Count(p =>
            p != null &&
            p.IsValid &&
            p.Connected == PlayerConnectedState.Connected &&
            (IncludeBots || !p.IsBot) &&
            (IncludeHLTV || !p.IsHLTV) &&
            ((IncludeCT && p.TeamNum == (byte)CsTeam.CounterTerrorist) ||
            (IncludeT && p.TeamNum == (byte)CsTeam.Terrorist) ||
            (IncludeSPEC && p.TeamNum == (byte)CsTeam.Spectator))
        );
    }
    
    public static void ClearVariables()
    {
        var g_Main = MainPlugin.Instance.g_Main;

        KillAntiBodyBlockTimer_All();
        g_Main.Clear();
        
    }
    private static CCSGameRules? GetGameRules()
    {
        try
        {
            var gameRulesEntities = Utilities.FindAllEntitiesByDesignerName<CCSGameRulesProxy>("cs_gamerules");
            return gameRulesEntities.First().GameRules;
        }
        catch (Exception ex)
        {
            DebugMessage(ex.Message);
            return null;
        }
    }
    public static bool IsWarmup()
    {
        return GetGameRules()?.WarmupPeriod ?? false;
    }
	
    public static void DebugMessage(string message, bool important = false)
    {
        if (!Configs.Instance.EnableDebug && !important) return;
        var color = important ? Con.Red : Con.Magenta;
        string output = $"[Anti Block]: {message}";
        Con.WriteLine(color + output);
    }

    public static bool ArePlayersOverlapping(CCSPlayerController controller1, CCSPlayerController controller2)
    {
        var pawn1 = controller1?.PlayerPawn?.Value;
        var pawn2 = controller2?.PlayerPawn?.Value;
        if (pawn1 == null || pawn2 == null) return false;

        var pos1 = pawn1.AbsOrigin;
        var pos2 = pawn2.AbsOrigin;
        var col1 = pawn1.Collision;
        var col2 = pawn2.Collision;
        if (pos1 == null || pos2 == null || col1 == null || col2 == null) return false;

        float minX1 = pos1.X + col1.Mins.X;
        float maxX1 = pos1.X + col1.Maxs.X;
        float minY1 = pos1.Y + col1.Mins.Y;
        float maxY1 = pos1.Y + col1.Maxs.Y;
        float minZ1 = pos1.Z + col1.Mins.Z;
        float maxZ1 = pos1.Z + col1.Maxs.Z;

        float minX2 = pos2.X + col2.Mins.X;
        float maxX2 = pos2.X + col2.Maxs.X;
        float minY2 = pos2.Y + col2.Mins.Y;
        float maxY2 = pos2.Y + col2.Maxs.Y;
        float minZ2 = pos2.Z + col2.Mins.Z;
        float maxZ2 = pos2.Z + col2.Maxs.Z;

        return minX1 < maxX2 && maxX1 > minX2 &&
            minY1 < maxY2 && maxY1 > minY2 &&
            minZ1 < maxZ2 && maxZ1 > minZ2;
    }

    public static void MuteCommands(CounterStrikeSharp.API.Modules.UserMessages.UserMessage? um, int Config, bool Fully = false)
    {
        if (um == null) return;
        if (!Fully && Config == 2 || Fully && Config > 0)
        {
            um.Recipients.Clear();
        }
    }



    public static void CheckPlayerInGlobals(CCSPlayerController player)
    {
        if (!player.IsValid(true)) return;

        var g_Main = MainPlugin.Instance.g_Main;
        if (!g_Main.Player_Data.ContainsKey(player.Slot))
        {
            var initialData = new Globals.PlayerDataClass(
                player,
                player.SteamID,
                null,
                0,
                DateTime.MinValue,
                DateTime.MinValue,
                DateTime.MinValue
            );
            g_Main.Player_Data.TryAdd(player.Slot, initialData);
        }else
        {
            g_Main.Player_Data[player.Slot].Player = player;
        }
    }


    
    public static void ResetAntiBodyBlock()
    {
        var cfg = Configs.Instance.AntiBodyBlock;
        foreach (var playerData in MainPlugin.Instance.g_Main.Player_Data.Values)
        {
            if (cfg.AntiBodyBlock_Mode_2_ResetMaxUsageOnNewRound)
            {
                playerData.NoBlock_Used = 0;
            }

            if (cfg.AntiBodyBlock_Mode_2_ResetCooldownOnNewRound)
            {
                playerData.Cooldown = DateTime.MinValue;
            }
        }
    }

    

    public static void KillAntiBodyBlockTimer_All()
    {
        foreach (var playerData in MainPlugin.Instance.g_Main.Player_Data.Values)
        {
            playerData.Timer_NoBlock?.Kill();
            playerData.Timer_NoBlock = null;
        }
    }

    public static void StartAntiBlock_All(bool StartAntiBlock = false)
    {
        foreach(var players in GetPlayersController(true, false, false, false))
        {
            if(!players.IsValid(true) || !players.IsAlive())continue;

            if(StartAntiBlock)
            {
                players.PlayerPawn.Value.SetCollisionGroup(CollisionGroup.COLLISION_GROUP_DEBRIS);
            }else
            {
                players.PlayerPawn.Value.SetCollisionGroup(CollisionGroup.COLLISION_GROUP_PLAYER);
            }
        }
    }

    public static void KillAntiBodyBlockTimer(CCSPlayerController player)
    {
        if (!player.IsValid(true)
        || !MainPlugin.Instance.g_Main.Player_Data.TryGetValue(player.Slot, out var playerData)) return;

        playerData.Timer_NoBlock?.Kill();
        playerData.Timer_NoBlock = null;
    }

    public static void StartAntiBlock(CCSPlayerController player)
    {
        if(!player.IsValid(true) || !player.IsAlive())return;

        player.PlayerPawn.Value.SetCollisionGroup(CollisionGroup.COLLISION_GROUP_DEBRIS);
    }

    public static void ChangeConvar()
    {
        if(Configs.Instance.AntiBodyBlock.AntiBodyBlock_Mode == 0)return;

        if(Configs.Instance.UseOnConVarChangedHook)
        {
            MainPlugin.Instance.g_Main.HookConVars.Clear();
        }

        Server.ExecuteCommand("mp_solid_enemies 1");
        if(Configs.Instance.UseOnConVarChangedHook)
        {
            MainPlugin.Instance.g_Main.HookConVars["mp_solid_enemies"] = "1";
        }
        

        Server.ExecuteCommand("mp_solid_teammates 1");
        if(Configs.Instance.UseOnConVarChangedHook)
        {
            MainPlugin.Instance.g_Main.HookConVars["mp_solid_teammates"] = "1";
        }
    }
    

    public static void ReloadPlayersGlobals()
    {
        foreach (var players in GetPlayersController(false, false, false))
        {
            if (!players.IsValid(true)) continue;
            CheckPlayerInGlobals(players);
        }
    }

    public static void RebuildNadeBounceMaps()
    {
        if(Configs.Instance.AntiNadeBlock_Enable == 0)return;
        
        MainPlugin.Instance.g_Main.NadeBounce_Teammates?.Clear();
        MainPlugin.Instance.g_Main.NadeBounce_Enemies?.Clear();

        MainPlugin.Instance.g_Main.NadeBounce_Teammates = NadesParse(Configs.Instance.AntiNadeBlock_To_Teammates);
        MainPlugin.Instance.g_Main.NadeBounce_Enemies   = NadesParse(Configs.Instance.AntiNadeBlock_To_Enemies);
    }

    public static void DownloadMissingFiles()
    {
        if(MainPlugin.Instance.g_Main.Downloading_FromGithub)return;

        MainPlugin.Instance.g_Main.Downloading_FromGithub = true;

        _ = Task.Run(async () =>
        {
            try
            {
                await DownloadMissingFilesAsync();
            }
            finally
            {
                MainPlugin.Instance.g_Main.Downloading_FromGithub = false;
            }
        });
    }
    public static async Task DownloadMissingFilesAsync()
    {
        try
        {
            await Start_DownloadMissingFiles();
            await Server.NextFrameAsync(CustomGameData.Load);
        }
        catch (Exception ex)
        {
            DebugMessage($"DownloadMissingFilesAsync failed: {ex.Message}");
        }
    }
    public static async Task Start_DownloadMissingFiles()
    {
        try
        {
            string localPath_gamedata = Path.Combine(MainPlugin.Instance.ModuleDirectory, "gamedata/gamedata.json");
            string githubUrl_gamedata = "https://raw.githubusercontent.com/oqyh/cs2-Private-Plugins/main/Resources/gamedata.json";
            await DownloadFromGitHub(localPath_gamedata, githubUrl_gamedata, Configs.Instance.AutoUpdateSignatures);
            
        }
        catch (Exception ex)
        {
            DebugMessage($"DownloadMissingFiles Error: {ex.Message}");
        }
    }

    private static readonly HttpClient _httpClient_Github = new HttpClient
    {
        Timeout = TimeSpan.FromSeconds(50)
    };
    private static readonly TimeSpan _timeout_Github = TimeSpan.FromSeconds(50);
    public static async Task DownloadFromGitHub(string filePath, string githubUrl, bool AutoUpdate = false)
    {
        try
        {
            string fullPath = Path.Combine(MainPlugin.Instance.ModuleDirectory, filePath);

            string? dir = Path.GetDirectoryName(fullPath);
            if (!string.IsNullOrEmpty(dir))
            {
                Directory.CreateDirectory(dir);
            }

            _httpClient_Github.DefaultRequestHeaders.Remove("User-Agent");
            _httpClient_Github.DefaultRequestHeaders.Add("User-Agent", "CS2-Plugin-AntiBlock");

            string actualDownloadUrl = githubUrl;

            if (githubUrl.EndsWith(".txt", StringComparison.OrdinalIgnoreCase))
            {
                using var ctsTxt = new CancellationTokenSource(_timeout_Github);
                var txtResponse = await _httpClient_Github.GetAsync(githubUrl, ctsTxt.Token);
                txtResponse.EnsureSuccessStatusCode();
                actualDownloadUrl = (await txtResponse.Content.ReadAsStringAsync()).Trim();
            }

            using var ctsBytes = new CancellationTokenSource(_timeout_Github);
            var bytesResponse = await _httpClient_Github.GetAsync(actualDownloadUrl, ctsBytes.Token);
            bytesResponse.EnsureSuccessStatusCode();
            byte[] remoteBytes = await bytesResponse.Content.ReadAsByteArrayAsync();

            bool needDownload = !File.Exists(fullPath);

            if (!needDownload && AutoUpdate)
            {
                using var sha256 = SHA256.Create();
                string Hash(byte[] b) => BitConverter.ToString(sha256.ComputeHash(b)).Replace("-", "").ToLowerInvariant();

                byte[] localBytes = await File.ReadAllBytesAsync(fullPath);
                needDownload = Hash(localBytes) != Hash(remoteBytes);
            }

            if (needDownload)
            {
                await File.WriteAllBytesAsync(fullPath, remoteBytes);
            }
        }
        catch (Exception ex)
        {
            DebugMessage($"DownloadFromGitHub Error: {ex.Message}");
        }
    }

    public static void RegisterCommandsAndHooks()
    {
        Server.ExecuteCommand("sv_hibernate_when_empty false");

        ChangeConvar();

        MainPlugin.Instance.RegisterListener<Listeners.OnMapStart>(MainPlugin.Instance.OnMapStart);
        MainPlugin.Instance.RegisterListener<Listeners.OnMapEnd>(MainPlugin.Instance.OnMapEnd);

        MainPlugin.Instance.RegisterEventHandler<EventRoundStart>(MainPlugin.Instance.OnEventRoundStart);
        MainPlugin.Instance.RegisterEventHandler<EventPlayerConnectFull>(MainPlugin.Instance.OnEventPlayerConnectFull);
        MainPlugin.Instance.RegisterEventHandler<EventRoundEnd>(MainPlugin.Instance.OnEventRoundEnd);
        MainPlugin.Instance.RegisterEventHandler<EventPlayerSpawn>(MainPlugin.Instance.OnEventPlayerSpawn);
        MainPlugin.Instance.RegisterEventHandler<EventPlayerDeath>(MainPlugin.Instance.OnEventPlayerDeath);
        MainPlugin.Instance.RegisterEventHandler<EventPlayerDisconnect>(MainPlugin.Instance.OnEventPlayerDisconnect);

        MainPlugin.Instance.AddCommandListener("say", MainPlugin.Instance.OnPlayerSay, HookMode.Post);
        MainPlugin.Instance.AddCommandListener("say_team", MainPlugin.Instance.OnPlayerSay_Team, HookMode.Post);
        MainPlugin.Instance.HookUserMessage(118, MainPlugin.Instance.OnUserMessage_OnSayText2, HookMode.Pre);

        RegisterCssCommands(Configs.Instance.Reload_AntiBlock.Reload_AntiBlock_CommandsInGame.ConvertCommands(), "Commands To Reload Anti Block Plugin", MainPlugin.Instance.Game_UserMessages.CommandsAction_ReloadPlugin);
        if(Configs.Instance.AntiBodyBlock.AntiBodyBlock_Mode != 0)
        {
            RegisterCssCommands(Configs.Instance.AntiBodyBlock.AntiBodyBlock_CommandsInGame.ConvertCommands(), "Commands To Anti Block Client Side", MainPlugin.Instance.Game_UserMessages.CommandsAction_AntiBodyBlock);
        }

        RebuildNadeBounceMaps();
    }

    public static void RemoveRegisterCommandsAndHooks()
    {
        MainPlugin.Instance.RemoveListener<Listeners.OnMapStart>(MainPlugin.Instance.OnMapStart);
        MainPlugin.Instance.RemoveListener<Listeners.OnMapEnd>(MainPlugin.Instance.OnMapEnd);

        MainPlugin.Instance.DeregisterEventHandler<EventRoundStart>(MainPlugin.Instance.OnEventRoundStart);
        MainPlugin.Instance.DeregisterEventHandler<EventPlayerConnectFull>(MainPlugin.Instance.OnEventPlayerConnectFull);
        MainPlugin.Instance.DeregisterEventHandler<EventRoundEnd>(MainPlugin.Instance.OnEventRoundEnd);
        MainPlugin.Instance.DeregisterEventHandler<EventPlayerSpawn>(MainPlugin.Instance.OnEventPlayerSpawn);
        MainPlugin.Instance.DeregisterEventHandler<EventPlayerDeath>(MainPlugin.Instance.OnEventPlayerDeath);
        MainPlugin.Instance.DeregisterEventHandler<EventPlayerDisconnect>(MainPlugin.Instance.OnEventPlayerDisconnect);

        MainPlugin.Instance.RemoveCommandListener("say", MainPlugin.Instance.OnPlayerSay, HookMode.Post);
        MainPlugin.Instance.RemoveCommandListener("say_team", MainPlugin.Instance.OnPlayerSay_Team, HookMode.Post);
        MainPlugin.Instance.UnhookUserMessage(118, MainPlugin.Instance.OnUserMessage_OnSayText2, HookMode.Pre);

        
        RemoveCssCommands(Configs.Instance.Reload_AntiBlock.Reload_AntiBlock_CommandsInGame.ConvertCommands(), MainPlugin.Instance.Game_UserMessages.CommandsAction_ReloadPlugin);
        RemoveCssCommands(Configs.Instance.AntiBodyBlock.AntiBodyBlock_CommandsInGame.ConvertCommands(), MainPlugin.Instance.Game_UserMessages.CommandsAction_AntiBodyBlock);

        CustomGameData.Unload();
    }
}