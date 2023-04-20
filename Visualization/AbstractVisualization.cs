using SFML.Audio;
using SFML.Graphics;

namespace AudioVisualizer.Visualization;

public abstract class AbstractVisualization : IVisualization
{
    protected readonly uint Height;
    protected readonly uint Width;

    protected AbstractVisualization(uint height, uint width, SoundBuffer soundBuffer)
    {
        Height = height;
        Width = width;
        Buffer = soundBuffer;
        Song = new Sound(Buffer);
        SampleRate = soundBuffer.SampleRate;
        SampleCount = (uint)soundBuffer.Samples.Length;
        AudioData = soundBuffer.Samples;
        if (soundBuffer.ChannelCount > 2)
        {
            throw new ArgumentException($"Too many channels: {soundBuffer.ChannelCount}");
        }
        ChannelCount = soundBuffer.ChannelCount;
        
        Song.Loop = true;
        Song.Play();
    }

    private SoundBuffer Buffer { get;}
    protected Sound Song { get; }
    protected short[] AudioData { get; }
    protected uint SampleRate { get; }
    protected uint SampleCount { get; }
    protected uint IterationDataSize { get; init; } = 4096;
    protected uint ChannelCount { get; }
    public abstract void Draw(RenderWindow window);
    public abstract void Update();

    public void Quit()
    {
        Song.Stop();
        Song.Dispose();
    }
}