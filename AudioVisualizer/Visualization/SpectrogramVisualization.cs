using AudioVisualizer.Utils;
using SFML.Audio;
using SFML.Graphics;
using SFML.System;

namespace AudioVisualizer.Visualization;

public class SpectrogramVisualization : FftVisualization
{
    private readonly uint _spectrogramWidth;
    private readonly VertexArray _vertices;
    private uint _updateColIndex;
    private readonly Vector2f _startingPosition;
    private readonly Vector2f _columnShift;
    
    public SpectrogramVisualization(uint height, uint width, SoundBuffer soundBuffer, int downSampleCoefficient) : base(height, width, soundBuffer, downSampleCoefficient)
    {
        _spectrogramWidth = (uint)(Width * 0.8f);
        _vertices = new VertexArray(PrimitiveType.Points, AvailableBins * _spectrogramWidth);
        _startingPosition = new Vector2f(Width * 0.1f, Height/2f + AvailableBins/2f);
        _columnShift = new Vector2f(1f, 0f);

        for (uint col = 0; col < _spectrogramWidth; col++)
        {
            for (uint i = 0; i < AvailableBins; i++)
            {
                var yOffset = new Vector2f(0, i);
                _vertices[col * AvailableBins + i] = new Vertex(_startingPosition - yOffset + _columnShift * col, Color.Black);
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

        for (int i = 0; i < AvailableBins; i++)
        {
            var yOffset = new Vector2f(0, i);
            var newPosition = _startingPosition - yOffset + _columnShift * _updateColIndex;
            _vertices[(uint)(_updateColIndex * AvailableBins + i)] = new Vertex(newPosition, IntensityToColor(Bin[i * 2], Bin[i*2 + 1], (int)AvailableBins)); 
        }
        
        _updateColIndex++;
        _updateColIndex %= _spectrogramWidth;
    }
    
    private Color IntensityToColor(double real, double imaginary, int n)
    {
        //Black : 0,0,0
        //White: 255,255,255
        var normalized = Arithmetics.GetComplexAbs(real, imaginary) / n;
        var decibel = 20 * Math.Log10(normalized);
        byte colorIntensity;
        if (decibel < 0)
        {
            colorIntensity = 0;
        }
        else if (decibel > 255)
        {
            colorIntensity = 255;
        }
        else
        {
            colorIntensity = (byte)(int)decibel;
        }
        
        return new Color(colorIntensity,colorIntensity,colorIntensity);
    }
}