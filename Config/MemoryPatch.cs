using System.Globalization;
using System.Runtime.InteropServices;
using CounterStrikeSharp.API.Core;
using CounterStrikeSharp.API.Modules.Memory;

namespace Anti_Block_GoldKingZ;

using size_t = nuint;

public unsafe class MemoryPatch
{
    [DllImport("libc", EntryPoint = "mprotect")]
    private static extern int MProtect(nint addr, size_t len, int protect);

    [DllImport("kernel32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool VirtualProtect(nint addr, size_t dwSize, nuint newProtect, nuint* oldProtect);

    [Flags]
    public enum MemoryAccess
    {
        Read  = 1 << 0,
        Write = 1 << 1,
        Exec  = 1 << 2
    }

    private readonly string _modulePath;
    private readonly Dictionary<int, byte[]> _oldPattern = new();
    private nint _addr;

    private const int PAGE_READONLY          = 0x02;
    private const int PAGE_READWRITE         = 0x04;
    private const int PAGE_EXECUTE_READ      = 0x20;
    private const int PAGE_EXECUTE_READWRITE = 0x40;
    private const int PAGESIZE               = 4096;

    public MemoryPatch(string? modulePath = null)
    {
        _modulePath = modulePath ?? Addresses.ServerPath;
    }

    public bool IsInitialized => _addr != nint.Zero;

    public bool Init(string signature)
    {
        _addr = NativeAPI.FindSignature(_modulePath, signature);
        return _addr != nint.Zero;
    }

    public bool Apply(string patchSignature, int offset = 0)
    {
        if (!IsInitialized || string.IsNullOrEmpty(patchSignature) || _oldPattern.ContainsKey(offset))
            return false;

        byte[] patch = patchSignature
            .Split(' ', StringSplitOptions.RemoveEmptyEntries)
            .Select(b => Convert.ToByte(b.StartsWith("0x", StringComparison.OrdinalIgnoreCase)
                ? b[2..]
                : b, 16))
            .ToArray();

        byte[] original = new byte[patch.Length];
        for (int i = 0; i < patch.Length; i++)
        {
            original[i] = Read<byte>(offset + i);
            Write(patch[i], offset + i);
        }

        _oldPattern[offset] = original;
        return true;
    }

    public void Restore()
    {
        if (_oldPattern.Count == 0) return;

        foreach (var (offset, pattern) in _oldPattern)
        {
            for (int i = 0; i < pattern.Length; i++)
                Write(pattern[i], offset + i);
        }
        _oldPattern.Clear();
    }

    public void Write<T>(T data, int offset = 0) where T : unmanaged => *GetPtr<T>(offset) = data;
    public T Read<T>(int offset = 0) where T : unmanaged => *GetPtr<T>(offset);

    public T* GetPtr<T>(int offset = 0) where T : unmanaged
    {
        nint addr = _addr + offset;
        SetMemAccess(addr, (size_t)sizeof(T));
        return (T*)addr;
    }

    public static bool SetMemAccess(nint addr, size_t size,
        MemoryAccess access = MemoryAccess.Read | MemoryAccess.Write | MemoryAccess.Exec)
    {
        if (addr == nint.Zero) throw new ArgumentNullException(nameof(addr));

        if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            return MProtect(LALIGN(addr), size + LALDIF(addr), (int)access) == 0;

        nuint* oldProtect = stackalloc nuint[1];
        nuint prot = access switch
        {
            MemoryAccess.Read  => PAGE_READONLY,
            MemoryAccess.Write => PAGE_READWRITE,
            MemoryAccess.Exec  => PAGE_EXECUTE_READ,
            _                  => PAGE_EXECUTE_READWRITE
        };
        return VirtualProtect(addr, size, prot, oldProtect);
    }

    private static nuint LALDIF(nint addr) => (nuint)addr % PAGESIZE;
    private static nint  LALIGN(nint addr) => addr & ~(PAGESIZE - 1);
}