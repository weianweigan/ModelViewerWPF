using System;
using System.IO;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("ModelViewerWPFTests")]

namespace ModelViewerWPF.Utils;

internal static class UriUtils
{
    public static string GenerateRandomFile(this Uri uri)
    {
        if (uri == null)
            throw new ArgumentNullException(nameof(uri));

        return Path.ChangeExtension(
            Path.GetRandomFileName(),
            Path.GetExtension(uri.OriginalString)
        );
    }
}
