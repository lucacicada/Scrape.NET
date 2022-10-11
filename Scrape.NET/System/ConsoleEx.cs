// namespace System;
namespace Scrape.NET;

using System;
using System.IO;
using System.Text;
using System.Threading;

/// <summary>
///     Represents a <see cref="Console"/> extension.
/// </summary>
public static partial class ConsoleEx
{
    /// <summary>
    ///     Keep track of concurrent writing.
    /// </summary>
    private static int writing;

    /// <summary>
    ///     Writes the specified string value to the standard output stream over the current line.
    /// </summary>
    /// <param name="value">The value to write.</param>
    /// <exception cref="IOException">An I/O error occurred.</exception>
    public static void ReWrite(string value) => ReWrite(value.AsSpan());

    /// <summary>
    ///     Writes the specified string value to the standard output stream over the current line.
    /// </summary>
    /// <param name="chars">The value to write.</param>
    /// <exception cref="IOException">An I/O error occurred.</exception>
    public static void ReWrite(ReadOnlySpan<char> chars)
    {
        if (Interlocked.Exchange(ref writing, 1) == 0)
        {
            try
            {
                int cursorLeft = Console.CursorLeft;
                int bufferWidth = Console.BufferWidth;

                int printWidth = bufferWidth - 1; // for the null terminator

                // create the string builder
                // value.Length + bufferWidth can be dangerous, value.Length could be huge
                var sb = new StringBuilder(240);

                // backspace a number of times equal to the current buffer width
                sb.Append('\b', bufferWidth);

                // do not print consecutive whitespaces
                bool skipWhiteSpace = false;

                // sanity check, ignore empty strings
                if (chars.Length > 0)
                {
                    // the string builder length cannot exceed (printWidth + bufferWidth)
                    for (int i = 0; i < chars.Length && sb.Length < printWidth + bufferWidth; i++)
                    {
                        char item = chars[i];

                        if (char.IsWhiteSpace(item))
                        {
                            if (!skipWhiteSpace)
                            {
                                // add a whitespace
                                skipWhiteSpace = true;
                                sb.Append(' ');
                            }
                        }
                        else
                        {
                            // add a non whitespace
                            skipWhiteSpace = false;
                            sb.Append(item);
                        }
                    }
                }

                // calculate the pad relative to the cursor position
                int pad = cursorLeft - (sb.Length - bufferWidth);
                if (pad > printWidth) pad = printWidth;

                if (pad > 0)
                {
                    // insert white spaces to clear the current text
                    sb.Append(' ', pad);

                    // backspace to preserve the cursor position
                    sb.Append('\b', pad);
                }

                // print the string
                Console.Out.Write(sb.ToString());

                // flushing does not help
                // Console.Out.Flush();
            }
            finally
            {
                _ = Interlocked.Exchange(ref writing, 0);
            }
        }
    }

    /// <summary>
    ///     Hides the console cursor.
    /// </summary>
    public static IDisposable HideCursor() => new CursorHidden();

    private sealed class CursorHidden : IDisposable
    {
        public CursorHidden() => Console.CursorVisible = false;

        public void Dispose() => Console.CursorVisible = true;
    }
}
