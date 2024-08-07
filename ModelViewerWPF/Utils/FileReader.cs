using System;
using System.IO;
using System.Net.Http;
using System.Windows;
using System.Windows.Resources;

namespace ModelViewerWPF.Utils;

internal static class FileReader
{
    public static MemoryStream ReadFileFromUri(Uri fileUri)
    {
        if (fileUri == null)
        {
            throw new ArgumentNullException(nameof(fileUri));
        }

        if (!fileUri.IsAbsoluteUri)
        {
            string file = Path.Combine(
                Path.GetDirectoryName(typeof(FileReader).Assembly.Location)!,
                fileUri.OriginalString
            );
            if (File.Exists(file))
            {
                fileUri = new Uri($@"file:///{file}");
            }
            else
            {
                string assembly = Application.ResourceAssembly.GetName().Name!;
                string relativeUri = fileUri.OriginalString;
                if (relativeUri.StartsWith("/") || relativeUri.StartsWith(@"\"))
                {
                    relativeUri = relativeUri.Substring(1);
                }
                fileUri = new Uri(
                    $@"pack://application:,,,/{assembly};component/{fileUri.OriginalString}"
                );
            }
        }

        // Determine the scheme and handle accordingly
        switch (fileUri.Scheme.ToLower())
        {
            case "file":
                return ReadLocalFile(fileUri.LocalPath);

            case "http":
            case "https":
                return ReadHttpFile(fileUri);
            case "pack":
                return ReadPackUri(fileUri);

            default:
                throw new NotSupportedException(
                    $"The URI scheme '{fileUri.Scheme}' is not supported."
                );
        }
    }

    private static MemoryStream ReadLocalFile(string localPath)
    {
        if (!File.Exists(localPath))
        {
            throw new FileNotFoundException("The specified file was not found.", localPath);
        }

        return new MemoryStream(File.ReadAllBytes(localPath));
    }

    private static MemoryStream ReadHttpFile(Uri httpUri)
    {
        using HttpClient client = new HttpClient();
        HttpResponseMessage response = client.GetAsync(httpUri).Result;
        response.EnsureSuccessStatusCode();
        using var stream = response.Content.ReadAsStreamAsync().Result;
        var memeoryStream = new MemoryStream();
        stream.CopyTo(memeoryStream);
        return memeoryStream;
    }

    private static MemoryStream ReadPackUri(Uri packUri)
    {
        StreamResourceInfo sri =
            Application.GetResourceStream(packUri)
            ?? throw new FileNotFoundException(
                "The specified pack URI resource was not found.",
                packUri.ToString()
            );

        var memeoryStream = new MemoryStream();
        sri.Stream.CopyTo(memeoryStream);
        sri.Stream.Dispose();
        return memeoryStream;
    }
}
