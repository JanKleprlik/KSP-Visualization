using AudioVisualizer.Utils;
using SFML.Audio;
using SFML.Graphics;
using SFML.System;

namespace AudioVisualizer.Visualization;

public class MapVisualization : FftVisualization
{
    private readonly Vector2f _moveDirection = new Vector2f(0, -1.2f);
    private readonly VertexArray _vertices;
    private readonly Vector2f _startingPosition;
    
    private const int Rows = 256;
    
    public MapVisualization(uint height, uint width, SoundBuffer soundBuffer, int downSampleCoefficient) : base(height, width, soundBuffer, downSampleCoefficient)
    {
        _vertices = new VertexArray(PrimitiveType.Points, AvailableBins * Rows);
        
        var rowShift = new Vector2f(0.9f, -1.2f);
        //initialize vertices;
        _startingPosition = new Vector2f(Width / 2f - AvailableBins/2f, Height/2f + Rows/2f);
        for (uint row = 0; row < Rows; row++)
        {
            for (uint i = 0; i < AvailableBins; i++)
            {
                var xOffset = new Vector2f(i, 0);
                _vertices[row * AvailableBins + i] = new Vertex(_startingPosition + xOffset + rowShift * row, GetRowColor(row));
            }
        }
        
    }

    public override void Draw(RenderWindow window)
    {
        window.Draw(_vertices);
    }

    public override void Update()
    {
        base.Update();
        
        //move existing map
        for (uint row = Rows - 1; row > 0; row--)
        {
            for (uint i = 0; i < AvailableBins; i++)
            {
                var currentPosition = _vertices[row * AvailableBins + i].Position;
                var previousPosition = _vertices[(row - 1) * AvailableBins + i].Position;
                
                _vertices[row * AvailableBins + i] =
                    new Vertex(new Vector2f(currentPosition.X, previousPosition.Y + _moveDirection.Y), GetRowColor(row));
            }
        }

        //Add new map
        float binIdx = 3f;
        for (uint i = 0; i < AvailableBins && binIdx < AvailableBins; i++, binIdx *= 1.01f)
        {
            var newPosition = 
                _startingPosition 
                + new Vector2f(i, (float)-Arithmetics.GetComplexAbs(Bin[(int)(2 * binIdx)], Bin[(int)(2 * binIdx + 1)]) / 100_000f);
            _vertices[i] = new Vertex(newPosition, GetRowColor(0));
        }
    }

    private Color GetRowColor(uint row)
    {
        return new Color(254, 254, (byte)(256 - row), 20);
    }
}