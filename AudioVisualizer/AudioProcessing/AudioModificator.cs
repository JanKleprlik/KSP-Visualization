using AudioVisualizer.AudioFormats;
using AudioVisualizer.Utils;

namespace AudioVisualizer.AudioProcessing;

public class AudioModificator
{
	private ButterworthFilter _butterworthFilter;

	public AudioModificator(int dataSize)
	{
		_butterworthFilter = new ButterworthFilter(dataSize);
	}
	
	/// Resamples data from multiple channels to single one.
	public void ConvertToMono(IAudioFormat audio)
	{
		if (audio == null)
			throw new ArgumentNullException($"Argument {nameof(audio)} is null.");
		if (audio.Data == null)
			throw new ArgumentNullException($"Argument {nameof(audio.Data)} is null");

		switch (audio.Channels)
		{
			case 1:
				break;

			case 2:
				short[] mono = new short[audio.NumOfDataSamples / 2];

				for (int i = 0; i < audio.NumOfDataSamples; i += 2) //4 bytes per loop are processed (2 left + 2 right samples)
				{
					mono[i / 2] = Arithmetics.Average(audio.Data[i], audio.Data[i + 1]);
				}

				// number of bytes is halved (Left and Right bytes are merget into one)
				audio.Data = mono; //set new SampleData
				audio.Channels = 1; //lower number of channels
				audio.NumOfDataSamples /= 2;  //lower number of data samples
				break;
			default:
				throw new NotImplementedException($"Convert from {audio.Channels} channels to mono is not supported.");
		}
	}

	/// Down-samples data by a <c>downFactor</c>
	/// <param name="data">data to down-sample</param>
	/// <param name="downFactor">factor of down-sampling</param>
	/// <param name="sampleRate">original sample rate</param>
	/// <returns>Down-sampled data</returns>
	public double[] DownSample(double[] data, int downFactor, double sampleRate)
	{
		if (downFactor == 0 || downFactor == 1)
			return data;
		var res = new double[data.Length / downFactor];

		//filter out frequencies larger than the one that will be available
		//after down-sampling by downFactor to avoid audio aliasing.
		double cutOff = sampleRate / downFactor;
		// double[] dataDouble = new double[data.Length];
		// for (int i = 0; i < data.Length; i++)
		// {
		// 	dataDouble[i] = data[i];
		// }


		var dataDoubleDownSampled = _butterworthFilter.Apply(data, sampleRate, cutOff);

		//make average of every downFactor number of samples
		for (int i = 0; i < dataDoubleDownSampled.Length / downFactor; i++)
		{
			double sum = 0;
			for (int j = 0; j < downFactor; j++)
			{
				sum += data[i * downFactor + j];
			}
			res[i] = sum / downFactor;
		}

		return res;
	}
}