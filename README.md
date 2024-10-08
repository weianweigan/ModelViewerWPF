# ModelViewerWPF

![NuGet Version](https://img.shields.io/nuget/v/ModelViewerWPF)
![NuGet Downloads](https://img.shields.io/nuget/dt/ModelViewerWPF)

ModelViewerWPF is a WPF control that uses WebView2 to display 3D models within a WPF application. 
This control is based on the [model-viewer](https://modelviewer.dev/) and supports *.gltf and *.glb formats.

## Features

- Display 3D models using WebView2
- Supports GLB/GLTF 3D model formats

<img src="wpf.png" alt="window" height="200">

## Installation

You can install [ModelViewerWPF](https://www.nuget.org/packages/ModelViewerWPF/) via the NuGet Package Manager:

```powershell
Install-Package ModelViewerWPF
```

Simple Usage

```xaml
<Window
    x:Class="YourNamespace.MainWindow"
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    xmlns:modelviewer="clr-namespace:ModelViewerWPF;assembly=ModelViewerWPF"
    Title="MainWindow"
    Width="800"
    Height="450">
    <Grid>
        <modelviewer:ModelViewer
            Margin="4"
            ModelSource="Assets/Models/Horse.glb" />
    </Grid>
</Window>
```

## Generate glb files based on [manidfold-csharp](https://github.com/weianweigan/manifold-csharp/blob/dev-csharp/bindings/csharp/Readme.md)

<img src="manifold.png" alt="window" height="400">
