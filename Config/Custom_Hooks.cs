using System.Runtime.InteropServices;
using Anti_Block_GoldKingZ.Config;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Memory.DynamicFunctions;
using CounterStrikeSharp.API.Modules.Utils;
using CounterStrikeSharp.API.Modules.Entities.Constants;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Modules.Memory;

namespace Anti_Block_GoldKingZ;

public static class CustomHooks
{
    public static CustomGameData? CustomFunctions { get; private set; }

    private static bool _isHooked   = false;
    private static bool _isStarting = false;

    public static void StartHook()
    {
        if (_isHooked || _isStarting) return;
        _isStarting = true;
        CustomFunctions = new CustomGameData();
        OnFunctionsInitialized();
    }

    internal static void OnFunctionsInitialized()
    {
        if (_isHooked || CustomFunctions == null) return;

        try
        {
            if (CustomFunctions.ShouldCollide != null)
            {
                CustomFunctions.ShouldCollide.Hook(OnShouldCollide, HookMode.Pre);
            }

            _isHooked = true;
            Helper.DebugMessage("All Hooks Started");
        }
        catch (Exception ex)
        {
            Helper.DebugMessage($"Hook Error: {ex.Message}");
        }
        finally
        {
            _isStarting = false;
        }
    }

    public static void UnHook()
    {
        if (!_isHooked || CustomFunctions == null) return;

        try
        {
            if (CustomFunctions.ShouldCollide != null)
            {
                CustomFunctions.ShouldCollide.Unhook(OnShouldCollide, HookMode.Pre);
            }
        }
        catch (Exception ex)
        {
            Helper.DebugMessage($"UnHook Error: {ex.Message}");
        }
        finally
        {
            CustomFunctions = null;
            _isHooked       = false;
            _isStarting     = false;
            Helper.DebugMessage("Hooks Removed");
        }
    }

    private static HookResult OnShouldCollide(DynamicHook hook)
    {
        if (Configs.Instance.AntiBodyBlock.AntiBodyBlock_Mode == 0) return HookResult.Continue;

        try
        {
            nint entity = hook.GetParam<nint>(1);
            if (entity == 0) return HookResult.Continue;

            nint filter = hook.GetParam<nint>(0);
            bool IsPlayerMovement = Marshal.ReadByte(filter + 56) == 11;
            if (!IsPlayerMovement) return HookResult.Continue;

            var targetPawn = new CCSPlayerPawn(entity);
            if (targetPawn == null || !targetPawn.IsValid) return HookResult.Continue;
            
            var playerC = targetPawn.Controller.Value?.As<CCSPlayerController>();
            if (playerC == null || !playerC.IsValid) return HookResult.Continue;

            var moverPawn  = Utilities.GetEntityFromIndex<CCSPlayerPawn>((int)((uint)Marshal.ReadInt32(filter + 32) & 0x7FFF));
            if (moverPawn == null || !moverPawn.IsValid) return HookResult.Continue;

            var playerM = moverPawn.Controller.Value?.As<CCSPlayerController>();
            if (playerM == null || !playerM.IsValid || !MainPlugin.Instance.g_Main.Player_Data.TryGetValue(playerM.Slot, out var data)) return HookResult.Continue;
            
            if (Configs.Instance.AntiBodyBlock.AntiBodyBlock_Mode == 1 && data.AntiBodyBlock is 2 or -2 || Configs.Instance.AntiBodyBlock.AntiBodyBlock_Mode == 2 && data.Timer_NoBlock == null)
            {
                if (Helper.ArePlayersOverlapping(playerM, playerC))
                {
                    hook.SetReturn(false);
                    return HookResult.Handled;
                }
                return HookResult.Continue;
            }

            switch (Configs.Instance.AntiBodyBlock.AntiBodyBlock_Teams)
            {
                case 1 when !(playerM.TeamNum == (byte)CsTeam.Terrorist && playerC.TeamNum == (byte)CsTeam.Terrorist):
                case 2 when !(playerM.TeamNum == (byte)CsTeam.CounterTerrorist && playerC.TeamNum == (byte)CsTeam.CounterTerrorist):
                case 3 when playerM.TeamNum != playerC.TeamNum:
                    return HookResult.Continue;
                case 4:
                    break;
            }
            
            if(Configs.Instance.AntiBodyBlock.AntiBodyBlock_HeadBoost)
            {
                var groundEntity = moverPawn.GroundEntity?.Value;
                if (groundEntity != null && groundEntity.Handle == targetPawn.Handle)return HookResult.Continue;

                var posM = moverPawn.AbsOrigin;
                var posC = targetPawn.AbsOrigin;
                if (posM != null && posC != null)
                {
                    float targetHeadZ = posC.Z + (playerC.IsDucking() ? 48f : 64f);
                    if (posM.Z >= targetHeadZ)return HookResult.Continue;
                }
            }

            hook.SetReturn(false);
            return HookResult.Handled;
        }
        catch { return HookResult.Continue; }
    }
    

    public static void CollisionRulesChanged(this CBaseEntity? entity)
    {
        if(CustomFunctions == null || entity == null || !entity.IsValid)return;

        int offset = CustomFunctions.GetOffset("CBaseEntity_CollisionRulesChanged");
        if (offset <= 0) return;
        
        var CollisionRulesChanged = new VirtualFunctionVoid<nint>(entity.Handle, offset);
        CollisionRulesChanged.Invoke(entity.Handle);
    }
}