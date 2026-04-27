using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Core;
using System.Text;
using Anti_Block_GoldKingZ.Config;
using CounterStrikeSharp.API.Modules.Commands;
using CounterStrikeSharp.API.Modules.Utils;
using CounterStrikeSharp.API.Core.Translations;
using Microsoft.VisualBasic;
using System.Text.RegularExpressions;
using CounterStrikeSharp.API.Modules.UserMessages;
using CounterStrikeSharp.API.Modules.Cvars;
using CounterStrikeSharp.API.Modules.Timers;
using CounterStrikeSharp.API.Modules.Entities.Constants;

namespace Anti_Block_GoldKingZ;

public class Game_UserMessages
{
    public HookResult HookPlayerChat_UserMessages(CCSPlayerController? player, string message, UserMessage? um = null)
    {
        if (!player.IsValid()) return HookResult.Continue;

        Helper.CheckPlayerInGlobals(player);

        if (Configs.Instance.Reload_AntiBlock.Reload_AntiBlock_CommandsInGame.ConvertCommands(true)?.Any(c => message.Equals(c.Trim(), StringComparison.OrdinalIgnoreCase)) == true)
        {
            Handle_ReloadPlugin(player, null!, um!);
        }

        if (Configs.Instance.AntiBodyBlock.AntiBodyBlock_Mode != 0)
        {
            if (Configs.Instance.AntiBodyBlock.AntiBodyBlock_CommandsInGame.ConvertCommands(true)?.Any(c => message.Equals(c.Trim(), StringComparison.OrdinalIgnoreCase)) == true)
            {
                Handle_AntiBodyBlock(player, null!, um!);
            }
        }

        return HookResult.Continue;
    }


    #region Commands Hook

    public void CommandsAction_ReloadPlugin(CCSPlayerController? player, CommandInfo info)
    {
        if (!player.IsValid()) return;

        Helper.CheckPlayerInGlobals(player);
        Handle_ReloadPlugin(player, info, null!);
    }

    public void CommandsAction_AntiBodyBlock(CCSPlayerController? player, CommandInfo info)
    {
        if (!player.IsValid()) return;

        Helper.CheckPlayerInGlobals(player);
        Handle_AntiBodyBlock(player, info, null!);
    }

    #endregion Commands Hook




    #region Handles

    public static void Handle_ReloadPlugin(CCSPlayerController player, CommandInfo commandInfo = null!, UserMessage um = null!)
    {
        if (!MainPlugin.Instance.g_Main.Player_Data.TryGetValue(player.Slot, out var playerData)) return;

        bool onetime = (DateTime.Now - playerData.EventPlayerChat).TotalSeconds > 0.4;
        if (onetime)
        {
            playerData.EventPlayerChat = DateTime.Now;
        }

        var cfg = Configs.Instance.Reload_AntiBlock;

        if (cfg.Reload_AntiBlock_Flags.HasValidPermissionData() && !Helper.IsPlayerInGroupPermission(player, cfg.Reload_AntiBlock_Flags))
        {
            if (onetime)
            {
                Helper.AdvancedPlayerPrintToChat(player, commandInfo, MainPlugin.Instance.Localizer["PrintToChatToPlayer.ReloadPlugin.Not.Allowed"]);
            }
        }
        else
        {
            if (onetime)
            {
                Server.NextFrame(() =>
                {
                    Helper.RemoveRegisterCommandsAndHooks();
                    Helper.ClearVariables();

                    Configs.Load(MainPlugin.Instance.ModuleDirectory);
                    Helper.DownloadMissingFiles();

                    Helper.RegisterCommandsAndHooks();
                    
                    Helper.ReloadPlayersGlobals();
                });

                Helper.AdvancedPlayerPrintToChat(player, commandInfo, MainPlugin.Instance.Localizer["PrintToChatToPlayer.ReloadPlugin.Successfully"]);
            }

            Helper.MuteCommands(um, cfg.Reload_AntiBlock_Hide);
        }

        Helper.MuteCommands(um, cfg.Reload_AntiBlock_Hide, true);
    }


