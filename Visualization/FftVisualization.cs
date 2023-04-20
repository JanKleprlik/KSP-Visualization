using AudioVisualizer.AudioProcessing;
using SFML.Audio;

namespace AudioVisualizer.Visualization;

public abstract class FftVisualization : AbstractVisualization
{
    protected readonly double[] Bin;
    protected readonly uint AvailableBins;
    private const uint ImaginaryDataCoefficient = 2;
    private readonly int _downSampleCoefficient;
    private readonly double[] _window;
    private readonly double[] _samples; //array used for down-sampling
    private double[] _cutOffData; //down-sampled data
    private readonly AudioModificator _audioModificator;

    
    protected FftVisualization(uint height, uint width, SoundBuffer soundBuffer, int downSampleCoefficient) : base(height, width, soundBuffer)
    {
        _downSampleCoefficient = downSampleCoefficient;
        Bin = new double[(IterationDataSize / _downSampleCoefficient) * ImaginaryDataCoefficient];
        AvailableBins = (uint)(Bin.Length / (2 * 2)); //2 for Im+Re; 2 for symmetricity of Bin
        _window = FastFourierTransformation.GenerateHammingWindow((uint)(IterationDataSize / _downSampleCoefficient));
        _samples = new double[IterationDataSize];
        _cutOffData = new double[IterationDataSize / _downSampleCoefficient];

        _audioModificator = new AudioModificator((int)IterationDataSize);
    }

    public override void Update()
    {
        int offset = (int)(Song.PlayingOffset.AsSeconds() * SampleRate);
        SetImaginary();

        if ((offset + IterationDataSize) * ChannelCount  < SampleCount)
        {
            for (uint i = 0; i < IterationDataSize; i++)
            {
                _samples[i] = AudioData[(i + offset) * ChannelCount];
            }
        }
        
        //Filter out frequencies and then down-samples
        _cutOffData = _audioModificator.DownSample(_samples, _downSampleCoefficient, SampleRate);

        //apply window to smoothen FFT output
        ApplyWindow(_cutOffData);

        FastFourierTransformation.Fft(Bin);
    }
    
    private void SetImaginary()
    {
        for (int i = 0; i < IterationDataSize / _downSampleCoefficient; i++)
        {
            Bin[i * 2 + 1] = 0d;
        }
    }
    
    private void ApplyWindow(double[] cutOffData)
    {
        for (int i = 0; i < IterationDataSize / _downSampleCoefficient; i++)
        {
            Bin[i * 2] = cutOffData[i] * _window[i];
        }
    }
}