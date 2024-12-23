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

namespace Anti_Block_GoldKingZ;


[MinimumApiVersion(276)]
public class AntiBlockGoldKingZ : BasePlugin
{
    public override string ModuleName => "Anti-Block Body/Nades";
    public override string ModuleVersion => "1.0.0";
    public override string ModuleAuthor => "Gold KingZ";
    public override string ModuleDescription => "https://github.com/oqyh";
	internal static IStringLocalizer? Stringlocalizer;
	public static AntiBlockGoldKingZ Instance { get; set; } = new();
    public Globals g_Main = new();

    public override void Load(bool hotReload)
    {
		Instance = this;
        Configs.Load(ModuleDirectory);
        Stringlocalizer = Localizer;
        Configs.Shared.CookiesModule = ModuleDirectory;
        Configs.Shared.StringLocalizer = Localizer;

        RegisterEventHandler<EventRoundStart>(OnRoundStart, HookMode.Post);
        RegisterEventHandler<EventRoundEnd>(OnEventRoundEnd, HookMode.Post);
        RegisterEventHandler<EventPlayerDisconnect>(OnPlayerDisconnect);

		RegisterListener<Listeners.OnEntityCreated>(OnEntityCreated);
		RegisterListener<Listeners.OnMapEnd>(OnMapEnd);
    }

    public HookResult OnRoundStart(EventRoundStart @event, GameEventInfo info)
    {
        if(Configs.GetConfigData().AntiBodyBlock_OnStartRoundDurationXInSecs == 0 || @event == null || Helper.IsWarmup())return HookResult.Continue;

        if(g_Main.AntiBodyBlockTimer != null)
        {
            g_Main.AntiBodyBlockTimer.Kill();
            g_Main.AntiBodyBlockTimer = null!;
        }
        
        g_Main.AntiBodyBlockTimer = AddTimer(1.0f, () => Helper.AntiBodyBlock(true), TimerFlags.REPEAT | TimerFlags.STOP_ON_MAPCHANGE);
        Helper.AdvancedServerPrintToChatAll(Configs.Shared.StringLocalizer!["PrintChatToAllPlayers.AntiBlock.Enabled", Configs.GetConfigData().AntiBodyBlock_OnStartRoundDurationXInSecs]);

        AddTimer(Configs.GetConfigData().AntiBodyBlock_OnStartRoundDurationXInSecs, () =>
        {
            if(g_Main.AntiBodyBlockTimer != null)
            {
                Helper.AntiBodyBlock(false);
                g_Main.AntiBodyBlockTimer.Kill();
                g_Main.AntiBodyBlockTimer = null!;
                Helper.AdvancedServerPrintToChatAll(Configs.Shared.StringLocalizer!["PrintChatToAllPlayers.AntiBlock.Disabled"]);
            }
        }, TimerFlags.STOP_ON_MAPCHANGE);
        
        return HookResult.Continue;
    }
    public HookResult OnEventRoundEnd(EventRoundEnd @event, GameEventInfo info)
    {
        if(Configs.GetConfigData().AntiBodyBlock_OnStartRoundDurationXInSecs == 0 || @event == null || Helper.IsWarmup())return HookResult.Continue;

        if(g_Main.AntiBodyBlockTimer != null)
        {
            g_Main.AntiBodyBlockTimer.Kill();
            g_Main.AntiBodyBlockTimer = null!;
        }
        return HookResult.Continue;
    }


    public void OnEntityCreated(CEntityInstance entity)
    {
        if (!Configs.GetConfigData().AntiBlockNades_IfThrowToEnemyTeam && !Configs.GetConfigData().AntiBlockNades_IfThrowToTeamMates)return;
        if (entity == null || entity.Entity == null || !entity.IsValid || !entity.DesignerName.Contains("_projectile"))return;
        string[] AntiBlockNades_TheseNadess = Configs.GetConfigData().AntiBlockNades_TheseNades.Split(',');
        if (AntiBlockNades_TheseNadess.Any(cmd => entity.DesignerName.StartsWith(cmd, StringComparison.OrdinalIgnoreCase)))
        {
            Server.NextFrame(() =>
            {
                if (entity == null || entity.Entity == null || !entity.IsValid)return;
                var projectile = new CBaseGrenade(entity.Handle);
                if (projectile == null || projectile.Entity == null || !projectile.IsValid)return;

                var pawn = projectile.OriginalThrower.Value;
                if (pawn == null || !pawn.IsValid)return;

                var player = pawn.OriginalController.Value;
                if (player == null || !player.IsValid)return;
                if(Configs.GetConfigData().AntiBlockNades_IfThrowToEnemyTeam && Configs.GetConfigData().AntiBlockNades_IfThrowToTeamMates)
                {
                    Helper.AntiBlock(projectile);
                    return;
                }
                Helper.AntiBlock(projectile);

                if(!g_Main.NadeTracker.ContainsKey(player))
                {
                    g_Main.NadeTracker.Add(player, new Globals.GetNadeAndPlayer(player,projectile, AddTimer(0.01f, () => Helper.NadeTracking(player,projectile), TimerFlags.REPEAT | TimerFlags.STOP_ON_MAPCHANGE)));
                }
                if(g_Main.NadeTracker.ContainsKey(player))
                {
                    g_Main.NadeTracker[player].Timer?.Kill();
                    g_Main.NadeTracker[player].Timer = null!;
                    g_Main.NadeTracker[player] = new Globals.GetNadeAndPlayer(player,projectile, AddTimer(0.01f, () => Helper.NadeTracking(player,projectile), TimerFlags.REPEAT | TimerFlags.STOP_ON_MAPCHANGE));
                }
            });
        }
    }


    public HookResult OnPlayerDisconnect(EventPlayerDisconnect @event, GameEventInfo info)
    {
        if(@event == null)return HookResult.Continue;

        var player = @event.Userid;
        if (player == null || !player.IsValid) return HookResult.Continue;

        if (g_Main.NadeTracker.ContainsKey(player))
        {
            if(g_Main.NadeTracker[player].Timer != null)
            {
                g_Main.NadeTracker[player].Timer?.Kill();
                g_Main.NadeTracker[player].Timer = null!;
            }
            g_Main.NadeTracker.Remove(player);
        }
        return HookResult.Continue;
    }

    private void OnMapEnd()
    {
        Helper.ClearVariables();
    }
    public override void Unload(bool hotReload)
    {
        Helper.ClearVariables();
    }
}