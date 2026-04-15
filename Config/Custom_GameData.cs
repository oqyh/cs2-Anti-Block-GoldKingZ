using System.Runtime.InteropServices;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Modules.Memory.DynamicFunctions;
using CounterStrikeSharp.API.Core;
using Newtonsoft.Json.Linq;
using Anti_Block_GoldKingZ.Config;
using CounterStrikeSharp.API.Modules.Memory;

namespace Anti_Block_GoldKingZ;

public class CustomGameData
{
    private readonly Dictionary<string, string> _customSignatures = new();
    private readonly Dictionary<string, string> _customLibraries = new();
    private readonly Dictionary<string, int>    _customOffsets    = new();
    private bool _isDataLoaded = false;
    public MemoryFunctionWithReturn<nint, nint, bool>? ShouldCollide { get; private set; }

    public CustomGameData()
    {
        LoadAndInit();
    }

    private static string? GetModulePath(string library)
    {
        return library.ToLowerInvariant() switch
        {
            "server" => null,
            "vphysics2" => $"{Constants.ModulePrefix}vphysics2{Constants.ModuleSuffix}",
            _ => null
        };
    }

    private void LoadAndInit()
    {
        string jsonFilePath = Path.Combine(MainPlugin.Instance.ModuleDirectory, "gamedata/gamedata.json");

        if (!File.Exists(jsonFilePath))
        {
            Helper.DebugMessage("gamedata.json Not Found");
            return;
        }

        try
        {
            var jsonData   = File.ReadAllText(jsonFilePath);
            var jsonObject = JObject.Parse(jsonData);

            bool   isLinux      = RuntimeInformation.IsOSPlatform(OSPlatform.Linux);
            string platformKey  = isLinux ? "linux" : "windows";

            _customSignatures.Clear();
            _customLibraries.Clear();
            _customOffsets.Clear();

            foreach (var item in jsonObject.Properties())
            {
                string key  = item.Name;
                var    data = item.Value;

                if (data["signatures"]?[platformKey] is { } sig)
                    _customSignatures[key] = sig.ToString();

                if (data["signatures"]?["library"] is { } lib)
                    _customLibraries[key] = lib.ToString();
                else
                    _customLibraries[key] = "server";

                if (data["offsets"]?[platformKey] is { } off)
                    _customOffsets[key] = off.Value<int>();
            }

            _isDataLoaded = true;
            InitializeFunctions();
        }
        catch (Exception ex)
        {
            Helper.DebugMessage($"LoadAndInit Error: {ex.Message}");
        }
    }

    private void InitializeFunctions()
    {
        if (!_isDataLoaded) return;

        try
        {
            if (Configs.Instance.AntiBodyBlock.AntiBodyBlock_Mode != 0)
            {
                ShouldCollide = TryCreate<MemoryFunctionWithReturn<nint, nint, bool>>("ShouldCollide");
            }

        }
        catch (Exception ex)
        {
            Helper.DebugMessage($"InitializeFunctions Error: {ex.Message}");
        }
    }

    private T? TryCreate<T>(string key) where T : class
    {
        if (!_customSignatures.TryGetValue(key, out var sig)) return null;

        var module = GetModulePath(_customLibraries.GetValueOrDefault(key, "server"));

        try
        {
            var result = module != null
                ? (T)Activator.CreateInstance(typeof(T), sig, module)!
                : (T)Activator.CreateInstance(typeof(T), sig)!;
            return result;
        }
        catch (Exception ex)
        {
            Helper.DebugMessage($"{key} TryCreate Error: {ex.Message}");
            return null;
        }
    }

    public string GetSignature(string key) => _customSignatures.TryGetValue(key, out var sig) ? sig : "Signature not found";
    public int GetOffset(string key) => _customOffsets.TryGetValue(key, out int val) ? val : -1;
}