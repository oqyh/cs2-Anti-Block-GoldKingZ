using System.Runtime.InteropServices;

namespace Anti_Block_GoldKingZ;

public static class Con
{
    public const string Reset         = "\x1b[0m";
    public const string Bold          = "\x1b[1m";
    public const string Dim           = "\x1b[2m";
    public const string Italic        = "\x1b[3m";
    public const string Underline     = "\x1b[4m";
    public const string Blink         = "\x1b[5m";
    public const string Inverse       = "\x1b[7m";
    public const string Strikethrough = "\x1b[9m";

    public static string Fg(int r, int g, int b) => $"\x1b[38;2;{r};{g};{b}m";
    public static string Bg(int r, int g, int b) => $"\x1b[48;2;{r};{g};{b}m";

    // ── Grayscale ─────────────────────────────────────────────
    public static readonly string Black       = Fg(  0,   0,   0);
    public static readonly string DarkGray    = Fg( 80,  80,  80);
    public static readonly string Gray        = Fg(128, 128, 128);
    public static readonly string LightGray   = Fg(192, 192, 192);
    public static readonly string Silver      = Fg(220, 220, 220);
    public static readonly string White       = Fg(255, 255, 255);

    // ── Red ───────────────────────────────────────────────────
    public static readonly string DarkRed     = Fg(139,   0,   0);
    public static readonly string Red         = Fg(231,  72,  86);
    public static readonly string LightRed    = Fg(255, 102, 102);
    public static readonly string Crimson     = Fg(220,  20,  60);
    public static readonly string Maroon      = Fg(128,   0,   0);
    public static readonly string Salmon      = Fg(250, 128, 114);
    public static readonly string Coral       = Fg(255, 127,  80);
    public static readonly string Tomato      = Fg(255,  99,  71);
    public static readonly string Brick       = Fg(178,  34,  34);

    // ── Orange ────────────────────────────────────────────────
    public static readonly string DarkOrange  = Fg(204,  85,   0);
    public static readonly string Orange      = Fg(255, 165,   0);
    public static readonly string LightOrange = Fg(255, 200, 100);
    public static readonly string Amber       = Fg(255, 191,   0);
    public static readonly string Peach       = Fg(255, 218, 185);
    public static readonly string Tangerine   = Fg(242, 133,   0);

    // ── Yellow / Gold ─────────────────────────────────────────
    public static readonly string DarkYellow  = Fg(180, 160,  20);
    public static readonly string Yellow      = Fg(249, 241, 165);
    public static readonly string BrightYellow= Fg(255, 255,   0);
    public static readonly string Gold        = Fg(255, 215,   0);
    public static readonly string GoldKingZ   = Fg(255, 195,   0);
    public static readonly string Mustard     = Fg(255, 219,  88);
    public static readonly string Khaki       = Fg(240, 230, 140);
    public static readonly string Cream       = Fg(255, 253, 208);

    // ── Green ─────────────────────────────────────────────────
    public static readonly string DarkGreen   = Fg(  0, 100,   0);
    public static readonly string Green       = Fg( 22, 198,  12);
    public static readonly string LightGreen  = Fg(144, 238, 144);
    public static readonly string Lime        = Fg(180, 255,   0);
    public static readonly string Mint        = Fg(152, 255, 152);
    public static readonly string Forest      = Fg( 34, 139,  34);
    public static readonly string Olive       = Fg(128, 128,   0);
    public static readonly string Emerald     = Fg( 80, 200, 120);
    public static readonly string Sea         = Fg( 46, 139,  87);

    // ── Cyan / Teal ───────────────────────────────────────────
    public static readonly string DarkCyan    = Fg(  0, 139, 139);
    public static readonly string Cyan        = Fg( 97, 214, 214);
    public static readonly string LightCyan   = Fg(224, 255, 255);
    public static readonly string Teal        = Fg(  0, 128, 128);
    public static readonly string Turquoise   = Fg( 64, 224, 208);
    public static readonly string Aqua        = Fg(  0, 255, 255);

    // ── Blue ──────────────────────────────────────────────────
    public static readonly string DarkBlue    = Fg(  0,   0, 139);
    public static readonly string Blue        = Fg( 59, 120, 255);
    public static readonly string LightBlue   = Fg(173, 216, 230);
    public static readonly string Navy        = Fg(  0,   0, 128);
    public static readonly string Royal       = Fg( 65, 105, 225);
    public static readonly string Sky         = Fg(135, 206, 235);
    public static readonly string Steel       = Fg( 70, 130, 180);
    public static readonly string Cobalt      = Fg(  0,  71, 171);
    public static readonly string Cornflower  = Fg(100, 149, 237);

    // ── Purple / Magenta ──────────────────────────────────────
    public static readonly string DarkMagenta = Fg(139,   0, 139);
    public static readonly string Magenta     = Fg(180,   0, 158);
    public static readonly string LightMagenta= Fg(255, 119, 255);
    public static readonly string Purple      = Fg(128,   0, 128);
    public static readonly string Violet      = Fg(143,   0, 255);
    public static readonly string Indigo      = Fg( 75,   0, 130);
    public static readonly string Lavender    = Fg(200, 162, 255);
    public static readonly string Plum        = Fg(221, 160, 221);
    public static readonly string Orchid      = Fg(218, 112, 214);

    // ── Pink ──────────────────────────────────────────────────
    public static readonly string Pink        = Fg(255, 192, 203);
    public static readonly string HotPink     = Fg(255, 105, 180);
    public static readonly string DeepPink    = Fg(255,  20, 147);
    public static readonly string Rose        = Fg(255,   0, 127);
    public static readonly string Fuchsia     = Fg(255,   0, 255);

    // ── Brown / Earth ─────────────────────────────────────────
    public static readonly string Brown       = Fg(139,  69,  19);
    public static readonly string DarkBrown   = Fg( 92,  64,  51);
    public static readonly string LightBrown  = Fg(181, 101,  29);
    public static readonly string Tan         = Fg(210, 180, 140);
    public static readonly string Beige       = Fg(245, 245, 220);
    public static readonly string Sand        = Fg(194, 178, 128);
    public static readonly string Chocolate   = Fg(210, 105,  30);
    public static readonly string Sienna      = Fg(160,  82,  45);

    [DllImport("kernel32.dll")] private static extern bool GetConsoleMode(IntPtr h, out uint m);
    [DllImport("kernel32.dll")] private static extern bool SetConsoleMode(IntPtr h, uint m);
    [DllImport("kernel32.dll")] private static extern IntPtr GetStdHandle(int n);

    public static void EnableOnWindows()
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) return;
        var h = GetStdHandle(-11);
        if (GetConsoleMode(h, out uint mode))
            SetConsoleMode(h, mode | 0x0004);
    }
    public static void ResetAll()
    {
        Console.Write(Reset);
        Console.ResetColor();
    }
    public static void WriteLine(string text)
    {
        Console.WriteLine(text + Reset);
        Console.ResetColor();
    }
}