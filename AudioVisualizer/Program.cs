using AudioVisualizer;
using AudioVisualizer.Visualization;
using SFML.Audio;
using SFML.Graphics;

const uint width = 1224;
const uint height = 800;
const int fps = 30;

VisualizationMode visualizationMode = UserSelectMode();
string songName = UserSelectSong();

// const VisualizationMode visualizationMode = VisualizationMode.Line;
// const string songName = "sifflet";

SoundBuffer soundBuffer = GetSoundBuffer(songName);
IVisualization visualisation = GetVisualization(visualizationMode, height, width, soundBuffer);
var window = SetupNewWindow(width, height, visualisation, fps);


// Start the game loop
while (window.IsOpen)
{
    //clear current frame
    window.Clear();
    // Process events
    window.DispatchEvents();
    
    //update and draw visualisation
    visualisation.Update();
    visualisation.Draw(window);
        
    // Finally, display the rendered frame on screen
    window.Display();
}

visualisation.Quit();

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
    string? fullPath = null;
    Console.WriteLine("Select full path to song:");
    while (fullPath is null || !File.Exists(fullPath))
    {
        fullPath = Console.ReadLine();
    }

    return fullPath;
}

IVisualization GetVisualization(VisualizationMode mode, uint winHeight, uint winWidth, SoundBuffer soundBuffer1)
{
    IVisualization visualization;
    switch (mode)
    {
        case VisualizationMode.Line:
            visualization = new LineVisualization(winHeight, winWidth, soundBuffer1);
            break;
        case VisualizationMode.Bars:
            visualization = new BarVisualization(winHeight, winWidth, soundBuffer1, 4);
            break;
        case VisualizationMode.Map:
            visualization = new MapVisualization(winHeight, winWidth, soundBuffer1, 4);
            break;
        case VisualizationMode.Spectrogram:
            visualization = new SpectrogramVisualization(winHeight, winWidth, soundBuffer1, 4);
            break;
        case VisualizationMode.Space:
            visualization = new SpaceVisualization(winHeight, winWidth, soundBuffer1, 4);
            break;
        default:
            throw new ArgumentOutOfRangeException();
    }

    return visualization;
}

RenderWindow SetupNewWindow(uint winWidth, uint winHeight, IVisualization visualisation1, uint frameLimit)
{
    var renderWindow = new RenderWindow(
        new SFML.Window.VideoMode(winWidth, winHeight),
        visualisation1.GetType().ToString());
    renderWindow.SetFramerateLimit(frameLimit);
    renderWindow.KeyPressed += (sender, eventArgs) =>
    {
        if (sender == null) return;
        var senderWindow = (SFML.Window.Window)sender;
        if (eventArgs.Code == SFML.Window.Keyboard.Key.Escape)
        {
            senderWindow.Close();
        }
    };
    return renderWindow;
}

SoundBuffer GetSoundBuffer(string fullPath)
{
    var rawData = File.ReadAllBytes(fullPath);
    return new SoundBuffer(rawData);
}