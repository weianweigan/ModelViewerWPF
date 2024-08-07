using System;
using System.Text;

namespace ModelViewerWPF.HtmlSource;

/// <summary>
/// Build index.html file.
/// </summary>
/// <remarks>
/// <see href="https://modelviewer.dev/docs/index.html"/>
/// </remarks>
internal class IndexSourceBuilder
{
    public const string TopContent = """
        <!DOCTYPE html>
        <html lang="en">

        <head>
            <meta charset="UTF-8">
            <meta name="viewport" content="width=device-width, initial-scale=1.0">
            <title>Document</title>
            <style>
                html, body {
                    height: 100%;
                    margin: 0;
                    padding: 0;
                }
                model-viewer {
                    width: 100%;
                    height: 100%;
                }
            </style>
        """;

    public const string BottomContent = """        
            <script type="module" src="https://ajax.googleapis.com/ajax/libs/model-viewer/3.5.0/model-viewer.min.js"></script>
        </head>

        <body>
            <model-viewer
                alt="Neil Armstrong's Spacesuit from the Smithsonian Digitization Programs Office and National Air and Space Museum"
                src="ModelViewerWPF.Samples/Assets/Models/Horse.glb" shadow-intensity="1" camera-controls
                touch-action="pan-y"></model-viewer>
        </body>

        </html>
        """;

    /// <summary>
    /// Load local js module file when it sets true.
    /// </summary>
    /// <remarks>
    /// Some regions cannot access Google services.
    /// </remarks>
    public bool UseLocalJsModule { get; set; } = true;

    /// <summary>
    /// The URL to the 3D model.
    /// </summary>
    /// <remarks>
    /// Only glTF/GLB models are supported.
    /// </remarks>
    public string? ModelSource { get; set; }

    /// <summary>
    /// Configures the model with custom text that will be used to describe the model to viewers
    /// who use a screen reader or otherwise depend on additional semantic context to understand what they are viewing.
    /// </summary>
    public string Alt { get; set; } = "Model";

    /// <summary>
    /// An enumerable attribute describing under what conditions the model should be preloaded.
    /// </summary>
    public LoadingOptions LoadingOptions { get; set; } = LoadingOptions.Auto;

    /// <summary>
    /// This attribute controls when the model should be revealed.
    /// </summary>
    public RevealOptions RevealOptions { get; set; } = RevealOptions.Auto;

    /// <summary>
    /// This attribute makes the browser include credentials (cookies, authorization headers or TLS client certificates) in the request to fetch the 3D model.
    /// It's useful if the 3D model file is stored on another server that require authentication.
    /// By default the file will be fetch without credentials.
    /// Note that this has no effect if you are loading files locally or from the same domain.
    /// </summary>
    public bool WithCredentials { get; set; }

    /// <summary>
    /// Add unique translations for the model wcag compliance.
    /// Use this attribute, you must provide a JSON object with the following keys:
    /// front, back, left, right, upper-front, upper-back, upper-left, upper-right, lower-front, lower-back, lower-left, lower-right, interaction-prompt.
    /// The values of these keys should be the translations for the model's orientation.
    /// This attribute is useful for screen readers and other assistive technologies.
    /// </summary>
    public string? Ally { get; set; }

    #region Animation
    public bool AutoPlay { get; set; }
    #endregion

    public IndexSourceBuilder UpdateModelSource(string source)
    {
        ModelSource = source;
        return this;
    }

    public string Build()
    {
        var stringBuilder = new StringBuilder();

        stringBuilder.Append(TopContent);

        if (UseLocalJsModule)
        {
            stringBuilder.Append(
                """
                    <script type="module" src="js/model-viewer.min.js"></script>
                """
            );
        }
        else
        {
            stringBuilder.Append(
                """
                    <script type="module" src="https://ajax.googleapis.com/ajax/libs/model-viewer/3.5.0/model-viewer.min.js"></script>
                """
            );
        }

        stringBuilder.Append(
            """
            </head>

            <body>
            """
        );

        // Add modelviewer.
        stringBuilder.Append(
            $"""
                <model-viewer
                    id="modelViewer"
                    alt="{Alt}"
                    src="{ModelSource}" 
                    loading="{LoadingOptions.ToString().ToLower()}"
                    reveal="{RevealOptions.ToString().ToLower()}"
                    shadow-intensity="1" 
                    camera-controls
                    {(AutoPlay ? "autoplay" : "")}
                    touch-action="pan-y"></model-viewer>
            """
        );

        // Add bottom items.
        stringBuilder.Append(
            """
            </body>

            </html>
            """
        );

        return stringBuilder.ToString();
    }
}
