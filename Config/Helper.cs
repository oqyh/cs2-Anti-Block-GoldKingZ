using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Admin;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Modules.Utils;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using System.Text.Json;
using System.Text.Encodings.Web;
using CounterStrikeSharp.API.Modules.Memory;
using CounterStrikeSharp.API.Modules.Cvars;
using System.Runtime.InteropServices;
using CounterStrikeSharp.API.Modules.Entities.Constants;
using Anti_Block_GoldKingZ.Config;


namespace Anti_Block_GoldKingZ;

public class Helper
{
    public static void AdvancedPlayerPrintToChat(CCSPlayerController player, string message, params object[] args)
    {
        if (string.IsNullOrEmpty(message))return;

        for (int i = 0; i < args.Length; i++)
        {
            message = message.Replace($"{{{i}}}", args[i].ToString());
        }
        if (Regex.IsMatch(message, "{nextline}", RegexOptions.IgnoreCase))
        {
            string[] parts = Regex.Split(message, "{nextline}", RegexOptions.IgnoreCase);
            foreach (string part in parts)
            {
                string messages = part.Trim();
                player.PrintToChat(" " + messages);
            }
        }else
        {
            player.PrintToChat(message);
        }
    }
    public static void AdvancedServerPrintToChatAll(string message, params object[] args)
    {
        if (string.IsNullOrEmpty(message))return;

        for (int i = 0; i < args.Length; i++)
        {
            message = message.Replace($"{{{i}}}", args[i].ToString());
        }
        if (Regex.IsMatch(message, "{nextline}", RegexOptions.IgnoreCase))
        {
            string[] parts = Regex.Split(message, "{nextline}", RegexOptions.IgnoreCase);
            foreach (string part in parts)
            {
                string messages = part.Trim();
                Server.PrintToChatAll(" " + messages);
            }
        }else
        {
            Server.PrintToChatAll(message);
        }
    }
    public static void AdvancedPlayerPrintToConsole(CCSPlayerController player, string message, params object[] args)
    {
        if (string.IsNullOrEmpty(message))return;
        
        for (int i = 0; i < args.Length; i++)
        {
            message = message.Replace($"{{{i}}}", args[i].ToString());
        }
        if (Regex.IsMatch(message, "{nextline}", RegexOptions.IgnoreCase))
        {
            string[] parts = Regex.Split(message, "{nextline}", RegexOptions.IgnoreCase);
            foreach (string part in parts)
            {
                string messages = part.Trim();
                player.PrintToConsole(" " + messages);
            }
        }else
        {
            player.PrintToConsole(message);
        }
    }
    
    public static bool IsPlayerInGroupPermission(CCSPlayerController player, string groups)
    {
        var excludedGroups = groups.Split(',');
        foreach (var group in excludedGroups)
        {
            switch (group[0])
            {
                case '#':
                    if (AdminManager.PlayerInGroup(player, group))
                    {
                        return true;
                    }
                    break;

                case '@':
                    if (AdminManager.PlayerHasPermissions(player, group))
                    {
                        return true;
                    }
                    break;

                default:
                if (AdminManager.PlayerInGroup(player, group))
                {
                    return true;
                }
                break;
            }
        }   
        return false;
    }
    public static List<CCSPlayerController> GetPlayersController(bool IncludeBots = false, bool IncludeSPEC = true, bool IncludeCT = true, bool IncludeT = true) 
    {
        var playerList = Utilities
            .FindAllEntitiesByDesignerName<CCSPlayerController>("cs_player_controller")
            .Where(p => p != null && p.IsValid && 
                        (IncludeBots || (!p.IsBot && !p.IsHLTV)) && 
                        p.Connected == PlayerConnectedState.PlayerConnected && 
                        ((IncludeCT && p.TeamNum == (byte)CsTeam.CounterTerrorist) || 
                        (IncludeT && p.TeamNum == (byte)CsTeam.Terrorist) || 
                        (IncludeSPEC && p.TeamNum == (byte)CsTeam.Spectator)))
            .ToList();

        return playerList;
    }
    public static int GetPlayersCount(bool IncludeBots = false, bool IncludeSPEC = true, bool IncludeCT = true, bool IncludeT = true)
    {
        return Utilities.GetPlayers().Count(p => 
            p != null && 
            p.IsValid && 
            p.Connected == PlayerConnectedState.PlayerConnected && 
            (IncludeBots || (!p.IsBot && !p.IsHLTV)) && 
            ((IncludeCT && p.TeamNum == (byte)CsTeam.CounterTerrorist) || 
            (IncludeT && p.TeamNum == (byte)CsTeam.Terrorist) || 
            (IncludeSPEC && p.TeamNum == (byte)CsTeam.Spectator))
        );
    }
    
