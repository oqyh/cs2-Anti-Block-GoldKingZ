using CounterStrikeSharp.API.Core;
using System.Diagnostics;
using CounterStrikeSharp.API.Modules.Entities.Constants;
using Anti_Block_GoldKingZ.Config;

namespace Anti_Block_GoldKingZ;

public static class Globals_Static
{
    public class PersonData
    {
        public ulong PlayerSteamID { get; set; }
        public int AntiBodyBlock { get; set; }
        public DateTime DateAndTime { get; set; }
    }
}


public class Globals
{
    public class PlayerDataClass
    {
        public CCSPlayerController Player { get; set; }
        public ulong SteamId { get; set; }
        public CounterStrikeSharp.API.Modules.Timers.Timer? Timer_NoBlock { get; set; }
        public int NoBlock_Used { get; set; }
        public DateTime Cooldown { get; set; }
        public DateTime EventPlayerChat { get; set; }
        public DateTime DateAndTime { get; set; }
        
        public PlayerDataClass(CCSPlayerController Playerr, ulong SteamIdd, CounterStrikeSharp.API.Modules.Timers.Timer? Timer_NoBlockk, int NoBlock_Usedd, DateTime Cooldownn, DateTime EventPlayerChatt, DateTime DateAndTimee)
        {
            Player = Playerr;
            SteamId = SteamIdd;
            Timer_NoBlock = Timer_NoBlockk;            
            NoBlock_Used = NoBlock_Usedd;            
            Cooldown = Cooldownn;            
            EventPlayerChat = EventPlayerChatt;
            DateAndTime = DateAndTimee;
        }
    }
    public Dictionary<int, PlayerDataClass> Player_Data = new Dictionary<int, PlayerDataClass>();
    public CounterStrikeSharp.API.Modules.Timers.Timer? AntiBodyBlockTimer { get; set; }
    public bool Downloading_FromGithub = false;
    public Dictionary<string, bool> NadeBounce_Teammates = new(StringComparer.OrdinalIgnoreCase);
    public Dictionary<string, bool> NadeBounce_Enemies  = new(StringComparer.OrdinalIgnoreCase);
    public Dictionary<string, string> HookConVars = new();

    public void Clear()
    {
        AntiBodyBlockTimer?.Kill();
        AntiBodyBlockTimer = null!;
        Player_Data?.Clear();
    }
}