using System.Reflection;
using MySqlConnector;
using Anti_Block_GoldKingZ.Config;

namespace Anti_Block_GoldKingZ;

public class MySqlDataManager
{
    private static readonly Type TableType = typeof(Globals_Static.PersonData);
    private const string TableName = "Anti_Block_PersonData";
    private static readonly object _lock = new();
    private static bool _isSending;
    private static PropertyInfo[] Props() =>TableType.GetProperties(BindingFlags.Public | BindingFlags.Instance);

    private static PropertyInfo PkProp() => Props()[0];
    private static string ToSqlType(PropertyInfo p)
    {
        if (p.Name == PkProp().Name)            return "BIGINT UNSIGNED PRIMARY KEY";
        if (p.PropertyType == typeof(float))    return "FLOAT NOT NULL DEFAULT 0";
        if (p.PropertyType == typeof(double))   return "DOUBLE NOT NULL DEFAULT 0";
        if (p.PropertyType == typeof(int))      return "INT NOT NULL DEFAULT 0";
        if (p.PropertyType == typeof(long))     return "BIGINT NOT NULL DEFAULT 0";
        if (p.PropertyType == typeof(ulong))    return "BIGINT UNSIGNED NOT NULL DEFAULT 0";
        if (p.PropertyType == typeof(bool))     return "TINYINT(1) NOT NULL DEFAULT 0";
        if (p.PropertyType == typeof(DateTime)) return "DATETIME NOT NULL";
        return "VARCHAR(255) NOT NULL DEFAULT ''";
    }

    private static string ToDataType(PropertyInfo p)
    {
        if (p.Name == PkProp().Name)            return "bigint";
        if (p.PropertyType == typeof(float))    return "float";
        if (p.PropertyType == typeof(double))   return "double";
        if (p.PropertyType == typeof(int))      return "int";
        if (p.PropertyType == typeof(long))     return "bigint";
        if (p.PropertyType == typeof(ulong))    return "bigint";
        if (p.PropertyType == typeof(bool))     return "tinyint";
        if (p.PropertyType == typeof(DateTime)) return "datetime";
        return "varchar";
    }

    private static MySqlDbType ToDbType(Type t)
    {
        if (t == typeof(ulong))    return MySqlDbType.UInt64;
        if (t == typeof(long))     return MySqlDbType.Int64;
        if (t == typeof(int))      return MySqlDbType.Int32;
        if (t == typeof(float))    return MySqlDbType.Float;
        if (t == typeof(double))   return MySqlDbType.Double;
        if (t == typeof(bool))     return MySqlDbType.Byte;
        if (t == typeof(DateTime)) return MySqlDbType.DateTime;
        return MySqlDbType.VarChar;
    }

    private static object ReadColumn(MySqlDataReader r, PropertyInfo p)
    {
        if (p.PropertyType == typeof(ulong))    return r.GetUInt64(p.Name);
        if (p.PropertyType == typeof(long))     return r.GetInt64(p.Name);
        if (p.PropertyType == typeof(int))      return r.GetInt32(p.Name);
        if (p.PropertyType == typeof(float))    return r.GetFloat(p.Name);
        if (p.PropertyType == typeof(double))   return r.GetDouble(p.Name);
        if (p.PropertyType == typeof(bool))     return r.GetBoolean(p.Name);
        if (p.PropertyType == typeof(DateTime)) return r.GetDateTime(p.Name);
        return r.IsDBNull(r.GetOrdinal(p.Name)) ? string.Empty : r.GetString(p.Name);
    }

