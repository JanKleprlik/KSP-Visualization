using SFML.Audio;
using SFML.Graphics;
using SFML.System;

namespace AudioVisualizer.Visualization;

public class LineVisualization : AbstractVisualization
{
    private readonly VertexArray _vertices;
    
    public LineVisualization(uint height, uint width, SoundBuffer soundBuffer) : base(height, width, soundBuffer)
    {
        IterationDataSize = 1024;
        _vertices = new VertexArray(PrimitiveType.LineStrip, IterationDataSize*2);
        
        _vertices[0] = new Vertex(new Vector2f((Width - IterationDataSize) / 2f, Height / 2f));
        _vertices[1] = new Vertex(new Vector2f((Width - IterationDataSize) / 2f, Height / 2f));
    }

    public override void Draw(RenderWindow window)
    {
        window.Draw(_vertices);
    }

    public override void Update()
    {
        int offset = (int)(Song.PlayingOffset.AsSeconds() * SampleRate);
        if (offset + IterationDataSize >= SampleCount) return;

        for (uint i = 1; i < IterationDataSize; i++)
        {
            _vertices[2*i] = new Vertex(new Vector2f(ComputeX(i-1), ComputeY(i-1, offset)));
            _vertices[2*i + 1] = new Vertex(new Vector2f(ComputeX(i), ComputeY(i, offset)));
        }
    }

    private float ComputeX(uint idx)
    {
        float sideOffset = (Width - IterationDataSize) / 2f;
        return idx + sideOffset;
    }

    private float ComputeY(uint idx, int offset)
    {
        const float amplifier = 0.008f;
        float heightCenter = Height / 2f;
        return heightCenter + AudioData[offset + idx * ChannelCount] * amplifier;
    }
}