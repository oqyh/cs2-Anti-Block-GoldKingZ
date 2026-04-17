using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using Microsoft.Extensions.Localization;
using CounterStrikeSharp.API.Core.Attributes;
using CounterStrikeSharp.API.Modules.Cvars;
using CounterStrikeSharp.API.Modules.Timers;
using Anti_Block_GoldKingZ.Config;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API.Core.Attributes.Registration;
using CounterStrikeSharp.API.Modules.Commands;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using CounterStrikeSharp.API.Modules.Entities.Constants;
using CounterStrikeSharp.API.Modules.Utils;
using System.Numerics;
using System.Text;
using System.Runtime.InteropServices;
using CounterStrikeSharp.API.Modules.Entities;
using CounterStrikeSharp.API.Modules.Memory;
using CounterStrikeSharp.API.Modules.Memory.DynamicFunctions;

namespace Anti_Block_GoldKingZ;

public class MainPlugin : BasePlugin
{
    public override string ModuleName => "Anti-BodyBlock Client Side (Support HeadBoost + Vips Flags) + Anti-NadeBlock (Support Specific Nades/Team Bounce)"; 
    public override string ModuleVersion => "1.0.3";
    public override string ModuleAuthor => "Gold KingZ";
    public override string ModuleDescription => "https://github.com/oqyh";
	public static MainPlugin Instance { get; set; } = new();
    public readonly Game_UserMessages Game_UserMessages = new();
    public Globals g_Main = new();
    public override void Load(bool hotReload)
    {
		Instance = this;
        Configs.Load(ModuleDirectory);

        Helper.RemoveRegisterCommandsAndHooks();
        Helper.DownloadMissingFiles();
        
        Helper.RegisterCommandsAndHooks();

        if (hotReload)
        {
            Helper.ClearVariables(true);

            Helper.RemoveRegisterCommandsAndHooks();
            Helper.ReloadPlayersGlobals();
            Helper.DownloadMissingFiles();

            Helper.RegisterCommandsAndHooks();
        }
    }
    public void OnMapStart(string Map)
    {
        Helper.DownloadMissingFiles();
    }

    public HookResult OnEventPlayerConnectFull(EventPlayerConnectFull @event, GameEventInfo info)
    {
        if (@event == null) return HookResult.Continue;
        
        var player = @event.Userid;
        if (!player.IsValid(true)) return HookResult.Continue;
        
        _ = HandlePlayerConnectionsAsync(player);
        
        return HookResult.Continue;
    }

    public async Task HandlePlayerConnectionsAsync(CCSPlayerController player)
    {
        try
        {
            if (!player.IsValid(true)) return;

            ulong steamId = player.SteamID;
            int slot = player.Slot;

            if (g_Main.Player_Data.TryGetValue(slot, out var handle))
            {
                handle.Player = player;
                return;
            }

            await Server.NextFrameAsync(() => Helper.CheckPlayerInGlobals(player));

            if (Configs.Instance.Cookies_Enable > 0)
            {
                var cookieData = Cookies.GetPlayerData(steamId);
                if (cookieData != null)
                {
                    await Server.NextFrameAsync(() => Helper.UpdatePlayerData(player, cookieData));
                }
            }

            if (Configs.Instance.MySql_Enable > 0)
            {
                var mysqlData = await MySqlDataManager.RetrievePersonDataByIdAsync(steamId);
                if (mysqlData != null)
                {
                    await Server.NextFrameAsync(() => Helper.UpdatePlayerData(player, mysqlData));
                }
            }
        }
        catch (Exception ex)
        {
            Helper.DebugMessage($"HandlePlayerConnectionsAsync error: {ex.Message}");
        }
    }

    public HookResult OnEventRoundStart(EventRoundStart @event, GameEventInfo info)
    {
        if (@event == null 
        || !Configs.Instance.AntiBodyBlock_OnRoundStart
        || Configs.Instance.AntiBodyBlock_DisableOnWarmUp && Helper.IsWarmup()) return HookResult.Continue;

        Helper.ChangeConvar();
        Helper.KillAntiBodyBlockTimer_All();
        Helper.ResetAntiBodyBlock();

        Helper.StartAntiBlock_All(true);
        
        g_Main.AntiBodyBlockTimer?.Kill();
        g_Main.AntiBodyBlockTimer = null!;
        g_Main.AntiBodyBlockTimer = AddTimer(Configs.Instance.AntiBodyBlock_OnRoundStartDuration, () =>
        {
            Helper.StartAntiBlock_All(false);
            g_Main.AntiBodyBlockTimer?.Kill();
            g_Main.AntiBodyBlockTimer = null!;
            Helper.AdvancedServerPrintToChatAll(Localizer["PrintToChatToAllPlayers.AntiBodyBlock.Disabled"], Configs.Instance.AntiBodyBlock_OnRoundStartDuration);
        }, TimerFlags.STOP_ON_MAPCHANGE);

        Helper.AdvancedServerPrintToChatAll(Localizer["PrintToChatToAllPlayers.AntiBodyBlock.Enabled"], Configs.Instance.AntiBodyBlock_OnRoundStartDuration);
        return HookResult.Continue;
    }

    public HookResult OnEventRoundEnd(EventRoundEnd @event, GameEventInfo info)
    {
        if (@event == null) return HookResult.Continue;

        Helper.ChangeConvar();
        Helper.KillAntiBodyBlockTimer_All();
        Helper.ResetAntiBodyBlock();
        
        return HookResult.Continue;
    }