    public static void ClearVariables()
    {
        foreach (var entry in AntiBlockGoldKingZ.Instance.g_Main.NadeTracker)
        {
            if (entry.Value.Timer != null)
            {
                entry.Value.Timer.Kill();
                entry.Value.Timer = null!;
            }
        }
        AntiBlockGoldKingZ.Instance.g_Main.NadeTracker.Clear();

        if(AntiBlockGoldKingZ.Instance.g_Main.AntiBodyBlockTimer!=null)
        {
            AntiBlockGoldKingZ.Instance.g_Main.AntiBodyBlockTimer.Kill();
            AntiBlockGoldKingZ.Instance.g_Main.AntiBodyBlockTimer = null!;
        }
        
    }
    public static CCSGameRules? GetGameRules()
    {
        try
        {
            var gameRulesEntities = Utilities.FindAllEntitiesByDesignerName<CCSGameRulesProxy>("cs_gamerules");
            return gameRulesEntities.First().GameRules;
        }
        catch
        {
            return null;
        }
    }
    public static bool IsWarmup()
    {
        return GetGameRules()?.WarmupPeriod ?? false;
    }
	
	
	public static void DebugMessage(string message)
    {
        if(!Configs.GetConfigData().EnableDebug)return;
        Console.WriteLine($"================================= [ Debug Anti Block Nade ] =================================");
        Console.WriteLine(message);
        Console.WriteLine("=====================================================================================================");
    }

    

    public static void NadeTracking(CCSPlayerController? player, CBaseEntity? projectile)
    {
        if(player == null || !player.IsValid)return;

        if(projectile != null && projectile.IsValid)
        { 
            var grenadePos = projectile.AbsOrigin;
            float radius = 100.0f;

            foreach (var otherPlayer in GetPlayersController(true, false))
            {
                if (otherPlayer == null || player == null || !otherPlayer.IsValid || !player.IsValid || player == otherPlayer) continue;

                if (otherPlayer.PlayerPawn == null || !otherPlayer.PlayerPawn.IsValid ||
                    otherPlayer.PlayerPawn.Value == null || !otherPlayer.PlayerPawn.Value.IsValid) continue;

                var playerPos = otherPlayer.PlayerPawn.Value.AbsOrigin;
                if (playerPos == null || grenadePos == null) continue;

                float distanceToGrenade = CalculateDistance(grenadePos, playerPos);

                if (distanceToGrenade <= radius)
                {
                    if (otherPlayer.TeamNum != player.TeamNum)
                    {
                        if (Configs.GetConfigData().AntiBlockNades_IfThrowToEnemyTeam)
                        {
                            AntiBlock(projectile);
                        }
                        else
                        {
                            NadeBlock(projectile);
                        }
                    }
                    else if (otherPlayer.TeamNum == player.TeamNum)
                    {
                        if (Configs.GetConfigData().AntiBlockNades_IfThrowToTeamMates)
                        {
                            AntiBlock(projectile);
                        }
                        else
                        {
                            NadeBlock(projectile);
                        }
                    }
                }
            }
        }else
        {
            if (AntiBlockGoldKingZ.Instance.g_Main.NadeTracker.ContainsKey(player))
            {
                AntiBlockGoldKingZ.Instance.g_Main.NadeTracker[player].Timer?.Kill();
                AntiBlockGoldKingZ.Instance.g_Main.NadeTracker[player].Timer = null!;
            }
        }
    }
    public static void AntiBlock(CBaseEntity entity)
    {
        if(entity == null || !entity.IsValid)return;
        if(entity.Collision == null) return;

        entity.Collision.CollisionGroup = 5;
        Utilities.SetStateChanged(entity, "CCollisionProperty", "m_collisionAttribute");
    }
    public static void NadeBlock(CBaseEntity entity)
    {
        if(entity == null || !entity.IsValid)return;
        if(entity.Collision == null) return;

        entity.Collision.CollisionGroup = 16;
        Utilities.SetStateChanged(entity, "CCollisionProperty", "m_collisionAttribute");
    }
    public static float CalculateDistance(Vector pos1, Vector pos2)
    {
        float dx = pos1.X - pos2.X;
        float dy = pos1.Y - pos2.Y;
        float dz = pos1.Z - pos2.Z;
        return MathF.Sqrt(dx * dx + dy * dy + dz * dz);
    }
    public static void AntiBodyBlock(bool AntiB)
    {
        foreach(var players in GetPlayersController(true,false))
        {
            if(AntiB)
            {
                if(players.PlayerPawn.Value!.Collision.CollisionAttribute.CollisionGroup != 5)
                {
                    players.PlayerPawn.Value!.Collision.CollisionAttribute.CollisionGroup = 5;
                    Utilities.SetStateChanged(players.PlayerPawn.Value!, "CCollisionProperty", "m_collisionAttribute");
                }
                if(players.PlayerPawn.Value!.Collision.CollisionGroup != 5)
                {
                    players.PlayerPawn.Value!.Collision.CollisionGroup = 5;
                }
            }else
            {
                if(players.PlayerPawn.Value!.Collision.CollisionAttribute.CollisionGroup != 8)
                {
                    players.PlayerPawn.Value!.Collision.CollisionAttribute.CollisionGroup = 8;
                    Utilities.SetStateChanged(players.PlayerPawn.Value!, "CCollisionProperty", "m_collisionAttribute");
                }
                if(players.PlayerPawn.Value!.Collision.CollisionGroup != 8)
                {
                    players.PlayerPawn.Value!.Collision.CollisionGroup = 8;
                }
            }
        }
    }
}