using AudioVisualizer.Utils;
using SFML.Audio;
using SFML.Graphics;
using SFML.System;

namespace AudioVisualizer.Visualization;

public class SpaceVisualization : FftVisualization
{
    private readonly Star[] _stars = new Star[256];
    private readonly VertexArray _starVertices;
    private readonly VertexArray _haloVertices;
    private readonly CircleShape _center;
    private readonly Random _randomGenerator = new Random();
    private const string CenterImagePath = "Resources/mars.png";
    private readonly float _baseCenterRadius;
    private readonly Vector2f _centerPosition;
    
    private const float PI = 3.14159265358979f;
    
    public SpaceVisualization(uint height, uint width, SoundBuffer soundBuffer, int downSampleCoefficient) : base(height, width, soundBuffer, downSampleCoefficient)
    {
        for (int i = 0; i < _stars.Length; i++)
        {
            _stars[i] = GenerateRandomStar();
        }
        _starVertices = new VertexArray(PrimitiveType.Lines, (uint)(2 * _stars.Length));
        _haloVertices = new VertexArray(PrimitiveType.TriangleFan);

        _baseCenterRadius = Height / 10f;
        _centerPosition = new Vector2f(Width / 2f, Height / 2f);
        _center = new CircleShape(_baseCenterRadius);
        _center.Texture = new Texture(CenterImagePath);
        _center.Origin = new Vector2f(_baseCenterRadius, _baseCenterRadius);
        _center.Position = _centerPosition;
    }


    public override void Draw(RenderWindow window)
    {
        window.Draw(_starVertices);
        
        window.Draw(_haloVertices);
        
        window.Draw(_center);
    }

    public override void Update()
    {
        base.Update();

        UpdateStars();

        UpdatePlanet();

        UpdateHalo();
    }

    private void UpdateHalo()
    {
        _haloVertices.Clear();
        const int from = 20;
        const int to = 60;
        const float scale = (to-from) / 180f;
        
        _haloVertices.Append(new Vertex(_centerPosition));
        
        //generating left-side
        for (float i = from; i < to; i *= 1.01f)
        {
            float binAbs = (float)Arithmetics.GetComplexAbs(Bin[(int)(i * 2)], Bin[(int)(i * 2 + 1)]);
            var newRadius = _baseCenterRadius * (1f + binAbs / 300_000_000f * 200f);
            var angle = ((i-from)/ scale + 90 ) * PI / 180f;
			
            var cartez = new Vector2f((float)(newRadius * Math.Cos(angle)), (float)(newRadius * Math.Sin(angle)));
            var basePosition = _centerPosition + cartez;
            _haloVertices.Append(new Vertex(basePosition, Color.Magenta));
        }	
        
        //generating left-side
        for (float i = from; i < to; i *= 1.01f)
        {
            float binAbs = (float)Arithmetics.GetComplexAbs(Bin[(int)(i * 2)], Bin[(int)(i * 2 + 1)]);
            var newRadius = _baseCenterRadius * (1f + binAbs / 300_000_000f * 200f);
            //minus here!
            var angle = (-(i-from)/ scale + 90 ) * PI / 180f;
			
            var cartez = new Vector2f((float)(newRadius * Math.Cos(angle)), (float)(newRadius * Math.Sin(angle)));
            var basePosition = _centerPosition + cartez;
            _haloVertices.Append(new Vertex(basePosition, Color.Red));
        }	
        
    }

    private void UpdatePlanet()
    {
        float enlargementCoefficient = 1f + GetAvgRangeIntensity(60, 250, 500_000);
        _center.Radius = _baseCenterRadius * enlargementCoefficient;
        _center.Origin = new Vector2f(_center.Radius, _center.Radius);
        _center.Position = _centerPosition;

    }

    private void UpdateStars()
    {
        var starSpeed = 1.01f + GetAvgRangeIntensity(0, 60, 10_000_000);

        uint vertexIdx = 0;
        for (int i = 0; i < _stars.Length; i++)
        {
            var star = _stars[i];
            star.OldX = star.X;
            star.OldY = star.Y;
            if (star.Z < 254)
                star.Z++;

            star.X = (star.X - Width / 2f) * starSpeed + Width / 2f;
            star.Y = (star.Y - Height / 2f) * starSpeed + Height / 2f;

            if (IsOutOfBounds(star))
            {
                star.X = _randomGenerator.Next() % Width;
                star.Y = _randomGenerator.Next() % Height;
                star.Z = 0;
                star.OldX = star.X;
                star.OldY = star.Y;
            }

            var headPosition = new Vector2f(star.X, star.Y);
            var tailPosition = new Vector2f(star.OldX, star.OldY);
            _starVertices[vertexIdx++] = new Vertex(headPosition, new Color(star.Z, star.Z, star.Z));
            _starVertices[vertexIdx++] = new Vertex(tailPosition, new Color(star.Z, star.Z, star.Z));
        }
    }

    private bool IsOutOfBounds(Star star)
    {
        return star.X < 0 || star.X > Width || star.Y < 0 || star.Y>Height;
    }

    private float GetAvgRangeIntensity(int from, int to, int reduceCoefficient)
    {
        float sum = 0;
        for (int i = from; i < to; i++)
        {
            sum += (float)Arithmetics.GetComplexAbs(Bin[i * 2], Bin[i * 2 + 1]);
        }

        return sum / (to - from) / reduceCoefficient;
    }
    
    
    private Star GenerateRandomStar()
    {
        var x = _randomGenerator.Next() % Width;
        var y = _randomGenerator.Next() % Height;
        
        return new Star
        {
            X = x,
            Y = y,
            OldX = x,
            OldY = y
        };
    }
    
}
