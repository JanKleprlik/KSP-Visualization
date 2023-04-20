namespace AudioVisualizer.AudioFormats;

public interface IAudioFormat
{
	/// Number of audio channels
	uint Channels { get; set; }
	/// Sample rate of the audio.
	uint SampleRate { get; set; }
	/// Number of Data Samples
	int NumOfDataSamples { get; set; }
	/// Raw audio data.
	short[] Data { get; set; }
}