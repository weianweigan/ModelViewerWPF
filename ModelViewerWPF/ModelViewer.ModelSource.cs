using System;
using System.Threading.Tasks;
using System.Windows;
using Microsoft.Web.WebView2.Core;
using ModelViewerWPF.Utils;

namespace ModelViewerWPF;

public partial class ModelViewer
{
    public static readonly DependencyProperty ModelSourceProperty = DependencyProperty.Register(
        "ModelSource",
        typeof(Uri),
        typeof(ModelViewer),
        new PropertyMetadata(null, OnModelSourceChanged)
    );

    public Uri ModelSource
    {
        get => (Uri)GetValue(ModelSourceProperty);
        set => SetValue(ModelSourceProperty, value);
    }

    private static void OnModelSourceChanged(
        DependencyObject d,
        DependencyPropertyChangedEventArgs e
    )
    {
        var control = (ModelViewer)d;
        Uri? newUri = e.NewValue as Uri;

        string randomName = newUri?.GenerateRandomFile() ?? "";
        control.UpdateModelSourceRandomName(randomName);
        control.ModelViewerLoaded = false;
        ChangeModelViewerProperty(control, "src", randomName);
    }

    private void UpdateModelSourceRandomName(string randomName)
    {
        _randomModelSourceFile = randomName;
    }

    /// <summary>
    /// Open dev tools when webview loaded.
    /// </summary>
    public bool OpenDevToolsWindow
    {
        get { return (bool)GetValue(OpenDevToolsWindowProperty); }
        set { SetValue(OpenDevToolsWindowProperty, value); }
    }

    public static readonly DependencyProperty OpenDevToolsWindowProperty =
        DependencyProperty.Register(
            "OpenDevToolsWindow",
            typeof(bool),
            typeof(ModelViewer),
            new PropertyMetadata(false)
        );

    private CoreWebView2WebResourceResponse LoadModelSource(string contentType)
    {
        if (ModelViewWebView2 == null)
        {
            throw new NullReferenceException(nameof(ModelViewWebView2));
        }
        var stream = FileReader.ReadFileFromUri(ModelSource);
        CoreWebView2WebResourceResponse response =
            ModelViewWebView2!.CoreWebView2.Environment.CreateWebResourceResponse(
                stream,
                StatusCode: 200,
                ReasonPhrase: "OK",
                Headers: $"Content-Type: {contentType}"
            );

        return response;
    }
}