    public HookResult OnEventPlayerSpawn(EventPlayerSpawn @event, GameEventInfo info)
    {
        if (@event == null) return HookResult.Continue;

        var player = @event.Userid;
        if (!player.IsValid(true))return HookResult.Continue;

        Helper.KillAntiBodyBlockTimer(player);

        if(Configs.Instance.AntiBodyBlock_OnRoundStartDuration == 0)
        {
            Server.NextFrame(()=>
            {
                if (!player.IsValid(true))return;
                
                Helper.StartAntiBlock(player);
            });
        }
        return HookResult.Continue;
    }
    public HookResult OnEventPlayerDeath(EventPlayerDeath @event, GameEventInfo info)
    {
        if (@event == null) return HookResult.Continue;

        var player = @event.Userid;
        if (!player.IsValid(true))return HookResult.Continue;

        Helper.KillAntiBodyBlockTimer(player);
        return HookResult.Continue;
    }

    public HookResult OnPlayerSay(CCSPlayerController? player, CommandInfo info)
    {
        return HandlePlayerMessage(player, info.ArgString.Trim('"'));
    }

    public HookResult OnPlayerSay_Team(CCSPlayerController? player, CommandInfo info)
    {
        return HandlePlayerMessage(player, info.ArgString.Trim('"'));
    }

    public HookResult OnUserMessage_OnSayText2(CounterStrikeSharp.API.Modules.UserMessages.UserMessage um)
    {
        var player = Utilities.GetPlayerFromIndex(um.ReadInt("entityindex"));
        return HandlePlayerMessage(player, Encoding.UTF8.GetString(um.ReadBytes("param2")), um);
    }
    
    private HookResult HandlePlayerMessage(CCSPlayerController? player, string? rawMessage, CounterStrikeSharp.API.Modules.UserMessages.UserMessage? um = null)
    {
        if (!player.IsValid()) return HookResult.Continue;
        if (string.IsNullOrWhiteSpace(rawMessage)) return HookResult.Continue;

        string message = rawMessage.Trim();
        Game_UserMessages.HookPlayerChat_UserMessages(player, message, um);

        return HookResult.Continue;
    }
    
    public HookResult OnEventPlayerDisconnect(EventPlayerDisconnect @event, GameEventInfo info)
    {
        if (@event.Userid == null) return HookResult.Continue;

        var player = @event.Userid;
        if (!player.IsValid(true)) return HookResult.Continue;

        Helper.KillAntiBodyBlockTimer(player);

        if (g_Main.Player_Data.TryGetValue(player.Slot, out var alldata))
        {
            bool AntiBlockChanged      = alldata.AntiBodyBlock         < 0;

            if (AntiBlockChanged)
            {
                var snapshot = new Globals_Static.PersonData
                {
                    PlayerSteamID     = alldata.SteamId,
                    AntiBodyBlock = alldata.AntiBodyBlock,
                    DateAndTime       = DateTime.Now
                };

                bool saveCookie = Configs.Instance.Cookies_Enable == 1;
                bool saveMySql  = Configs.Instance.MySql_Enable   == 1;

                if (saveCookie || saveMySql)
                {
                    _ = HandlePlayerDisconnectAsync(snapshot, saveCookie, saveMySql);
                }
            }
        }
        
        if (!(Configs.Instance.Cookies_Enable == 2 || Configs.Instance.MySql_Enable == 2))
        {
            if (g_Main.Player_Data.ContainsKey(player.Slot))
            {
                g_Main.Player_Data.Remove(player.Slot);
            }
        }

        return HookResult.Continue;
    }

    public async Task HandlePlayerDisconnectAsync(Globals_Static.PersonData data, bool saveCookie, bool saveMySql)
    {
        try
        {
            if (saveCookie)
            {
                await Cookies.SaveAsync(data);
            }

            if (saveMySql)
            {
                await MySqlDataManager.SaveToMySqlAsync(data);
            }
        }
        catch (Exception ex)
        {
            Helper.DebugMessage($"HandlePlayerDisconnectAsync Error: {ex.Message}");
        }
    }

    public void OnMapEnd()
    {
        try
        {
            Helper.SavePlayersValues();
            Helper.ClearVariables();
        }
        catch (Exception ex)
        {
            Helper.DebugMessage($"OnMapEnd Error: {ex.Message}", true);
        }
    }

    public override void Unload(bool hotReload)
    {
        try
        {
            Helper.RemoveRegisterCommandsAndHooks();
            Helper.ClearVariables(true);
        }
        catch (Exception ex)
        {
            Helper.DebugMessage($"Unload Error: {ex.Message}", true);
        }

        if (hotReload)
        {
            try
            {
                Helper.RemoveRegisterCommandsAndHooks();
                Helper.ClearVariables(true);
            }
            catch (Exception ex)
            {
                Helper.DebugMessage($"Unload hotReload Error: {ex.Message}", true);
            }
        }
    }
    

    /* [ConsoleCommand("css_Test", "testttt")]
    [CommandHelper(whoCanExecute: CommandUsage.CLIENT_AND_SERVER)]
    public void test(CCSPlayerController? player, CommandInfo commandInfo)
    {
        if (!player.IsValid()) return;
    } */
}