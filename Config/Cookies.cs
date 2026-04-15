using System.Collections.Concurrent;
using System.Reflection;
using LiteDB;
using Anti_Block_GoldKingZ.Config;

namespace Anti_Block_GoldKingZ;

public class Cookies
{
    private const string TableName = "PlayerCookies";
    private static string DbPath => Path.Combine(MainPlugin.Instance.ModuleDirectory, "cookies", "cookies.db");
    private static readonly Type TableType = typeof(Globals_Static.PersonData);
    private static readonly ConcurrentDictionary<ulong, Globals_Static.PersonData> _cache = new();
    private static readonly SemaphoreSlim _dbLock = new(1, 1);
    private static LiteDatabase? _db;
    private static ILiteCollection<BsonDocument>? _col;

    public static void InitializeDatabase()
    {
        Directory.CreateDirectory(Path.GetDirectoryName(DbPath)!);
        _db  = new LiteDatabase($"Filename={DbPath};Connection=shared;");
        _col = _db.GetCollection(TableName);
        _col.EnsureIndex("_id", true);

        foreach (var doc in _col.FindAll().ToList())
        {
            MigrateDoc(doc);
            try { var d = FromDoc(doc); _cache[PkValue(d)] = d; } catch { }
        }

        Helper.DebugMessage($"Ready. {_cache.Count} players loaded.");
    }

    public static void Dispose()
    {
        _db?.Dispose();
        _db = null; _col = null;
    }

    public static Globals_Static.PersonData? GetPlayerData(ulong steamId) =>
        _cache.TryGetValue(steamId, out var d) ? d : null;

    public static async Task SaveAsync(Globals_Static.PersonData data)
    {
        foreach (var p in Props())
            if (p.PropertyType == typeof(string) && p.GetValue(data) == null)
                p.SetValue(data, string.Empty);

        _cache[PkValue(data)] = data;
        await _dbLock.WaitAsync();
        try   { await Task.Run(() => _col!.Upsert(ToDoc(data))); }
        catch (Exception ex) { Helper.DebugMessage($"SaveAsync error: {ex.Message}", true); }
        finally { _dbLock.Release(); }
    }

    public static async Task RemoveOldEntriesAsync()
    {
        int days = Configs.Instance.Cookies_AutoRemovePlayerOlderThanXDays;
        if (days < 1) return;

        var cutoff   = DateTime.Now.AddDays(-days);
        var dateProp = Props().FirstOrDefault(p => p.PropertyType == typeof(DateTime))?.Name ?? "DateAndTime";

        var expired = _cache
            .Where(kv => TableType.GetProperty(dateProp)?.GetValue(kv.Value) is DateTime dt && dt < cutoff)
            .Select(kv => kv.Key).ToList();

        expired.ForEach(id => _cache.TryRemove(id, out _));
        if (expired.Count == 0) { Helper.DebugMessage($"No entries older than {days} days."); return; }

        await _dbLock.WaitAsync();
        try
        {
            await Task.Run(() =>
            {
                int removed = _col!.DeleteMany(doc => doc[dateProp] != null && doc[dateProp].AsDateTime < cutoff);
                Helper.DebugMessage($"Removed {removed} inactive players. Cache: {_cache.Count}");
            });
        }
        catch (Exception ex) { Helper.DebugMessage($"RemoveOldEntries error: {ex.Message}", true); }
        finally { _dbLock.Release(); }
    }

    private static void MigrateDoc(BsonDocument doc)
    {
        bool changed = false;
        var pk = PkProp().Name;

        var expected = Props().ToDictionary(
            p => p.Name == pk ? "_id" : p.Name,
            p => ToBson(DefaultValue(p.PropertyType), p.PropertyType).Type);

        foreach (var key in doc.Keys.Where(k => k != "_id").ToList())
        {
            if (!expected.TryGetValue(key, out var expectedType) || doc[key].Type != expectedType)
            { doc.Remove(key); changed = true; }
        }

        foreach (var p in Props())
        {
            var key = p.Name == pk ? "_id" : p.Name;
            if (!doc.ContainsKey(key))
            { doc[key] = ToBson(DefaultValue(p.PropertyType), p.PropertyType); changed = true; }
        }

        if (changed) _col!.Upsert(doc);
    }

    private static PropertyInfo[] Props() =>
        TableType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

    private static PropertyInfo PkProp() => Props()[0];

    private static ulong PkValue(Globals_Static.PersonData d) =>
        (ulong)Convert.ChangeType(PkProp().GetValue(d)!, typeof(ulong));

    private static object DefaultValue(Type type)
    {
        if (type == typeof(string))   return string.Empty;
        if (type == typeof(DateTime)) return DateTime.MinValue;
        return type.IsValueType ? Activator.CreateInstance(type)! : string.Empty;
    }

    private static BsonDocument ToDoc(Globals_Static.PersonData data)
    {
        var doc = new BsonDocument();
        var pk  = PkProp().Name;
        foreach (var p in Props())
        {
            var val = p.GetValue(data) ?? DefaultValue(p.PropertyType);
            doc[p.Name == pk ? "_id" : p.Name] = ToBson(val, p.PropertyType);
        }
        return doc;
    }

    private static Globals_Static.PersonData FromDoc(BsonDocument doc)
    {
        var obj = Activator.CreateInstance<Globals_Static.PersonData>();
        var pk  = PkProp().Name;
        foreach (var p in Props())
        {
            var key = p.Name == pk ? "_id" : p.Name;
            if (doc.ContainsKey(key)) p.SetValue(obj, FromBson(doc[key], p.PropertyType));
        }
        return obj;
    }

    private static BsonValue ToBson(object value, Type type) => value switch
    {
        ulong u    => new BsonValue((long)u),
        float f    => new BsonValue((double)f),
        DateTime d => new BsonValue(d),
        int i      => new BsonValue(i),
        long l     => new BsonValue(l),
        double dbl => new BsonValue(dbl),
        bool b     => new BsonValue(b),
        string s   => new BsonValue(s),
        _          => new BsonValue(value.ToString())
    };

    private static object FromBson(BsonValue value, Type target)
    {
        if (target == typeof(ulong))    return (ulong)value.AsInt64;
        if (target == typeof(long))     return value.AsInt64;
        if (target == typeof(int))      return value.AsInt32;
        if (target == typeof(float))    return (float)value.AsDouble;
        if (target == typeof(double))   return value.AsDouble;
        if (target == typeof(bool))     return value.AsBoolean;
        if (target == typeof(DateTime)) return value.AsDateTime;
        return value.AsString;
    }
}