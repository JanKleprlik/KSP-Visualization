using AudioVisualizer.Utils;

namespace AudioVisualizer.AudioFormats;

public class WavFormat : IAudioFormat
{
	/// Manual constructor from 16 bit PCM audio data.
	/// <param name="sampleRate">Audio sample rate.</param>
	/// <param name="channels">Number of audio channels.</param>
	/// <param name="numOfDataSamples">Number of total data samples.</param>
	/// <param name="data">16 bit PCM audio data.</param>
	public WavFormat(uint sampleRate, uint channels, int numOfDataSamples, short[] data)
	{
		Channels = channels;
		SampleRate = sampleRate;
		NumOfDataSamples = numOfDataSamples;
		Data = data;
	}

	/// Creates WavFormat from raw audio data containing audio metadata.
	/// <param name="rawData">Raw audio data including metadata.</param>
	public WavFormat(byte[] rawData)
	{
		// Check for RIFF at the beginning of the data.
		if ( rawData.Length < 4 || !IsCorrectFormat(new[] { rawData[0], rawData[1], rawData[2], rawData[3] }))
		{
			throw new ArgumentException("Invalid data format given to Wav format constructor.");
		}

		// Find FMT offset in data so we can read metadata.
		int fmtOffset = FindOffset(rawData, new byte[] { 0x66, 0x6D, 0x74, 0x20 });

		Channels = Converter.BytesToUInt(new byte[] { rawData[fmtOffset + 2], rawData[fmtOffset + 3] });
		SampleRate = Converter.BytesToUInt(new byte[] { rawData[fmtOffset + 4], rawData[fmtOffset + 5], rawData[fmtOffset + 6], rawData[fmtOffset + 7] });

		// Find data offset so we can read raw audio data.
		int dataOffset = FindOffset(rawData, new byte[] { 0x64, 0x61, 0x74, 0x61 });

		// Number of bytes divide by two (short = 2 bytes && 1 sample = 1 short)
		NumOfDataSamples = Converter.BytesToInt(new byte[]
			{rawData[dataOffset - 4], rawData[dataOffset - 3], rawData[dataOffset - 2], rawData[dataOffset - 1]}) / 2;
		var byteData = rawData.Skip(dataOffset).Take(this.NumOfDataSamples * 2).ToArray();
		Data = Converter.BytesToShorts(byteData);
	}

	public uint Channels { get; set; }
	public uint SampleRate { get; set; }
	public int NumOfDataSamples { get; set; }
	public short[] Data { get; set; }

	/// Check for RIFF at the start of the data array
	/// <returns>True if contains RIFF, false otherwise.</returns>
	public static bool IsCorrectFormat(byte[] data)
	{
		//RIFF in ASCII
		if (data[0] == 0x52 && //R
		    data[1] == 0x49 && //I
		    data[2] == 0x46 && //F
		    data[3] == 0x46)   //F
			return true;
		return false;
	}


	/// Finds offset of data same as anchor
	/// <param name="data">Data to look in.</param>
	/// <param name="anchor">Bytes to look for in data.</param>
	/// <returns>Offset of the anchor in provided data.</returns>
	private static int FindOffset(byte[] data, byte[] anchor)
	{
		if (data.Length < anchor.Length)
			throw new ArgumentException("Unable to find offset: anchor is longer than data");
		for (int i = 0; i <= data.Length - anchor.Length; i++)
		{
			if (data[i] == anchor[0])
			{
				int correct = 1;
				for (int j = 1; j < anchor.Length; j++)
				{
					if (data[i + j] == anchor[j])
						correct++;
					else
						break;
				}
				if (correct == anchor.Length)
				{
					return i + anchor.Length;
				}
			}
		}

		return -1;
	}
}