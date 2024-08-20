using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ManifoldNET;

namespace ModelViewerWPF.Samples;

public partial class MainWindowViewModel
{
    [ObservableProperty]
    private string? _cubeDentsModelSource;

    [ObservableProperty]
    private int _dentCount = 5;

    [ObservableProperty]
    private bool _overlap = true;

    [ObservableProperty]
    private bool _isWorking = false;

    [ObservableProperty]
    private string? _time;

    [ObservableProperty]
    private Color _cubeWithDentsColor = Colors.Green;

    [RelayCommand]
    public async Task CubeWithDents()
    {
        IsWorking = true;
        Stopwatch stopwatch = new Stopwatch();
        try
        {
            stopwatch.Start();
            CubeDentsModelSource = "";

            var manifoldFile = await Task.Run(() =>
            {
                var manifold = Run(DentCount, Overlap);

                // Save
                string fileName = "cube_with_dents.glb";
                if (File.Exists(fileName))
                    File.Delete(fileName);

                manifold.MeshGL.ExportMeshGL(
                    fileName,
                    new ExportOptions()
                    {
                        Material = new Material()
                        {
                            Color = new Vector4(
                                CubeWithDentsColor.R / 255f,
                                CubeWithDentsColor.G / 255f,
                                CubeWithDentsColor.B / 255f,
                                CubeWithDentsColor.A / 255f
                            ),
                        }
                    }
                );

                return fileName;
            });

            CubeDentsModelSource = manifoldFile;

            static Manifold Run(int n = 5, bool overlap = true)
            {
                var a = Manifold.Cube(n, n, 0.5f).Translate(-0.5f, -0.5f, -0.5f);

                List<Manifold> list = new(n * n);
                for (int i = 0; i < n; i++)
                {
                    for (int j = 0; j < n; j++)
                    {
                        list.Add(Manifold.Sphere(overlap ? 0.45f : 0.55f, 50).Translate(i, j, 0));
                    }
                }

                return Manifold.Difference(a, list.Aggregate(Manifold.Union));
            }

            stopwatch.Stop();
            Time = stopwatch.ElapsedMilliseconds.ToString() + "ms";
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
        finally
        {
            IsWorking = false;
        }
    }
}
