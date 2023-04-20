namespace AudioVisualizer.AudioProcessing;

public class AudioModificator
{
	private ButterworthFilter _butterworthFilter;

	public AudioModificator(int dataSize)
	{
		_butterworthFilter = new ButterworthFilter(dataSize);
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