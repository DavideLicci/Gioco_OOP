using System.Runtime.InteropServices;

namespace OltreIlTempo;

/// <summary>
/// Inizializza la console per supportare ANSI/VT (colori e Unicode) su Windows.
/// </summary>
internal static class ConsoleSetup
{
    private const int STD_OUTPUT_HANDLE = -11;
    private const int ENABLE_VIRTUAL_TERMINAL_PROCESSING = 0x0004;

    internal static void Initialize()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        if (!OperatingSystem.IsWindows()) return;

        try
        {
            nint handle = GetStdHandle(STD_OUTPUT_HANDLE);
            if (GetConsoleMode(handle, out int mode))
                SetConsoleMode(handle, mode | ENABLE_VIRTUAL_TERMINAL_PROCESSING);
        }
        catch
        {
            // Se il processo non ha una console (es. redirect) ignora silenziosamente.
        }
    }

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern nint GetStdHandle(int nStdHandle);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool GetConsoleMode(nint hConsoleHandle, out int lpMode);

    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern bool SetConsoleMode(nint hConsoleHandle, int dwMode);
}
