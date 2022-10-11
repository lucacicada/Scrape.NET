// namespace System;
namespace Scrape.NET;

using System;
using System.Runtime.InteropServices;

partial class ConsoleEx
{
    private const uint ENABLE_QUICK_EDIT = 0x0040;

    private const int STD_INPUT_HANDLE = -10;

    // see: https://docs.microsoft.com/windows/console/getstdhandle
    [DllImport("kernel32.dll", SetLastError = true)]
    private static extern IntPtr GetStdHandle(int nStdHandle);

    [DllImport("kernel32.dll")]
    private static extern bool GetConsoleMode(IntPtr hConsoleHandle, out uint lpMode);

    [DllImport("kernel32.dll")]
    private static extern bool SetConsoleMode(IntPtr hConsoleHandle, uint dwMode);

    /// <summary>
    ///     Enables the user to use the mouse to select and edit text on the console window.
    /// </summary>
    /// <remarks>
    ///     This feature is only available on Windows.
    ///     <see href="https://docs.microsoft.com/en-us/windows/console/setconsolemode"/>
    /// </remarks>
    public static bool EnableQuickEdit(bool value)
    {
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            IntPtr consoleHandle = GetStdHandle(STD_INPUT_HANDLE);

            return GetConsoleMode(consoleHandle, out uint consoleMode) && SetConsoleMode(consoleHandle,
                value
                ? consoleMode | ENABLE_QUICK_EDIT
                : consoleMode & ~ENABLE_QUICK_EDIT);
        }

        return false;
    }
}
