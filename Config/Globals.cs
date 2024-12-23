using CounterStrikeSharp.API.Core;
using System.Diagnostics;

namespace Anti_Block_GoldKingZ;

public class Globals
{
    public class GetNadeAndPlayer
    {
        public CCSPlayerController Player { get; set; }
        public CBaseEntity Nade { get; set; }
        public CounterStrikeSharp.API.Modules.Timers.Timer Timer { get; set; }
        
        public GetNadeAndPlayer(CCSPlayerController player, CBaseEntity nade, CounterStrikeSharp.API.Modules.Timers.Timer timer)
        {
            Player = player;
            Nade = nade;
            Timer = timer;
        }
    }
    public Dictionary<CCSPlayerController, GetNadeAndPlayer> NadeTracker = new Dictionary<CCSPlayerController, GetNadeAndPlayer>();
    public CounterStrikeSharp.API.Modules.Timers.Timer? AntiBodyBlockTimer { get; set; }
}