    public static void Handle_AntiBodyBlock(CCSPlayerController player, CommandInfo commandInfo = null!, UserMessage um = null!)
    {
        if (!MainPlugin.Instance.g_Main.Player_Data.TryGetValue(player.Slot, out var playerData)) return;

        bool onetime = (DateTime.Now - playerData.EventPlayerChat).TotalSeconds > 0.4;
        if (onetime)
        {
            playerData.EventPlayerChat = DateTime.Now;
        }

        var cfg = Configs.Instance.AntiBodyBlock;

        if (cfg.AntiBodyBlock_Flags.HasValidPermissionData() && !Helper.IsPlayerInGroupPermission(player, cfg.AntiBodyBlock_Flags))
        {
            if (onetime)
            {
                Helper.AdvancedPlayerPrintToChat(player, commandInfo, MainPlugin.Instance.Localizer["PrintToChatToPlayer.AntiBodyBlock.Not.Allowed"]);
            }
        }
        else
        {
            if (player.PlayerPawn.Value?.Collision.CollisionAttribute.CollisionGroup == (byte)CollisionGroup.COLLISION_GROUP_DEBRIS)
            {
                if (onetime)
                {
                    Helper.AdvancedPlayerPrintToChat(player, commandInfo, MainPlugin.Instance.Localizer["PrintToChatToPlayer.AntiBodyBlock.AlreadyActive"]);
                }
                Helper.MuteCommands(um, cfg.AntiBodyBlock_Hide, true);
                return;
            }

            if (cfg.AntiBodyBlock_Mode == 1)
            {
                if (onetime)
                {
                    if(MainPlugin.Instance._prefs != null && MainPlugin.Instance._prefs.TryGetValue(player.Slot, out var _prefs))
                    {
                        _prefs.AntiBodyBlock_Toggle = _prefs.AntiBodyBlock_Toggle.ToggleOnOff();
                        if (_prefs.AntiBodyBlock_Toggle)
                        {
                            Helper.AdvancedPlayerPrintToChat(player, commandInfo, MainPlugin.Instance.Localizer["PrintToChatToPlayer.AntiBodyBlock.Mode1.Enabled"]);
                        }
                        else if (!_prefs.AntiBodyBlock_Toggle)
                        {
                            Helper.AdvancedPlayerPrintToChat(player, commandInfo, MainPlugin.Instance.Localizer["PrintToChatToPlayer.AntiBodyBlock.Mode1.Disabled"]);
                        }
                    }
                    
                }
                Helper.MuteCommands(um, cfg.AntiBodyBlock_Hide);
            }
            else if (cfg.AntiBodyBlock_Mode == 2)
            {
                if (cfg.AntiBodyBlock_Mode_2_Cooldown > 0)
                {
                    if (cfg.AntiBodyBlock_Mode_2_Cooldown_ImmunityFlags.HasValidPermissionData() && !Helper.IsPlayerInGroupPermission(player, cfg.AntiBodyBlock_Mode_2_Cooldown_ImmunityFlags))
                    {
                        var remaining = cfg.AntiBodyBlock_Mode_2_Cooldown - (DateTime.Now - playerData.Cooldown).TotalSeconds;
                        if (remaining > 0)
                        {
                            if (onetime)
                            {
                                Helper.AdvancedPlayerPrintToChat(player, commandInfo, MainPlugin.Instance.Localizer["PrintToChatToPlayer.AntiBodyBlock.Mode2.OnCooldown"], (int)Math.Ceiling(remaining));
                            }
                            Helper.MuteCommands(um, cfg.AntiBodyBlock_Hide, true);
                            return;
                        }
                    }
                }

                if (cfg.AntiBodyBlock_Mode_2_MaxUsage > 0)
                {
                    if (cfg.AntiBodyBlock_Mode_2_MaxUsage_ImmunityFlags.HasValidPermissionData() && !Helper.IsPlayerInGroupPermission(player, cfg.AntiBodyBlock_Mode_2_MaxUsage_ImmunityFlags) && playerData.NoBlock_Used >= cfg.AntiBodyBlock_Mode_2_MaxUsage)
                    {
                        if (onetime)
                        {
                            Helper.AdvancedPlayerPrintToChat(player, commandInfo, MainPlugin.Instance.Localizer["PrintToChatToPlayer.AntiBodyBlock.Mode2.MaxUsesReached", cfg.AntiBodyBlock_Mode_2_MaxUsage]);
                        }
                        Helper.MuteCommands(um, cfg.AntiBodyBlock_Hide, true);
                        return;
                    }
                }

                if (onetime)
                {
                    playerData.Timer_NoBlock?.Kill();
                    playerData.Timer_NoBlock = null;
                    playerData.NoBlock_Used++;
                    playerData.Cooldown = DateTime.Now;
                    playerData.Timer_NoBlock = MainPlugin.Instance.AddTimer(Configs.Instance.AntiBodyBlock.AntiBodyBlock_Mode_2_Duration, () =>
                    {
                        if (!player.IsValid() || !MainPlugin.Instance.g_Main.Player_Data.TryGetValue(player.Slot, out var playerData2)) return;

                        playerData2.Timer_NoBlock?.Kill();
                        playerData2.Timer_NoBlock = null;
                        if (player.IsAlive())
                        {
                            Helper.AdvancedPlayerPrintToChat(player, commandInfo, MainPlugin.Instance.Localizer["PrintToChatToPlayer.AntiBodyBlock.Mode2.Disabled"], "");
                        }

                    }, TimerFlags.STOP_ON_MAPCHANGE);

                    Helper.AdvancedPlayerPrintToChat(player, commandInfo, MainPlugin.Instance.Localizer["PrintToChatToPlayer.AntiBodyBlock.Mode2.Enabled"], Configs.Instance.AntiBodyBlock.AntiBodyBlock_Mode_2_Duration);
                }
                Helper.MuteCommands(um, cfg.AntiBodyBlock_Hide);
            }
        }
        Helper.MuteCommands(um, cfg.AntiBodyBlock_Hide, true);
    }

    #endregion Handles
}