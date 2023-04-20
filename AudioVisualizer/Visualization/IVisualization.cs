using SFML.Audio;
using SFML.Graphics;

namespace AudioVisualizer.Visualization;

public interface IVisualization
{
    void Draw(RenderWindow window);
    void Update();
    void Quit();
}