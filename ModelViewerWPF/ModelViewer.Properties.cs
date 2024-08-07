using System.Threading.Tasks;
using Microsoft.Web.WebView2.Core;

namespace ModelViewerWPF;

public partial class ModelViewer
{
    private const string ElementId = "modelViewer";

    private static async Task AppendModelViewerAttribute(
        ModelViewer modelViewer,
        string attributeName
    )
    {
        await ExecuteScriptAsync(
            modelViewer,
            $"document.getElementById('{ElementId}').setAttribute('{attributeName}', '');"
        );
    }

    private static async Task RemoveModelViewerAttribute(
        ModelViewer modelViewer,
        string attributeName
    )
    {
        await ExecuteScriptAsync(
            modelViewer,
            $"document.getElementById('{ElementId}').removeAttribute('{attributeName}');"
        );
    }

    private static async Task ChangeModelViewerProperty(
        ModelViewer modelViewer,
        string propertyName,
        object value
    )
    {
        await ExecuteScriptAsync(
            modelViewer,
            $"document.getElementById('{ElementId}').{propertyName} = '{value}';"
        );
    }

    private static async Task ExecuteScriptAsync(ModelViewer modelViewer, string script)
    {
        if (modelViewer.ModelViewWebView2?.CoreWebView2 == null)
        {
            return;
        }
        await modelViewer.ModelViewWebView2.CoreWebView2.ExecuteScriptAsync(script);
    }

    private static async Task<CoreWebView2ExecuteScriptResult?> ExecuteScriptWithResultAsync(
        ModelViewer modelViewer,
        string script
    )
    {
        if (modelViewer.ModelViewWebView2?.CoreWebView2 == null)
        {
            return null;
        }
        CoreWebView2ExecuteScriptResult result =
            await modelViewer.ModelViewWebView2.CoreWebView2.ExecuteScriptWithResultAsync(script);
        return result;
    }
}
