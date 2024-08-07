using System.IO;
using CommunityToolkit.Mvvm.ComponentModel;

namespace ModelViewerWPF.Samples;

public partial class MainWindowViewModel : ObservableObject
{
    [ObservableProperty]
    private string _modelSource;

    [ObservableProperty]
    private bool _autoPlay;

    [ObservableProperty]
    private List<string> _models =
    [
        "pack://application:,,,/ModelViewerWPF.Samples;component/Assets/Models/Horse.glb",
        "Assets/Models/Horse.glb"
    ];

    public MainWindowViewModel()
    {
        var dir = Directory.GetParent(AppDomain.CurrentDomain.BaseDirectory)!.Parent!.FullName;
        while (!Directory.GetDirectories(dir).Any(p => p.EndsWith("Assets")))
        {
            dir = Directory.GetParent(dir)!.FullName;
        }

        Models.Add(Path.Combine(dir, "Assets", "Models", "Horse.glb"));
        ModelSource = Models[0];
    }
}
