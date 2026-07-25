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
    private static MemoryFunctionWithReturn<nint, nint, bool>? ShouldCollide;
    private static MemoryFunctionWithReturn<nint, bool>?       CBaseCSGrenadeProjectile_OnThrow;
    private static MemoryFunctionWithReturn<IntPtr, int, IntPtr, IntPtr, IntPtr, byte>? OnConVarChanged;
    private static bool _isHooked = false;

    public static void Init(CustomGameData gameData)
    {
        if (_isHooked) return;

        try
        {
            if (Configs.Instance.AntiBodyBlock.AntiBodyBlock_Mode != 0)
            {
                ShouldCollide = gameData.CreateFunction<MemoryFunctionWithReturn<nint, nint, bool>>("ShouldCollide");

                if(Configs.Instance.UseOnConVarChangedHook)
                {
                    OnConVarChanged = gameData.CreateFunction<MemoryFunctionWithReturn<IntPtr, int, IntPtr, IntPtr, IntPtr, byte>>("OnConVarChanged");
                    if (OnConVarChanged != null)
                    {
                        OnConVarChanged.Hook(OnConVarChanged_Hook, HookMode.Pre);
                        Helper.ChangeConvar();
                    }
                }
                
            }
            if (Configs.Instance.AntiNadeBlock_Enable == 4)
            {
                CBaseCSGrenadeProjectile_OnThrow = gameData.CreateFunction<MemoryFunctionWithReturn<nint, bool>>("CBaseCSGrenadeProjectile_OnThrow");
            }

            if (ShouldCollide != null)
            {
                ShouldCollide.Hook(OnShouldCollide, HookMode.Pre);
            }
            if (CBaseCSGrenadeProjectile_OnThrow != null)
            {
                CBaseCSGrenadeProjectile_OnThrow.Hook(OnCBaseCSGrenadeProjectile_OnThrow,  HookMode.Pre);
            }

            _isHooked = true;
            Helper.DebugMessage("All Hooks Started");
        }
        catch (Exception ex)
        {
            Helper.DebugMessage($"Hook Init Error: {ex.Message}");
        }
    }

    public static void Cleanup()
    {
        if (!_isHooked) return;

        try
        {
            if (ShouldCollide != null)
            {
                ShouldCollide.Unhook(OnShouldCollide, HookMode.Pre);
            }

            if (CBaseCSGrenadeProjectile_OnThrow != null)
            {
                CBaseCSGrenadeProjectile_OnThrow.Unhook(OnCBaseCSGrenadeProjectile_OnThrow,  HookMode.Pre);
            }

            if (OnConVarChanged != null)
            {
                OnConVarChanged.Unhook(OnConVarChanged_Hook, HookMode.Pre);
            }
        }
        catch (Exception ex)
        {
            Helper.DebugMessage($"Hook Cleanup Error: {ex.Message}");
        }
        finally
        {
            ShouldCollide = null;
            CBaseCSGrenadeProjectile_OnThrow = null;
            OnConVarChanged = null;
            _isHooked      = false;
            Helper.DebugMessage("Hooks Removed");
        }
    }

    public static HookResult OnConVarChanged_Hook(DynamicHook hook)
    {
        try
        {
            var cvarRefPtr = hook.GetParam<IntPtr>(0);
            var slot       = hook.GetParam<int>(1);
            var valuePtr   = hook.GetParam<IntPtr>(2);

            if (cvarRefPtr == IntPtr.Zero || valuePtr == IntPtr.Zero)
                return HookResult.Continue;

            string cvarName = ReadConVarName(cvarRefPtr);
            if (string.IsNullOrEmpty(cvarName))
                return HookResult.Continue;

            string newValue = Marshal.PtrToStringAnsi(valuePtr) ?? "";

            if (MainPlugin.Instance.g_Main.HookConVars.TryGetValue(cvarName, out string? expectedValue))
            {
                if (!newValue.Equals(expectedValue, StringComparison.OrdinalIgnoreCase))
                {
                    Helper.DebugMessage($"[OnConVarChanged_Hook] Slot {slot} attempted to change \"{cvarName}\" from \"{expectedValue}\" to \"{newValue}\", forcing back to \"{expectedValue}\"");

                    Server.NextFrame(() =>
                    {
                        Server.ExecuteCommand($"{cvarName} {expectedValue}");
                    });
                }
            }

            return HookResult.Continue;
        }
        catch (Exception ex)
        {
            Helper.DebugMessage("[OnConVarChanged_Hook] Error in hook callback");
            Helper.DebugMessage(ex.ToString());
            return HookResult.Continue;
        }
    }
    private static string ReadConVarName(IntPtr cvarRefPtr)
    {
        try
        {
            var cvarPtr = Marshal.ReadIntPtr(cvarRefPtr, 0x8);
            if (cvarPtr == IntPtr.Zero) return "";

            var namePtr = Marshal.ReadIntPtr(cvarPtr, 0x0);
            if (namePtr == IntPtr.Zero) return "";

            return Marshal.PtrToStringAnsi(namePtr) ?? "";
        }
        catch
        {
            return "";
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

            var moverPawn = Utilities.GetEntityFromIndex<CCSPlayerPawn>((int)((uint)Marshal.ReadInt32(filter + 32) & 0x7FFF));
            if (moverPawn == null || !moverPawn.IsValid) return HookResult.Continue;

            var playerM = moverPawn.Controller.Value?.As<CCSPlayerController>();
            if (playerM == null || !playerM.IsValid || MainPlugin.Instance._prefs == null || !MainPlugin.Instance._prefs.TryGetValue(playerM.Slot, out var _prefs)) return HookResult.Continue;

            if (Configs.Instance.AntiBodyBlock.AntiBodyBlock_Mode == 1 && !_prefs.AntiBodyBlock_Toggle || Configs.Instance.AntiBodyBlock.AntiBodyBlock_Mode == 2 && MainPlugin.Instance.g_Main.Player_Data.TryGetValue(playerM.Slot, out var playerData) && playerData.Timer_NoBlock == null)
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

            if (Configs.Instance.AntiBodyBlock.AntiBodyBlock_HeadBoost)
            {
                var groundEntity = moverPawn.GroundEntity?.Value;
                if (groundEntity != null && groundEntity.Handle == targetPawn.Handle) return HookResult.Continue;

                var posM = moverPawn.AbsOrigin;
                var posC = targetPawn.AbsOrigin;
                if (posM != null && posC != null)
                {
                    float targetHeadZ = posC.Z + (playerC.IsDucking() ? 48f : 64f);
                    if (posM.Z >= targetHeadZ) return HookResult.Continue;
                }
            }

            hook.SetReturn(false);
            return HookResult.Handled;
        }
        catch { return HookResult.Continue; }
    }

    private static HookResult OnCBaseCSGrenadeProjectile_OnThrow(DynamicHook hook)
    {
        if (Configs.Instance.AntiNadeBlock_Enable == 0) return HookResult.Continue;
        
        if (CustomPatches.IsDynamicPatchActive)
        {
            CustomPatches.RestoreDynamic();
        }

        if (!CustomPatches.IsDynamicPatchReady) return HookResult.Continue;

        try
        {
            nint grenadePtr = hook.GetParam<nint>(0);
            if (grenadePtr == nint.Zero) return HookResult.Continue;

            var grenade = new CBaseEntity(grenadePtr);
            if (!grenade.IsValid) return HookResult.Continue;

            string? canonical = Helper.DesignerToCanonical(grenade.DesignerName);
            if (canonical == null) return HookResult.Continue;

            bool bounceTeam  = MainPlugin.Instance.g_Main.NadeBounce_Teammates.TryGetValue(canonical, out bool bt) && bt;
            bool bounceEnemy = !MainPlugin.Instance.g_Main.NadeBounce_Enemies.TryGetValue(canonical, out bool be) || be;

            if (!bounceTeam && !bounceEnemy)
            {
                hook.SetReturn(false);
                return HookResult.Handled;
            }

            if (bounceTeam)
            {
                CustomPatches.ApplyDynamic();
            }
        }
        catch (Exception ex)
        {
            Helper.DebugMessage($"OnGrenadeBouncePre Error: {ex.Message}");
        }

        return HookResult.Continue;
    }
    public static void CollisionRulesChanged(this CBaseEntity? entity)
    {
        var gameData = CustomGameData.Instance;
        if (gameData == null || entity == null || !entity.IsValid) return;

        int offset = gameData.GetOffset("CBaseEntity_CollisionRulesChanged");
        if (offset <= 0) return;

        var CollisionRulesChanged = new VirtualFunctionVoid<nint>(entity.Handle, offset);
        CollisionRulesChanged.Invoke(entity.Handle);
    }
}