using Anti_Block_GoldKingZ.Config;

namespace Anti_Block_GoldKingZ;

public static class CustomPatches
{
    private static MemoryPatch? _staticPatch;
    private static MemoryPatch? _dynamicPatch;
    private static readonly string PATCH_INVERT  = "0F 84";
    private static readonly int OFFSET_INVERT = 7;
    private static readonly string PATCH_PASS    = "B0 00 90 90 90";
    private static readonly string PATCH_BOUNCE  = "B0 01 90 90 90";
    private static bool _staticApplied = false;
    private static bool _dynamicApplied = false;
    private static bool _isInitialized = false;
    public static bool IsDynamicPatchReady  => _dynamicPatch != null;
    public static bool IsDynamicPatchActive => _dynamicApplied;

    public static void Init(CustomGameData gameData)
    {
        if (_isInitialized) return;

        int mode = Configs.Instance.AntiNadeBlock_Enable;

        try
        {
            switch (mode)
            {
                case 0:
                    break;

                case 1:
                    _staticPatch = gameData.CreatePatch("CBaseCSGrenadeProjectile_OnThrow_Patch");
                    if (_staticPatch != null && _staticPatch.Apply(PATCH_INVERT, OFFSET_INVERT))
                    {
                        _staticApplied = true;
                    }
                    break;

                case 2:
                    _staticPatch = gameData.CreatePatch("CBaseCSGrenadeProjectile_OnThrow_Patch");
                    if (_staticPatch != null && _staticPatch.Apply(PATCH_PASS))
                    {
                        _staticApplied = true;
                    }
                    break;

                case 3:
                    _staticPatch = gameData.CreatePatch("CBaseCSGrenadeProjectile_OnThrow_Patch");
                    if (_staticPatch != null && _staticPatch.Apply(PATCH_BOUNCE))
                    {
                        _staticApplied = true;
                    }
                    break;

                case 4:
                    _dynamicPatch = gameData.CreatePatch("CBaseCSGrenadeProjectile_OnThrow_Patch");
                    break;
            }

            _isInitialized = true;
            Helper.DebugMessage("All Patches Initialized");
        }
        catch (Exception ex)
        {
            Helper.DebugMessage($"Patch Init Error: {ex.Message}");
        }
    }

    public static void Cleanup()
    {
        if (!_isInitialized) return;

        RestoreDynamic();

        if (_staticApplied && _staticPatch != null)
        {
            try
            {
                _staticPatch.Restore();
            }catch (Exception ex)
            {
                Helper.DebugMessage($"Static patch restore Error: {ex.Message}");
            }
        }

        _staticPatch    = null;
        _staticApplied  = false;
        _dynamicPatch   = null;
        _dynamicApplied = false;
        _isInitialized  = false;
        Helper.DebugMessage("All Patches Cleaned Up");
    }

    public static void ApplyDynamic()
    {
        if (_dynamicApplied || _dynamicPatch == null) return;
        _dynamicPatch.Apply(PATCH_BOUNCE, 0);
        _dynamicApplied = true;
    }

    public static void RestoreDynamic()
    {
        if (!_dynamicApplied || _dynamicPatch == null) return;
        _dynamicPatch.Restore();
        _dynamicApplied = false;
    }
}