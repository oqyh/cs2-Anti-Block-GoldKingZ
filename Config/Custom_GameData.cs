using System.Runtime.InteropServices;
using CounterStrikeSharp.API;
using CounterStrikeSharp.API.Modules.Memory;
using CounterStrikeSharp.API.Modules.Memory.DynamicFunctions;
using CounterStrikeSharp.API.Core;
using Newtonsoft.Json.Linq;
using Anti_Block_GoldKingZ.Config;

namespace Anti_Block_GoldKingZ;

public class CustomGameData
{
    private static CustomGameData? _instance;
    public static CustomGameData? Instance => _instance;

    private readonly Dictionary<string, string> _signatures = new();
    private readonly Dictionary<string, string> _libraries  = new();
    private readonly Dictionary<string, int>    _offsets    = new();
    private readonly Dictionary<string, string> _patches    = new();
    private bool _isLoaded = false;

    private static string? GetModulePath(string library) => library.ToLowerInvariant() switch
    {
        "server"    => null,
        "vphysics2" => $"{Constants.ModulePrefix}vphysics2{Constants.ModuleSuffix}",
        _           => null
    };

    public static void Load()
    {
        if (_instance != null) return;

        _instance = new CustomGameData();
        _instance.LoadFromJson();

        if (!_instance._isLoaded)
        {
            Helper.DebugMessage("GameData failed to load");
            return;
        }

        CustomPatches.Init(_instance);
        CustomHooks.Init(_instance);

        Helper.DebugMessage("GameData loaded");
    }

    public static void Unload()
    {
        if (_instance == null) return;

        CustomPatches.Cleanup();
        CustomHooks.Cleanup();

        _instance = null;
        Helper.DebugMessage("GameData unloaded");
    }

    public string GetSignature(string key)  => _signatures.TryGetValue(key, out var s) ? s : string.Empty;
    public int    GetOffset(string key)     => _offsets.TryGetValue(key, out var v) ? v : -1;
    public string GetPatchBytes(string key) => _patches.TryGetValue(key, out var p) ? p : string.Empty;
    public string GetLibrary(string key)    => _libraries.GetValueOrDefault(key, "server");

    public T? CreateFunction<T>(string key) where T : class
    {
        string sig = GetSignature(key);
        if (string.IsNullOrEmpty(sig))
        {
            Helper.DebugMessage($"{key} signature missing for this platform");
            return null;
        }

        var module = GetModulePath(GetLibrary(key));

        try
        {
            return module != null
                ? (T)Activator.CreateInstance(typeof(T), sig, module)!
                : (T)Activator.CreateInstance(typeof(T), sig)!;
        }
        catch (Exception ex)
        {
            Helper.DebugMessage($"{key} CreateFunction Error: {ex.Message}");
            return null;
        }
    }

    public MemoryPatch? CreatePatch(string key)
    {
        string sig = GetSignature(key);
        if (string.IsNullOrEmpty(sig))
        {
            Helper.DebugMessage($"{key} signature missing for this platform");
            return null;
        }

        var module = GetModulePath(GetLibrary(key));
        var patch = new MemoryPatch(module);

        if (!patch.Init(sig))
        {
            Helper.DebugMessage($"{key} signature not found in {GetLibrary(key)}");
            return null;
        }
        return patch;
    }

    private void LoadFromJson()
    {
        string jsonFilePath = Path.Combine(MainPlugin.Instance.ModuleDirectory, "gamedata/gamedata.json");
        if (!File.Exists(jsonFilePath))
        {
            Helper.DebugMessage("gamedata.json Not Found");
            return;
        }

        try
        {
            var jsonObject = JObject.Parse(File.ReadAllText(jsonFilePath));
            string platformKey = RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ? "linux" : "windows";

            _signatures.Clear();
            _libraries.Clear();
            _offsets.Clear();
            _patches.Clear();

            foreach (var item in jsonObject.Properties())
            {
                string key  = item.Name;
                var    data = item.Value;

                if (data["signatures"]?[platformKey] is { } sig) _signatures[key] = sig.ToString();
                _libraries[key] = data["signatures"]?["library"]?.ToString() ?? "server";
                if (data["offsets"]?[platformKey]  is { } off)   _offsets[key]   = off.Value<int>();
                if (data["patches"]?[platformKey]  is { } pat)   _patches[key]   = pat.ToString();
            }

            _isLoaded = true;
        }
        catch (Exception ex)
        {
            Helper.DebugMessage($"LoadFromJson Error: {ex.Message}");
        }
    }
}