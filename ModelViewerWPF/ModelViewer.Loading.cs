using Microsoft.Web.WebView2.Core;

namespace ModelViewerWPF;

public partial class ModelViewer
{
    public bool QueryModelViewerLoaded()
    {
        if (!WebViewNavigationCompleted)
        {
            return false;
        }
        CoreWebView2ExecuteScriptResult? result = ExecuteScriptWithResultAsync(
                this,
                $"document.getElementById('{ElementId}').value;"
            )
            .GetAwaiter()
            .GetResult();

        if (result?.Succeeded != true)
        {
            return false;
        }

        return bool.Parse(result.ResultAsJson);
    }
}
