using System.Reflection;
using System.Globalization;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Encodings.Web;

namespace Anti_Block_GoldKingZ.Config
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)] public class ForceStringAttribute : Attribute { public string FallbackValue { get; } public ForceStringAttribute(string fallbackValue) { FallbackValue = fallbackValue; } }
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)] public class StringAttribute : Attribute { public string[] Keys { get; } public StringAttribute(params string[] keys) => Keys = keys; }
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true)] public class CommentAttribute : Attribute { public string Text; public CommentAttribute(string t) => Text = t; }
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)] public class BreakLineAttribute : Attribute { public string Text; public BreakLineAttribute(string t) => Text = t; }
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = true)] public class InfoAttribute : Attribute { public string Key { get; } public InfoAttribute(string key) => Key = key; }
    [AttributeUsage(AttributeTargets.Property)] public class RangeAttribute : Attribute { public double Min, Max, Default; public string? Message; public RangeAttribute(double min, double max, double def, string? msg = null) { Min = min; Max = max; Default = def; Message = msg; } }

    public class Reload_AntiBlock
    {
        [Comment("Note: Console_Commands Can Be Execute Via Both Console And Chat By (! or css_)")]
        [Comment("Making Both Console_Commands And Chat_Commands Empty = Disable")]
        [String("Console_Commands", "Chat_Commands")]
        public string Reload_AntiBlock_CommandsInGame { get; set; } = "Console_Commands: css_reloadantiblock,css_reloadab | Chat_Commands: ";

        [Comment("If [Reload_AntiBlock_CommandsInGame] Is Used, Flags Or Group Or SteamID")]
        [Comment("Example:")]
        [Comment("\"SteamIDs: 76561198206086993,STEAM_0:1:507335558 | Flags: @css/root,@css/admin | Groups: #css/root,#css/admin\"")]
        [Comment("\"SteamIDs:  | Flags:  | Groups: \" = To Allow Everyone")]
        [String("SteamIDs", "Flags", "Groups")]
        public string Reload_AntiBlock_Flags { get; set; } = "SteamIDs: 76561198206086993,STEAM_0:1:507335558 | Flags: @css/root,@css/admin | Groups: #css/root,#css/admin";

        [Comment("If [Reload_AntiBlock_CommandsInGame] Is Used, Hide Chat After Execute Reload_AntiBlock_CommandsInGame?:")]
        [Comment("0 = No")]
        [Comment("1 = Yes, But Only After Toggle Successfully")]
        [Comment("2 = Yes, Hide All The Time")]
        [Range(0, 2, 0,
        "Reload_AntiBlock_Hide: is invalid, setting to default value (0) Please Choose From 0 To 2." +
        "0 = No\n" +
        "1 = Yes, But Only After Toggle Successfully\n" +
        "2 = Yes, Hide All The Time")]
        public int Reload_AntiBlock_Hide { get; set; } = 0;
    }

    public class AntiBodyBlock
    {

        [Comment("How [AntiBodyBlock_CommandsInGame] Behaves When Executed By A Player")]
        [Comment("0 = Disabled Anti-BodyBlock Client Side")]
        [Comment("1 = Make It Toggle Mode (Execute Once To Enable, Execute Again To Disable)")]
        [Comment("2 = Make It Timed Mode (Execute To Enable For Seconds, Then Auto Disable)")]
        [Range(0, 2, 2,
        "AntiBodyBlock_Mode: invalid value, setting to default (2).\n" +
        "0 = Disabled Anti-BodyBlock Client Side\n" +
        "1 = Make It Toggle Mode (Execute Once To Enable, Execute Again To Disable)\n" +
        "2 = Make It Timed Mode (Execute To Enable For Seconds, Then Auto Disable)")]
        public int AntiBodyBlock_Mode { get; set; } = 2;

        [Comment("Commands For [AntiBodyBlock_Mode] To Execute Anti-BodyBlock Client Side")]
        [Comment("Note: Console_Commands Can Be Execute Via Both Console And Chat By (! or css_)")]
        [Comment("Making Both Console_Commands And Chat_Commands Empty = Disable")]
        [String("Console_Commands", "Chat_Commands")]
        public string AntiBodyBlock_CommandsInGame { get; set; } = "Console_Commands: css_noblock,css_nb | Chat_Commands: ";

        [Comment("If [AntiBodyBlock_CommandsInGame] Is Used, Restrict Who Can Use It?")]
        [Comment("Flags Or Group Or SteamID")]
        [Comment("Example:")]
        [Comment("\"SteamIDs: 76561198206086993,STEAM_0:1:507335558 | Flags: @css/root,@css/admin | Groups: #css/root,#css/admin\"")]
        [Comment("\"SteamIDs:  | Flags:  | Groups: \" = To Allow Everyone")]
        [String("SteamIDs", "Flags", "Groups")]
        public string AntiBodyBlock_Flags { get; set; } = "SteamIDs:  | Flags:  | Groups: ";

        [Comment("If [AntiBodyBlock_CommandsInGame] Is Used, Hide Chat After Execute AntiBodyBlock_CommandsInGame?:")]
        [Comment("0 = No")]
        [Comment("1 = Yes, But Only After Execute Successfully")]
        [Comment("2 = Yes, Hide All The Time")]
        [Range(0, 2, 0,
        "AntiBodyBlock_Hide: is invalid, setting to default value (0) Please Choose From 0 To 2." +
        "0 = No\n" +
        "1 = Yes, But Only After Execute Successfully\n" +
        "2 = Yes, Hide All The Time")]
        public int AntiBodyBlock_Hide { get; set; } = 0;

        [Comment("If [AntiBodyBlock_Mode] Not 0, Which Teams Can Phase Through Each Other?")]
        [Comment("1 = Terrorists Only (T Can Go Through T)")]
        [Comment("2 = Counter-Terrorists Only (CT Can Go Through CT)")]
        [Comment("3 = Both Teams Separately (T Through T, CT Through CT, No Cross-Team Phasing)")]
        [Comment("4 = All Players Can Phase Through Any Teammate (CT Through T, T Through CT)")]
        [Range(1, 4, 3,
        "AntiBodyBlock_Teams: invalid value, setting to default (3).\n" +
        "1 = Terrorists Only (T Can Go Through T)\n" +
        "2 = Counter-Terrorists Only (CT Can Go Through CT)\n" +
        "3 = Both Teams Separately (T Through T, CT Through CT, No Cross-Team Phasing)\n" +
        "4 = All Players Can Phase Through Any Teammate (CT Through T, T Through CT)")]
        public int AntiBodyBlock_Teams { get; set; } = 3;

        [Comment("If [AntiBodyBlock_Mode] Not 0, Do You Want Head Boost (Can Stand On Top Of Head)")]
        [Comment("true = Yes")]
        [Comment("false = No")]
        public bool AntiBodyBlock_HeadBoost { get; set; } = false;

        [Comment("If [AntiBodyBlock_Mode = 1], Whats The Default Value Of Client Side Anti-BodyBlock To New Players")]
        [Comment("true = On")]
        [Comment("false = Off")]
        public bool AntiBodyBlock_Mode_1_Default { get; set; } = false;

        [Comment("If [AntiBodyBlock_Mode = 2], How Long (In Secs) The Client Side Anti-BodyBlock Stays Active Then Back To Normal")]
        [Range(1, 999, 10, "AntiBodyBlock_Mode_2_Duration: invalid value, setting to default (10) Choose Between 1 to 999")]
        public int AntiBodyBlock_Mode_2_Duration { get; set; } = 10;

        [Comment("Give Cooldown (In Secs) After Using [AntiBodyBlock_CommandsInGame]")]
        [Comment("0 = No Cooldown")]
        [Range(0, 999, 0, "AntiBodyBlock_Mode_2_Cooldown: invalid value, setting to default (0) Choose Between 0 to 999")]
        public int AntiBodyBlock_Mode_2_Cooldown { get; set; } = 0;

        [Comment("If [AntiBodyBlock_Mode_2_Cooldown] Not 0, Who Is Immune From Cooldown?")]
        [Comment("Flags Or Group Or SteamID")]
        [Comment("Example:")]
        [Comment("\"SteamIDs: 76561198206086993,STEAM_0:1:507335558 | Flags: @css/root,@css/admin | Groups: #css/root,#css/admin\"")]
        [Comment("\"SteamIDs:  | Flags:  | Groups: \" = Everyone Is Immune (Means Disable Cooldown To Everyone)")]
        [String("SteamIDs", "Flags", "Groups")]
        public string AntiBodyBlock_Mode_2_Cooldown_ImmunityFlags { get; set; } = "SteamIDs:  | Flags:  | Groups: ";

        [Comment("Reset [AntiBodyBlock_Mode_2_Cooldown] On Every New Round Start?")]
        [Comment("true = Yes")]
        [Comment("false = No (Make It On Map End)")]
        public bool AntiBodyBlock_Mode_2_ResetCooldownOnNewRound { get; set; } = true;

        [Comment("Maximum Number Of Times A Player Can Use [AntiBodyBlock_CommandsInGame]")]
        [Comment("0 = Unlimited")]
        [Range(0, 999, 0, "AntiBodyBlock_Mode_2_MaxUsage: invalid value, setting to default (0) Choose Between 0 to 999")]
        public int AntiBodyBlock_Mode_2_MaxUsage { get; set; } = 0;

        [Comment("If [AntiBodyBlock_Mode_2_MaxUsage] Not 0, Who Is Immune From Max Usage Limit?")]
        [Comment("Flags Or Group Or SteamID")]
        [Comment("Example:")]
        [Comment("\"SteamIDs: 76561198206086993,STEAM_0:1:507335558 | Flags: @css/root,@css/admin | Groups: #css/root,#css/admin\"")]
        [Comment("\"SteamIDs:  | Flags:  | Groups: \" = Everyone Is Immune (Means Unlimited Usage To Everyone)")]
        [String("SteamIDs", "Flags", "Groups")]
        public string AntiBodyBlock_Mode_2_MaxUsage_ImmunityFlags { get; set; } = "SteamIDs:  | Flags:  | Groups: ";

        [Comment("Reset [AntiBodyBlock_Mode_2_MaxUsage] On Every New Round Start?")]
        [Comment("true = Yes")]
        [Comment("false = No (Make It On Map End)")]
        public bool AntiBodyBlock_Mode_2_ResetMaxUsageOnNewRound { get; set; } = true;
        
    }

    public class MySqlServer
    {
        [Comment("MySQL Server address (hostname or IP)")]
        public string Server { get; set; } = "localhost";

        [Comment("MySQL Server port")]
        public int Port { get; set; } = 3306;

        [Comment("MySQL Database name")]
        public string Database { get; set; } = "MySql_Database";

        [Comment("MySQL Username")]
        public string Username { get; set; } = "MySql_Username";

        [Comment("MySQL Password")]
        public string Password { get; set; } = "MySql_Password";
    }

    public class MySqlConfig
    {
        [Comment("MySQL Servers You Can Add As Many As You like")]
        public List<MySqlServer> MySql_Servers { get; set; } = new List<MySqlServer>
        {
            new MySqlServer
            {
                Server = "localhost",
                Port = 3306,
                Database = "Database",
                Username = "Username",
                Password = "Password"
            },
            new MySqlServer
            {
                Server = "localhost2",
                Port = 3306,
                Database = "Database2",
                Username = "Username2",
                Password = "Password2"
            }
        };
    }

    public class Config
    {
        [BreakLine("----------------------------[ ↓ Plugin Info ↓ ]----------------------------{nextline}")]
        [Info("Version")]
        [Info("Github")]
        public object __InfoSection { get; set; } = null!;

        [BreakLine("----------------------------[ ↓ Main Config ↓ ]----------------------------{nextline}")]

        [Comment("Reload Anti-BodyBlock Plugin")]
        public Reload_AntiBlock Reload_AntiBlock { get; set; } = new();

        [BreakLine("----------------------------[ ↓ Anti-NadeBlock Config ↓ ]----------------------------{nextline}")]

        [Comment("Use Anti-NadeBlock Filter")]
        [Comment("0 = Disabled (Default CS2 Behavior: Pass Teammates, Bounce Enemies)")]
        [Comment("1 = All Nades Bounce On Teammates And Pass On Enemies")]
        [Comment("2 = All Nades Pass On Teammates And Pass On Enemies")]
        [Comment("3 = All Nades Bounce On Teammates And Bounce On Enemies")]
        [Comment("4 = Use Filter AntiNadeBlock_To_Teammates And AntiNadeBlock_To_Enemies")]
        [Range(0, 4, 0,
        "AntiNadeBlock_Enable: Is Invalid, Setting To Default Value (0) Please Choose From 0 To 4.\n" +
        "0 = Disabled (Default CS2 Behavior: Pass Teammates, Bounce Enemies)\n" +
        "1 = All Nades Bounce On Teammates And Pass On Enemies\n" +
        "2 = All Nades Pass On Teammates And Pass On Enemies\n" +
        "3 = All Nades Bounce On Teammates And Bounce On Enemies\n" +
        "4 = Use Filter AntiNadeBlock_To_Teammates And AntiNadeBlock_To_Enemies")]
        public int AntiNadeBlock_Enable { get; set; } = 0;

        [Comment("If [AntiNadeBlock_Enable = 4], How Do You Want Nades Behaves When Throw To Teammate")]
        [Comment("<Nade_Name>:<Pass Or Block>  Example molotov:p Means Molotov Pass When Throw To Teammates")]
        [Comment("Nades Names : [hegrenade] [flashbang] [smokegrenade] [decoy] [molotov] [incendiary]")]
        public List<string> AntiNadeBlock_To_Teammates { get; set; } = new List<string>
        {
            "hegrenade:p",
            "flashbang:b",
            "smokegrenade:b",
            "decoy:p",
            "molotov:b",
            "incendiary:b"
        };

        [Comment("If [AntiNadeBlock_Enable = 4], How Do You Want Nades Behaves When Throw To Enemy")]
        [Comment("<Nade_Name>:<Pass Or Block>  Example molotov:b Means Molotov Block/Bounce When Throw To Enemy")]
        [Comment("Nades Names : [hegrenade] [flashbang] [smokegrenade] [decoy] [molotov] [incendiary]")]
        public List<string> AntiNadeBlock_To_Enemies { get; set; } = new List<string>
        {
            "hegrenade:p",
            "flashbang:b"
        };
        
        [BreakLine("----------------------------[ ↓ Anti-BodyBlock Config ↓ ]----------------------------{nextline}")]
        
        [Comment("Disable Anti-BodyBlock On WarmUp?")]
        [Comment("true = Yes")]
        [Comment("false = No")]
        public bool AntiBodyBlock_DisableOnWarmUp { get; set; } = true;
        
        [Comment("Enable Anti-BodyBlock On Round Start")]
        [Comment("true = Yes")]
        [Comment("false = No")]
        public bool AntiBodyBlock_OnRoundStart { get; set; } = true;

        [Comment("Duration (In Secs) To Keep Anti-BodyBlock Active When Round Start")]
        [Comment("0 = Always Active")]
        public int AntiBodyBlock_OnRoundStartDuration { get; set; } = 10;

        [BreakLine("----------------------------[ ↓ Anti-BodyBlock Client Side Config ↓ ]----------------------------{nextline}")]

        [Comment("Anti-BodyBlock Client Side")]
        public AntiBodyBlock AntiBodyBlock { get; set; } = new();

        [BreakLine("----------------------------[ ↓ Locally Config ↓ ]----------------------------{nextline}")]

        [Comment("Save Players Data By Cookies Locally (In ../plugins/ClientPrefs-GoldKingZ/Anti-Block-GoldKingZ/)?")]
        [Comment("0 = No")]
        [Comment("1 = Yes, But Save Data On Players Disconnect (Warning Performance)")]
        [Comment("2 = Yes, But Save Data On Map Change (Recommended)")]
        [Range(0, 2, 2,
        "Cookies_Enable: is invalid, setting to default value (2) Please Choose From 0 To 2.\n" +
        "0 = No\n" +
        "1 = Yes, But Save Data On Players Disconnect (Warning Performance)\n" +
        "2 = Yes, But Save Data On Map Change (Recommended)")]
        public int Cookies_Enable { get; set; } = 2;

        [Comment("Required [Cookies_Enable = 1/2]")]
        [Comment("Auto Delete Inactive Players More Than X (Days) Old")]
        [Comment("0 = Dont Auto Delete")]
        public int Cookies_AutoRemoveInactivePlayersOlderThanDays { get; set; } = 7;

        [BreakLine("----------------------------[ ↓ MySql Config ↓ ]----------------------------{nextline}")]

        [Comment("Save Players Data Into MySql?")]
        [Comment("0 = No")]
        [Comment("1 = Yes, But Save Data On Players Disconnect (Warning Performance)")]
        [Comment("2 = Yes, But Save Data On Map Change (Recommended)")]
        [Range(0, 2, 0,
        "MySql_Enable: is invalid, setting to default value (0) Please Choose From 0 To 2.\n" +
        "0 = No\n" +
        "1 = Yes, But Save Data On Players Disconnect (Warning Performance)\n" +
        "2 = Yes, But Save Data On Map Change (Recommended)")]
        public int MySql_Enable { get; set; } = 0;

        [Comment("Connection Timeout (In Secs)")]
        [Range(5, 60, 30, "Connection timeout must be between 5 and 60 seconds")]
        public int MySql_ConnectionTimeout { get; set; } = 30;

        [Comment("Retry Attempts When Connection Fails")]
        [Range(1, 5, 3, "Retry attempts must be between 1 and 5")]
        public int MySql_RetryAttempts { get; set; } = 3;

        [Comment("Delay Between Retries (In Secs)")]
        [Range(1, 10, 2, "Retry delay must be between 1 and 10 seconds")]
        public int MySql_RetryDelay { get; set; } = 2;

        [Comment("MySql Config")]
        public MySqlConfig MySql_Config { get; set; } = new MySqlConfig();

        [Comment("Auto Delete Inactive Players More Than X (Days) Old")]
        [Comment("0 = Dont Auto Delete")]
        public int MySql_AutoRemoveInactivePlayersOlderThanDays { get; set; } = 7;

        [BreakLine("----------------------------[ ↓ Utilities  ↓ ]----------------------------{nextline}")]

        [Comment("Auto Update Signatures (In ../plugins/Anti-Block-GoldKingZ/gamedata/gamedata.json)?")]
        [Comment("true = Yes")]
        [Comment("false = No")]
        public bool AutoUpdateSignatures { get; set; } = true;

        [Comment("After SetStateChanged What Method We Use")]
        [Comment("0 = No (Just Use Utilities.SetStateChanged) (Recommended)")]
        [Comment("1 = By CollisionRulesChanged")]
        [Range(0, 1, 0,
        "SetStateChanged: is invalid, setting to default value (0) Please Choose From 0 To 1.\n" +
        "0 = No\n" +
        "1 = By CollisionRulesChanged")]
        public int SetStateChanged { get; set; } = 0;

        [Comment("Use OnConVarChanged Hook On Any ConVar Change")]
        [Comment("true = Yes")]
        [Comment("false = No")]
        public bool UseOnConVarChangedHook { get; set; } = true;

        [Comment("Enable Debug Plugin In Server Console (Helps You To Debug Issues You Facing)?")]
        [Comment("true = Yes")]
        [Comment("false = No")]
        public bool EnableDebug { get; set; } = false;
    }

    public static class Configs
    {
        public static string Version = $"Version : {MainPlugin.Instance.ModuleVersion ?? "Unknown"}";
        public static string Github = "https://github.com/oqyh/cs2-Anti-Block-GoldKingZ";
        public static Config Instance { get; private set; } = new Config();
        static string? filePath;
        static bool IsSimple(Type t) => t.IsPrimitive || t.IsEnum || t == typeof(string) || t == typeof(decimal) || t == typeof(DateTime) || t == typeof(uint);
        static bool IsList(Type t) => t.IsGenericType && t.GetGenericTypeDefinition() == typeof(List<>);

        private static readonly JsonSerializerOptions SimpleValueJsonOptions = new JsonSerializerOptions { WriteIndented = false, Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping };

        public static void Load(string moduleDirectory)
        {
            string configDirectory = Path.Combine(moduleDirectory ?? ".", "config");
            if (!Directory.Exists(configDirectory))
            {
                Directory.CreateDirectory(configDirectory);
            }

            filePath = Path.Combine(configDirectory, "config.json");

            if (!File.Exists(filePath)) { Save(); return; }

            try
            {
                var json = File.ReadAllText(filePath);

                json = RemoveTrailingCommas(json);

                var lines = json.Split('\n').Where(l => !l.TrimStart().StartsWith("//")).ToArray();
                json = string.Join("\n", lines);

                JsonNode? root = null;
                try
                {
                    root = JsonNode.Parse(json);
                }
                catch
                {
                    Instance = new Config();
                    EnsureNestedDefaults(Instance);
                    ValidateStringRecursive(Instance);
                    ValidateRangesRecursive(Instance);
                    ValidateForceStringRecursive(Instance);
                    Save();
                    return;
                }

                if (root is JsonObject rootObj)
                {
                    CleanJsonObjectStrict(rootObj, Instance.GetType());
                }

                string normalizedJson = root?.ToJsonString(new JsonSerializerOptions { WriteIndented = false }) ?? "{}";
                Instance = JsonSerializer.Deserialize<Config>(normalizedJson, new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new Config();
            }
            catch
            {
                Instance = new Config();
            }

            EnsureNestedDefaults(Instance);
            ValidateStringRecursive(Instance);
            ValidateRangesRecursive(Instance);
            ValidateForceStringRecursive(Instance);
            Save();
        }

        public static void Save()
        {
            try
            {
                var path = filePath ?? Path.Combine(".", "config", "config.json");
                string? directory = Path.GetDirectoryName(path);

                if (!string.IsNullOrEmpty(directory) && !Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }

                var props = typeof(Config).GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .Where(p => p.CanRead && p.CanWrite && p.GetIndexParameters().Length == 0);

                var rendered = new List<string>();
                foreach (var p in props)
                {
                    try
                    {
                        rendered.Add(RenderProperty(p, Instance, 2));
                    }
                    catch
                    {
                    }
                }

                string JoinJsonProperties(List<string> propsList)
                {
                    var filtered = propsList.Where(r => !string.IsNullOrWhiteSpace(r)).ToList();
                    var result = new List<string>();

                    bool BlockContainsProperty(string block)
                    {
                        return block
                            .Split('\n')
                            .Any(line =>
                            {
                                var t = line.TrimStart();
                                return t.StartsWith("\"") && t.Contains("\":");
                            });
                    }

                    for (int i = 0; i < filtered.Count; i++)
                    {
                        var current = filtered[i];
                        bool isCurrentPropertyBlock = BlockContainsProperty(current);

                        int nextPropIndex = -1;
                        for (int j = i + 1; j < filtered.Count; j++)
                        {
                            if (BlockContainsProperty(filtered[j]))
                            {
                                nextPropIndex = j;
                                break;
                            }
                        }

                        bool hasNextProp = nextPropIndex != -1;
                        if (isCurrentPropertyBlock && hasNextProp)
                        {
                            if (!current.TrimEnd().EndsWith(","))
                            {
                                current += ",";
                            }
                        }

                        result.Add(current);
                    }

                    return string.Join("\n\n", result);
                }

                var body = JoinJsonProperties(rendered);
                var final = "{\n" + body + "\n}\n";
                File.WriteAllText(path, final);
            }
            catch
            {
            }
        }

        static void EnsureNestedDefaults(object? obj)
        {
            if (obj == null) return;

            var props = obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanRead && p.CanWrite && p.GetIndexParameters().Length == 0);

            foreach (var p in props)
            {
                if (p.GetCustomAttributes<InfoAttribute>().Any()) continue;

                if (IsList(p.PropertyType)) continue;

                if (p.PropertyType == typeof(string) || p.PropertyType.IsValueType) continue;

                try
                {
                    var val = p.GetValue(obj);
                    if (val == null)
                    {
                        try
                        {
                            var inst = Activator.CreateInstance(p.PropertyType);
                            if (inst != null) p.SetValue(obj, inst);
                        }
                        catch { }
                    }
                    EnsureNestedDefaults(p.GetValue(obj));
                }
                catch (TargetParameterCountException)
                {
                    continue;
                }
                catch
                {
                }
            }
        }

        public static void ValidateStringRecursive(object? obj)
        {
            if (obj == null) return;

            var props = obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanRead && p.CanWrite && p.GetIndexParameters().Length == 0);

            foreach (var prop in props)
            {
                if (prop.GetCustomAttributes<InfoAttribute>().Any() && obj is Config) continue;

                if (prop.PropertyType == typeof(string))
                {
                    if (prop.GetCustomAttribute<StringAttribute>() is StringAttribute attr)
                    {
                        try
                        {
                            var current = prop.GetValue(obj) as string;
                            prop.SetValue(obj, string.Join(" | ", attr.Keys.Select(key =>
                                $"{key}: {GetStringValue(current, key)}")));
                        }
                        catch
                        {
                        }
                    }
                }
                else if (!IsSimple(prop.PropertyType) && !IsList(prop.PropertyType))
                {
                    try
                    {
                        ValidateStringRecursive(prop.GetValue(obj));
                    }
                    catch (TargetParameterCountException)
                    {
                        continue;
                    }
                }
            }
        }

        public static string GetStringValue(string? input, string key)
        {
            if (string.IsNullOrWhiteSpace(input)) return "";

            return input.Split('|')
                .Select(segment => segment.Trim())
                .FirstOrDefault(segment => segment.StartsWith(key + ":", StringComparison.OrdinalIgnoreCase))?
                .Substring(key.Length + 1)
                .Trim() ?? "";
        }

        static void ValidateRangesRecursive(object? obj)
        {
            if (obj == null) return;

            var props = obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanRead && p.CanWrite && p.GetIndexParameters().Length == 0);

            foreach (var p in props)
            {
                if (p.GetCustomAttributes<InfoAttribute>().Any()) continue;

                try
                {
                    var range = p.GetCustomAttribute<RangeAttribute>();
                    var val = p.GetValue(obj);
                    if (range != null && val != null)
                    {
                        if (double.TryParse(Convert.ToString(val), out double d))
                        {
                            if (d < range.Min || d > range.Max)
                            {
                                if (!string.IsNullOrEmpty(range.Message))
                                {
                                    var messageLines = range.Message.Replace("\\n", "\n").Split('\n');
                                    foreach (var line in messageLines)
                                    {
                                        if (!string.IsNullOrWhiteSpace(line))
                                        {
                                            Helper.DebugMessage(line.Trim(), true);
                                        }
                                    }
                                }
                                p.SetValue(obj, Convert.ChangeType(range.Default, p.PropertyType));
                            }
                        }
                    }
                    if (!IsSimple(p.PropertyType) && !IsList(p.PropertyType))
                    {
                        ValidateRangesRecursive(p.GetValue(obj));
                    }
                }
                catch (TargetParameterCountException)
                {
                    continue;
                }
                catch
                {
                }
            }
        }

        static void ValidateForceStringRecursive(object? obj)
        {
            if (obj == null) return;

            var props = obj.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(p => p.CanRead && p.CanWrite && p.GetIndexParameters().Length == 0);

            foreach (var prop in props)
            {
                if (prop.GetCustomAttributes<InfoAttribute>().Any()) continue;

                try
                {
                    if (prop.PropertyType == typeof(string))
                    {
                        if (prop.GetCustomAttribute<ForceStringAttribute>() is ForceStringAttribute forceAttr)
                        {
                            var current = prop.GetValue(obj) as string;
                            if (string.IsNullOrWhiteSpace(current))
                            {
                                prop.SetValue(obj, forceAttr.FallbackValue);
                            }
                        }
                    }
                    else if (!IsSimple(prop.PropertyType) && !IsList(prop.PropertyType))
                    {
                        ValidateForceStringRecursive(prop.GetValue(obj));
                    }
                }
                catch (TargetParameterCountException)
                {
                    continue;
                }
                catch
                {
                }
            }
        }

        static IEnumerable<string> RenderCommentLines(string? text, string pad)
        {
            if (string.IsNullOrWhiteSpace(text)) yield break;
            var lines = text.Replace("\r", "").Split('\n');
            foreach (var raw in lines)
            {
                var t = raw.TrimEnd();
                if (string.IsNullOrWhiteSpace(t))
                {
                    yield return pad + "//";
                }
                else if (t == "{nextline}")
                {
                    yield return "";
                }
                else
                {
                    yield return pad + "// " + t;
                }
            }
        }
        private static string RemoveTrailingCommas(string json)
        {
            json = System.Text.RegularExpressions.Regex.Replace(json, @",(\s*[]])", "$1");
            json = System.Text.RegularExpressions.Regex.Replace(json, @",(\s*[}])", "$1");
            return json;
        }

        static string RenderProperty(PropertyInfo p, object? instance, int indent)
        {
            var pad = new string(' ', indent);
            var parts = new List<string>();

            try
            {
                var br = p.GetCustomAttribute<BreakLineAttribute>();
                if (br != null)
                {
                    var txt = br.Text ?? "";

                    bool emptyLineBefore = txt.StartsWith("{nextline}");
                    bool emptyLineAfter = txt.EndsWith("{nextline}");

                    if (emptyLineBefore) txt = txt.Substring("{nextline}".Length);
                    if (emptyLineAfter) txt = txt.Substring(0, txt.Length - "{nextline}".Length);

                    txt = txt.Trim();

                    if (emptyLineBefore)
                        parts.Add(string.Empty);

                    foreach (var line in RenderCommentLines(txt, pad))
                        parts.Add(line);

                    if (emptyLineAfter)
                        parts.Add(string.Empty);
                }

                var infos = p.GetCustomAttributes<InfoAttribute>();
                foreach (var info in infos)
                {
                    string text = info.Key switch
                    {
                        "Version" => Version,
                        "Github" => Github,
                        _ => info.Key
                    };
                    foreach (var line in RenderCommentLines(text, pad))
                        parts.Add(line);
                }

                var comments = p.GetCustomAttributes<CommentAttribute>();
                foreach (var comment in comments)
                {
                    foreach (var line in RenderCommentLines(comment.Text, pad))
                        parts.Add(line);
                }

                if (p.GetCustomAttributes<InfoAttribute>().Any() && (p.PropertyType == typeof(object) || p.PropertyType == typeof(void)))
                    return string.Join("\n", parts);

                var val = p.GetValue(instance);
                if (IsSimple(p.PropertyType))
                {
                    var jsonVal = JsonSerializer.Serialize(val, SimpleValueJsonOptions);
                    parts.Add(pad + $"\"{p.Name}\": {jsonVal}");
                }
                else if (IsList(p.PropertyType))
                {
                    parts.Add(pad + $"\"{p.Name}\":");
                    parts.Add(pad + "[");

                    if (val is System.Collections.IList list && list.Count > 0)
                    {
                        var listItems = new List<string>();
                        var elementType = p.PropertyType.GetGenericArguments()[0];

                        if (IsSimple(elementType))
                        {
                            foreach (var item in list)
                            {
                                if (item != null)
                                {
                                    var jsonVal = JsonSerializer.Serialize(item, SimpleValueJsonOptions);
                                    listItems.Add(pad + "  " + jsonVal);
                                }
                            }
                            parts.Add(string.Join(",\n", listItems));
                        }
                        else
                        {
                            foreach (var item in list)
                            {
                                if (item != null)
                                {
                                    var itemProps = item.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                                        .Where(ip => ip.CanRead && ip.GetIndexParameters().Length == 0);
                                    var itemLines = new List<string>();
                                    foreach (var ip in itemProps)
                                    {
                                        try
                                        {
                                            itemLines.Add(RenderProperty(ip, item, indent + 4));
                                        }
                                        catch
                                        {
                                        }
                                    }
                                    var filtered = itemLines.Where(s => !string.IsNullOrWhiteSpace(s)).ToList();
                                    var itemJson = string.Join(",\n", filtered);
                                    listItems.Add(pad + "  {\n" + itemJson + "\n" + pad + "  }");
                                }
                            }
                            parts.Add(string.Join(",\n", listItems));
                        }
                    }

                    parts.Add(pad + "]");
                }
                else
                {
                    if (val == null)
                    {
                        parts.Add(pad + $"\"{p.Name}\": null");
                    }
                    else
                    {
                        parts.Add(pad + $"\"{p.Name}\":");
                        parts.Add(pad + "{");

                        var innerProps = p.PropertyType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                            .Where(ip => ip.CanRead && ip.GetIndexParameters().Length == 0);

                        var innerLines = new List<string>();
                        foreach (var ip in innerProps)
                        {
                            try
                            {
                                innerLines.Add(RenderProperty(ip, val, indent + 2));
                            }
                            catch
                            {
                            }
                        }

                        var filtered = innerLines.Where(s => !string.IsNullOrWhiteSpace(s)).ToList();
                        var innerJoined = string.Join(",\n\n", filtered);
                        if (!string.IsNullOrEmpty(innerJoined)) parts.Add(innerJoined);

                        parts.Add(pad + "}");
                    }
                }
            }
            catch
            {
                return "";
            }

            return string.Join("\n", parts);
        }

        static void CleanJsonObjectStrict(JsonObject jsonObj, Type targetType)
        {
            if (jsonObj == null || targetType == null) return;

            var props = targetType.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                .Where(p => !p.GetCustomAttributes<InfoAttribute>().Any() && p.GetIndexParameters().Length == 0)
                                .ToList();
            var map = props.ToDictionary(p => p.Name.ToLowerInvariant(), p => p);

            foreach (var key in jsonObj.Select(kv => kv.Key).ToList())
            {
                var keyLower = key.ToLowerInvariant();
                if (!map.TryGetValue(keyLower, out var prop))
                {
                    jsonObj.Remove(key);
                    continue;
                }

                var expectedType = prop.PropertyType;
                var underlying = Nullable.GetUnderlyingType(expectedType) ?? expectedType;
                var node = jsonObj[key];
                if (node == null) continue;

                if (IsSimple(underlying) || underlying.IsEnum)
                {
                    if (node is JsonValue jv)
                    {
                        if (!JsonValueIsExactType(jv, underlying))
                        {
                            jsonObj.Remove(key);
                        }
                    }
                    else
                    {
                        jsonObj.Remove(key);
                    }
                }
                else if (IsList(underlying))
                {
                    if (!(node is JsonArray))
                    {
                        jsonObj.Remove(key);
                    }
                    else
                    {
                        var array = node.AsArray();
                        var itemType = underlying.GetGenericArguments()[0];

                        for (int i = array.Count - 1; i >= 0; i--)
                        {
                            var item = array[i];
                            if (IsSimple(itemType))
                            {
                                if (!(item is JsonValue jvItem) || !JsonValueIsExactType(jvItem, itemType))
                                {
                                    if (itemType == typeof(string) && item is JsonValue jv)
                                    {
                                        try
                                        {
                                            if (jv.TryGetValue<int>(out int intVal))
                                            {
                                                array[i] = JsonValue.Create(intVal.ToString());
                                                continue;
                                            }
                                            if (jv.TryGetValue<double>(out double doubleVal))
                                            {
                                                array[i] = JsonValue.Create(doubleVal.ToString());
                                                continue;
                                            }
                                            if (jv.TryGetValue<long>(out long longVal))
                                            {
                                                array[i] = JsonValue.Create(longVal.ToString());
                                                continue;
                                            }
                                        }
                                        catch
                                        {
                                            array.RemoveAt(i);
                                        }
                                    }
                                    else
                                    {
                                        array.RemoveAt(i);
                                    }
                                }
                            }
                            else if (item is JsonObject itemObj)
                            {
                                CleanJsonObjectStrict(itemObj, itemType);
                            }
                            else if (!(item is JsonObject))
                            {
                                array.RemoveAt(i);
                            }
                        }
                    }
                }
                else
                {
                    if (node is JsonObject childObj)
                    {
                        CleanJsonObjectStrict(childObj, underlying);
                    }
                    else
                    {
                        jsonObj.Remove(key);
                    }
                }
            }
        }

        static bool JsonValueIsExactType(JsonValue jv, Type clrType)
        {
            try
            {
                var t = clrType;
                if (t == typeof(bool))
                {
                    try { jv.GetValue<bool>(); return true; } catch { return false; }
                }

                if (t == typeof(int) || t == typeof(long) || t == typeof(short) || t == typeof(byte) ||
                    t == typeof(uint) || t == typeof(ulong) || t == typeof(ushort) || t == typeof(sbyte))
                {
                    try
                    {
                        jv.GetValue<long>();
                        return true;
                    }
                    catch
                    {
                        return false;
                    }
                }

                if (t == typeof(double) || t == typeof(float) || t == typeof(decimal))
                {
                    try { jv.GetValue<double>(); return true; } catch { return false; }
                }

                if (t == typeof(string))
                {
                    try { jv.GetValue<string>(); return true; } catch { return false; }
                }

                if (t == typeof(DateTime))
                {
                    try
                    {
                        var s = jv.GetValue<string>();
                        return DateTime.TryParse(s, CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out _);
                    }
                    catch { return false; }
                }

                if (t.IsEnum)
                {
                    try
                    {
                        var s = jv.GetValue<string>();
                        if (!string.IsNullOrEmpty(s))
                        {
                            var names = Enum.GetNames(t);
                            if (names.Any(n => string.Equals(n, s, StringComparison.OrdinalIgnoreCase)))
                                return true;
                        }
                    }
                    catch { }

                    try
                    {
                        var v = jv.GetValue<long>();
                        return true;
                    }
                    catch { }

                    return false;
                }

                return false;
            }
            catch
            {
                return false;
            }
        }
    }
}