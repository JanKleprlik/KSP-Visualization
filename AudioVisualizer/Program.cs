// ReSharper disable StringLiteralTypo
using AudioVisualizer;
using AudioVisualizer.Visualization;
using SFML.Audio;

const uint width = 1224;
const uint height = 800;
const int fps = 30;

const string pathBase = @"C:\Users\klepr\source\repos\KSP\AudioVizualiser\AudioVisualizer\Resources\";

// VisualizationMode visualizationMode = UserSelectMode();
// string songName = UserSelectSong();

const VisualizationMode visualizationMode = VisualizationMode.Line;
const string songName = "sifflet";

var data = File.ReadAllBytes(pathBase + songName + ".wav");
SoundBuffer soundBuffer = new SoundBuffer(data);

IVisualization visualisation;
switch (visualizationMode)
{
    case VisualizationMode.Line:
        visualisation = new LineVisualization(height, width, soundBuffer);
        break;
    case VisualizationMode.Bars:
        visualisation = new BarVisualization(height, width, soundBuffer, 4);
        break;
    case VisualizationMode.Map:
        visualisation = new MapVisualization(height, width, soundBuffer, 4);
        break;
    case VisualizationMode.Spectrogram:
        visualisation = new SpectrogramVisualization(height, width, soundBuffer, 4);
        break;
    case VisualizationMode.Space:
        visualisation = new SpaceVisualization(height, width, soundBuffer, 4);
        break;
    default:
        throw new ArgumentOutOfRangeException();
}


var window = new SFML.Graphics.RenderWindow(
    new SFML.Window.VideoMode(width, height),
    visualisation.GetType().ToString());
window.SetFramerateLimit(fps);
window.KeyPressed += (sender, eventArgs) =>
{
    if (sender == null) return;
    var senderWindow = (SFML.Window.Window)sender;
    if (eventArgs.Code == SFML.Window.Keyboard.Key.Escape)
    {
        senderWindow.Close();
    } };


// Start the game loop
while (window.IsOpen)
{
    visualisation.Update();
    window.Clear();

    // Process events
    window.DispatchEvents();
    visualisation.Draw(window);
        
    // Finally, display the rendered frame on screen
    window.Display();
}

#pragma warning disable CS8321
VisualizationMode UserSelectMode()
{
    Console.WriteLine("Select one of: `line`, `bar`, `map`, `spec`, `space`: ");
    while (true)
    {
        var modeInput = Console.ReadLine();
        switch (modeInput)
        {
            case "Line":
            case "line":
                return VisualizationMode.Line;

            case "Bar":
            case "bar":
                return VisualizationMode.Bars;

            case "Map":
            case "map":
                return VisualizationMode.Map;

            case "Spec":
            case "spec":
                return VisualizationMode.Spectrogram;

            case "Space":
            case "space":
                return VisualizationMode.Space;
        }
    }
}

string UserSelectSong()
{
    string? name = "_";
    Console.WriteLine("Select song name:");
    while (name is null || !File.Exists(pathBase + name + ".wav"))
    {
        name = Console.ReadLine();
    }

    return name;
}
#pragma warning restore CS8321