    public static async Task<MySqlConnection?> GetConnectionAsync()
    {
        var config = Configs.Instance.MySql_Config;
        if (config.MySql_Servers == null || config.MySql_Servers.Count == 0) return null;

        for (int attempt = 0; attempt < Configs.Instance.MySql_RetryAttempts; attempt++)
        {
            if (attempt > 0) await Task.Delay(TimeSpan.FromSeconds(Configs.Instance.MySql_RetryDelay));

            foreach (var s in config.MySql_Servers)
            {
                try
                {
                    var conn = new MySqlConnection(new MySqlConnectionStringBuilder
                    {
                        Server = s.Server, Port = (uint)s.Port, Database = s.Database,
                        UserID = s.Username, Password = s.Password,
                        ConnectionTimeout = (uint)Configs.Instance.MySql_ConnectionTimeout,
                        Pooling = true, MinimumPoolSize = 0, MaximumPoolSize = 100,
                    }.ConnectionString);

                    using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(Configs.Instance.MySql_ConnectionTimeout));
                    await conn.OpenAsync(cts.Token);
                    return conn;
                }
                catch (Exception ex) { Helper.DebugMessage($"MySQL connect failed {s.Server}:{s.Port} - {ex.Message}", true); }
            }
        }
        return null;
    }

    public static async Task<bool> CreateTableIfNotExistsAsync()
    {
        lock (_lock) { if (_isSending) return false; _isSending = true; }
        try
        {
            await using var conn = await GetConnectionAsync();
            if (conn == null) return false;

            await using var checkCmd = new MySqlCommand("SELECT COUNT(*) FROM information_schema.tables WHERE table_schema = DATABASE() AND table_name = @t", conn);
            checkCmd.Parameters.AddWithValue("@t", TableName);
            bool exists = Convert.ToInt32(await checkCmd.ExecuteScalarAsync()) > 0;

            if (!exists)
            {
                var cols = Props().Select(p => $"{p.Name} {ToSqlType(p)}");
                await using var cmd = new MySqlCommand($"CREATE TABLE {TableName} ({string.Join(", ", cols)})", conn);
                await cmd.ExecuteNonQueryAsync();
                Helper.DebugMessage("Table created.");
            }
            else
            {
                await MigrateAsync(conn);
            }
            return true;
        }
        catch (Exception ex) { Helper.DebugMessage($"Init error: {ex.Message}", true); return false; }
        finally { lock (_lock) { _isSending = false; } }
    }

    private static async Task MigrateAsync(MySqlConnection conn)
    {
        var existing = new Dictionary<string, string>();
        await using (var cmd = new MySqlCommand("SELECT COLUMN_NAME, DATA_TYPE FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_SCHEMA = DATABASE() AND TABLE_NAME = @t", conn))
        {
            cmd.Parameters.AddWithValue("@t", TableName);
            await using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
                existing[reader.GetString(0)] = reader.GetString(1).ToLower();
        }

        var props  = Props();
        var pkName = PkProp().Name;

        foreach (var col in existing.Keys.Where(k => k != pkName).ToList())
        {
            var prop = props.FirstOrDefault(p => p.Name == col);
            if (prop == null || existing[col] != ToDataType(prop))
            {
                await using var cmd = new MySqlCommand($"ALTER TABLE {TableName} DROP COLUMN {col}", conn);
                await cmd.ExecuteNonQueryAsync();
                existing.Remove(col);
                Helper.DebugMessage($"Migration: dropped '{col}'.");
            }
        }

        string? prev = null;
        foreach (var p in props)
        {
            if (!existing.ContainsKey(p.Name) && p.Name != pkName)
            {
                var after = prev != null ? $"AFTER {prev}" : "FIRST";
                await using var cmd = new MySqlCommand($"ALTER TABLE {TableName} ADD COLUMN {p.Name} {ToSqlType(p)} {after}", conn);
                await cmd.ExecuteNonQueryAsync();
                Helper.DebugMessage($"Migration: added '{p.Name}'.");
            }
            prev = p.Name;
        }
    }
    
    public static async Task<bool> SaveToMySqlAsync(Globals_Static.PersonData data)
    {
        foreach (var p in Props())
            if (p.PropertyType == typeof(string) && p.GetValue(data) == null)
                p.SetValue(data, string.Empty);

        var props  = Props();
        var pkName = PkProp().Name;
        var cols   = string.Join(", ", props.Select(p => p.Name));
        var parms  = string.Join(", ", props.Select(p => "@" + p.Name));
        var update = string.Join(", ", props.Where(p => p.Name != pkName).Select(p => $"{p.Name} = VALUES({p.Name})"));

        try
        {
            await using var conn = await GetConnectionAsync();
            if (conn == null) return false;

            await using var cmd = new MySqlCommand(
                $"INSERT INTO {TableName} ({cols}) VALUES ({parms}) ON DUPLICATE KEY UPDATE {update}", conn);

            foreach (var p in props)
                cmd.Parameters.Add("@" + p.Name, ToDbType(p.PropertyType)).Value = p.GetValue(data)!;

            await cmd.ExecuteNonQueryAsync();
            return true;
        }
        catch (Exception ex) { Helper.DebugMessage($"Save error: {ex.Message}", true); return false; }
    }

    public static async Task<Globals_Static.PersonData> RetrievePersonDataByIdAsync(ulong steamId)
    {
        try
        {
            await using var conn = await GetConnectionAsync();
            if (conn == null) return new();

            await using var cmd = new MySqlCommand($"SELECT * FROM {TableName} WHERE {PkProp().Name} = @id", conn);
            cmd.Parameters.Add("@id", MySqlDbType.UInt64).Value = steamId;

            await using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                var obj = Activator.CreateInstance<Globals_Static.PersonData>();
                foreach (var p in Props())
                    try { p.SetValue(obj, ReadColumn(reader, p)); } catch { }
                return obj;
            }
        }
        catch (Exception ex) { Helper.DebugMessage($"Retrieve error: {ex.Message}", true); }
        return new();
    }

    public static async Task<bool> DeleteOldPlayersAsync()
    {
        int days = Configs.Instance.MySql_AutoRemovePlayerOlderThanXDays;
        if (days < 1) return false;

        var dateProp = Props().FirstOrDefault(p => p.PropertyType == typeof(DateTime))?.Name ?? "DateAndTime";
        try
        {
            await using var conn = await GetConnectionAsync();
            if (conn == null) return false;

            await using var cmd = new MySqlCommand($"DELETE FROM {TableName} WHERE {dateProp} < NOW() - INTERVAL @Days DAY", conn);
            cmd.Parameters.Add("@Days", MySqlDbType.Int32).Value = days;
            await cmd.ExecuteNonQueryAsync();
            return true;
        }
        catch (Exception ex) { Helper.DebugMessage($"DeleteOldPlayers error: {ex.Message}", true); return false; }
    }
}