using System.Threading.Tasks;
using System.Windows;

namespace ModelViewerWPF;

public partial class ModelViewer
{
    public bool AutoPlay
    {
        get { return (bool)GetValue(AutoPlayProperty); }
        set { SetValue(AutoPlayProperty, value); }
    }

    public static readonly DependencyProperty AutoPlayProperty = DependencyProperty.Register(
        "AutoPlay",
        typeof(bool),
        typeof(ModelViewer),
        new PropertyMetadata(false, OnAutoPlayChanged)
    );

    private static void OnAutoPlayChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
        var control = (ModelViewer)d;
        bool autoPlay = (bool)e.NewValue;
        if (autoPlay)
        {
            AppendModelViewerAttribute(control, "autoplay");
        }
        else
        {
            RemoveModelViewerAttribute(control, "autoplay");
        }
    }
}
