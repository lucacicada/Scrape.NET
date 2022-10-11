// namespace System.Net.Http;
namespace Scrape.NET;

using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

/// <inheritdoc />
public class FileStreamContent : StreamContent
{
    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private readonly FileStream fileStream;

    [DebuggerBrowsable(DebuggerBrowsableState.Never)]
    private readonly bool deleteOnDispose;

    /// <summary>
    ///     Gets the absolute path of the file opened in the <see cref="FileStreamContent"/>.
    /// </summary>
    public string FileName => fileStream.Name;

    /// <summary>
    ///     Gets the length in bytes of the stream opened in the <see cref="FileStreamContent"/>.
    /// </summary>
    public long Length => fileStream.Length;

    /// <summary>
    ///     Creates a new instance of the <see cref="FileStreamContent"/> class.
    /// </summary>
    public FileStreamContent(string file, bool deleteOnDispose = true)
        : this(new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read), deleteOnDispose) { }

    /// <summary>
    ///     Creates a new instance of the <see cref="FileStreamContent"/> class.
    /// </summary>
    public FileStreamContent(string file, int bufferSize, bool deleteOnDispose = true)
        : this(new FileStream(file, FileMode.Open, FileAccess.Read, FileShare.Read, bufferSize), bufferSize, deleteOnDispose) { }

    private FileStreamContent(FileStream fileStream, bool deleteOnDispose)
        : base(fileStream)
    {
        this.fileStream = fileStream;
        this.deleteOnDispose = deleteOnDispose;
    }

    private FileStreamContent(FileStream fileStream, int bufferSize, bool deleteOnDispose)
        : base(fileStream, bufferSize)
    {
        this.fileStream = fileStream;
        this.deleteOnDispose = deleteOnDispose;
    }

    /// <inheritdoc />
    protected override void SerializeToStream(Stream stream, TransportContext? context, CancellationToken cancellationToken)
    {
        Debug.Assert(stream is not MemoryStream, $"Do not use ${nameof(LoadIntoBufferAsync)} on ${nameof(FileStreamContent)}");

        base.SerializeToStream(stream, context, cancellationToken);
    }

    /// <inheritdoc />
    protected override Task SerializeToStreamAsync(Stream stream, TransportContext? context)
    {
        Debug.Assert(stream is not MemoryStream, $"Do not use ${nameof(LoadIntoBufferAsync)} on ${nameof(FileStreamContent)}");

        return base.SerializeToStreamAsync(stream, context);
    }

    /// <inheritdoc />
    protected override Task SerializeToStreamAsync(Stream stream, TransportContext? context, CancellationToken cancellationToken)
    {
        Debug.Assert(stream is not MemoryStream, $"Do not use ${nameof(LoadIntoBufferAsync)} on ${nameof(FileStreamContent)}");

        return base.SerializeToStreamAsync(stream, context, cancellationToken);
    }

    /// <inheritdoc />
    protected override bool TryComputeLength(out long length)
    {
        length = fileStream.Length;

        return true;
    }

    /// <inheritdoc />
    protected override void Dispose(bool disposing)
    {
        // this will dispose the fileStream
        base.Dispose(disposing);

        if (disposing && deleteOnDispose)
        {
            // always check with File.Exists, if the subfolder is deleted,
            // File.Delete will throw DirectoryNotFoundException
            if (File.Exists(FileName))
            {
                try
                {
                    File.Delete(FileName);
                }
                catch (DirectoryNotFoundException)
                {
                    //
                }
                catch (UnauthorizedAccessException)
                {
                    //
                }
            }
        }
    }
}
