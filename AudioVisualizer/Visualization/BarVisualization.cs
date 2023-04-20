using AudioVisualizer.Utils;
using SFML.Audio;
using SFML.Graphics;
using SFML.System;

namespace AudioVisualizer.Visualization;

public class BarVisualization : FftVisualization
{
    private readonly VertexArray _vertices;
    public BarVisualization(uint height, uint width, SoundBuffer soundBuffer, int downSampleCoefficient) : base(height, width, soundBuffer, downSampleCoefficient)
    {
        _vertices = new VertexArray(PrimitiveType.LineStrip, AvailableBins);
    }

    public override void Draw(RenderWindow window)
    {
        window.Draw(_vertices);
    }

    public override void Update()
    {
        base.Update();

        for (uint i = 0; i < _vertices.VertexCount; i++)
        {
            _vertices[i] = new Vertex(new Vector2f(ComputeX(i), ComputeY(i)));
        }
    }

    private float ComputeY(uint i)
    {
        float heightCenter = Height / 2f;
        return (float)(heightCenter- Arithmetics.GetComplexAbs(Bin[2 * i], Bin[2 * i + 1]) / 50_000);
    }

    private float ComputeX(uint i)
    {
        float sideOffset = (Width - _vertices.VertexCount) / 2f;
        return i + sideOffset;
    }
}