using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Web.WebView2.Core;
using Microsoft.Web.WebView2.Wpf;
using ModelViewerWPF.HtmlSource;
using ModelViewerWPF.Utils;

namespace ModelViewerWPF;

[TemplatePart(Name = PART_WebView2, Type = typeof(WebView2))]
public partial class ModelViewer : Control
{
    private const string DOMAIN = "https://app.local/";
    private const string HOST = "https://app.local/index.html";
    private const string PART_WebView2 = nameof(PART_WebView2);
    private const string PART_Error = nameof(PART_Error);
    private string? _randomModelSourceFile;

    private WebView2? ModelViewWebView2;

    static ModelViewer()
    {
        DefaultStyleKeyProperty.OverrideMetadata(
            typeof(ModelViewer),
            new FrameworkPropertyMetadata(typeof(ModelViewer))
        );
    }

    public bool WebViewNavigationCompleted { get; private set; }

    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        ModelViewWebView2 = (WebView2)GetTemplateChild(PART_WebView2);

        if (ModelViewWebView2 != null)
        {
            InitWebView();
        }
    }

    private async Task InitWebView()
    {
        if (ModelViewWebView2 == null)
        {
            return;
        }

        await Webview2InitializeUtils.InitializeAsync(ModelViewWebView2);

        if (OpenDevToolsWindow)
        {
            ModelViewWebView2.CoreWebView2.OpenDevToolsWindow();
        }

        ModelViewWebView2.CoreWebView2.AddWebResourceRequestedFilter(
            "*://app.local/*",
            CoreWebView2WebResourceContext.All
        );

        ModelViewWebView2.CoreWebView2.WebResourceRequested += WebResourceRequestedHandler;
        ModelViewWebView2.CoreWebView2.NavigationCompleted += NavigationCompletedHandler;
        ModelViewWebView2.CoreWebView2.WebMessageReceived += WebMessageReceivedHandler;

        ModelViewWebView2.CoreWebView2.Navigate(HOST);
    }

    private void WebMessageReceivedHandler(
        object? sender,
        CoreWebView2WebMessageReceivedEventArgs e
    )
    {
        // Parse json.
    }

    private void NavigationCompletedHandler(
        object? sender,
        CoreWebView2NavigationCompletedEventArgs e
    )
    {
        WebViewNavigationCompleted = true;
        if (!e.IsSuccess && DisplayException)
        {
            MessageBox.Show(
                $"{nameof(e.HttpStatusCode)}:{e.HttpStatusCode} {Environment.NewLine}{nameof(e.WebErrorStatus)}:{e.WebErrorStatus}"
            );
        }
    }

    private void WebResourceRequestedHandler(
        object? sender,
        CoreWebView2WebResourceRequestedEventArgs e
    )
    {
        try
        {
            var requestUri = new Uri(e.Request.Uri);

            if (!string.Equals(requestUri.Host, "app.local", StringComparison.OrdinalIgnoreCase))
            {
                return;
            }

            var contentType = GetContentType(requestUri);

            if (requestUri.AbsolutePath == '/' + _randomModelSourceFile)
            {
                e.Response = LoadModelSource(contentType);
            }
            else
            {
                e.Response = LoadResource(requestUri, contentType);
            }
        }
        catch (Exception ex)
        {
            Debug.Print(ex.ToString());
            if (DisplayException)
            {
                MessageBox.Show(
                    ex.ToString(),
                    "Resource error",
                    MessageBoxButton.OK,
                    MessageBoxImage.Error
                );
            }
        }

        CoreWebView2WebResourceResponse LoadResource(Uri requestUri, string contentType)
        {
            string content;
            switch (requestUri.AbsolutePath)
            {
                case "/index.html":
                    content = CreateIndexContent();
                    break;
                case "/js/model-viewer.min.js":
                    using (Stream? resourceStream = Application.GetResourceStream(new Uri($"pack://application:,,,/ModelViewerWPF;component/{requestUri.AbsolutePath}")).Stream)
                    {
                        using var reader = new StreamReader(resourceStream!);
                        content = reader.ReadToEnd();
                    }
                    break;
                default:
                    content = ReadContent(requestUri.AbsolutePath);
                    break;
            }

            var stream = new MemoryStream(Encoding.UTF8.GetBytes(content));
            CoreWebView2WebResourceResponse response =
                ModelViewWebView2!.CoreWebView2.Environment.CreateWebResourceResponse(
                    stream,
                    200,
                    "OK",
                    $"Content-Type: {contentType}"
                );
            return response;
        }
    }

    private string GetContentType(Uri requestUri)
    {
        // 定义一个字典来存储文件扩展名和对应的MIME类型
        var mimeTypes = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            { ".html", "text/html" },
            { ".css", "text/css" },
            { ".js", "application/javascript" },
            { ".json", "application/json" },
            { ".png", "image/png" },
            { ".jpg", "image/jpeg" },
            { ".jpeg", "image/jpeg" },
            { ".gif", "image/gif" },
            { ".svg", "image/svg+xml" },
            { ".glb", "model/gltf-binary" },
            { "gltf", "model/gltf+json" }
        };

        string extension = Path.GetExtension(requestUri.AbsolutePath);

        string contentType = "application/octet-stream";

        if (mimeTypes.TryGetValue(extension, out string? mimeType))
        {
            contentType = mimeType;
        }

        return contentType;
    }

    private string ReadContent(string absolutePath)
    {
        var dir = Path.GetDirectoryName(typeof(ModelViewer).Assembly.Location)!;
        var file = dir + absolutePath;
        if (File.Exists(file))
        {
            return File.ReadAllText(file);
        }
        else
        {
            Debug.Print($"Source not found: {file}");
            return "";
        }
    }

    private string CreateIndexContent()
    {
        var builder = new IndexSourceBuilder();

        builder.AutoPlay = AutoPlay;
        builder
            .UpdateModelSource(_randomModelSourceFile = ModelSource?.GenerateRandomFile() ?? "")
            .Build();

        return builder.Build();
    }
}

internal class Webview2InitializeUtils
{
    internal static async Task InitializeAsync(WebView2 webView2)
    {
        webView2.CreationProperties = GetWebView2CreationProperties();
        webView2.CoreWebView2InitializationCompleted += CoreWebView2InitializationCompletedHandler;
        // await CoreWebView2Environment.CreateAsync(userDataFolder: userDataFolder);

        await webView2.EnsureCoreWebView2Async();
    }

    private static void CoreWebView2InitializationCompletedHandler(
        object? sender,
        CoreWebView2InitializationCompletedEventArgs e
    )
    {
        if (sender is WebView2 webView2)
        {
            webView2.CoreWebView2InitializationCompleted -=
                CoreWebView2InitializationCompletedHandler;
        }
        if (!e.IsSuccess)
        {
            throw new ModelViewerException(
                $"Cannot init webview2: {e.InitializationException.Message}",
                e.InitializationException
            );
        }
    }

    private static CoreWebView2CreationProperties GetWebView2CreationProperties(
        string appName = nameof(ModelViewerWPF)
    )
    {
        var userDataFolder = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
            nameof(ModelViewerWPF)
        );

        if (!Directory.Exists(userDataFolder))
        {
            Directory.CreateDirectory(userDataFolder);
        }
        return new CoreWebView2CreationProperties() { UserDataFolder = userDataFolder };
    }

    internal static void RemoveWebView2(WebView2 webView)
    {
        webView.Dispose();
    }
}
