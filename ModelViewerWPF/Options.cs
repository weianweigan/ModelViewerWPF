namespace ModelViewerWPF;

/// <summary>
/// An enumerable attribute describing under what conditions the model should be preloaded. The supported values are "auto", "lazy" and "eager".
/// </summary>
public enum LoadingOptions
{
    /// <summary>
    /// Auto is equivalent to lazy, which loads the model when it is near the viewport for reveal="auto"
    /// </summary>
    Auto,
    Lazy,
    Eager,
}

/// <summary>
/// This attribute controls when the model should be revealed.
/// </summary>
public enum RevealOptions
{
    /// <summary>
    /// If reveal is set to "auto", the model will be revealed as soon as it is done loading and rendering.
    /// </summary>
    Auto,

    /// <summary>
    ///  If reveal is set to "manual", the model will remain hidden until dismissPoster() is called.
    /// </summary>
    Manual,
}
