using System.Windows;

namespace ModelViewerWPF;

public partial class ModelViewer
{
    /// <summary>
    /// Web resource loading failure is displayed.
    /// </summary>
    public bool DisplayException
    {
        get { return (bool)GetValue(DisplayExceptionProperty); }
        set { SetValue(DisplayExceptionProperty, value); }
    }

    public static readonly DependencyProperty DisplayExceptionProperty =
        DependencyProperty.Register(
            "DisplayException",
            typeof(bool),
            typeof(ModelViewer),
            new PropertyMetadata(true)
        );
}
