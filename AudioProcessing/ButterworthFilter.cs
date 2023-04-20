namespace AudioVisualizer.AudioProcessing;

public class ButterworthFilter
{
	private double[] _dataCopy;
	private double[] datYt;
	private double[] datZt;
	private const double pi = 3.14159265358979;
	private const double sqrt2 = 1.414213562;

	public ButterworthFilter(int dataSize)
	{
		_dataCopy = new double[dataSize - 1 + 4]; // Array with 4 extra points front and back
		datYt = new double[dataSize - 1 + 4];
		datZt = new double[dataSize - 1 + 2];
	}
	
	/// Butterworth filter implementation taken from http://www.codeproject.com/Tips/1092012/A-Butterworth-Filter-in-Csharp
	/// <param name="inData">PCM 16bit audio data</param>
	/// <param name="samplingRate">Original sample rate</param>
	/// <param name="cutOffFrequency">Cut off frequency</param>
	public double[] Apply(double[] inData, double samplingRate, double cutOffFrequency)
	{
		if (cutOffFrequency == 0) return inData;
		long dF2 = inData.Length - 1;        // The data range is set with dF2
		
		// double[] data2 = new double[dF2 + 4]; // Array with 4 extra points front and back
		double[] data = inData; // Ptr., changes passed data

		// Copy indata to Dat2
		for (long r = 0; r < dF2; r++)
		{
			_dataCopy[2 + r] = inData[r];
		}
		_dataCopy[1] = _dataCopy[0] = inData[0];
		_dataCopy[dF2 + 3] = _dataCopy[dF2 + 2] = inData[dF2];

		double wc = Math.Tan(cutOffFrequency * pi / samplingRate);
		double k1 = sqrt2 * wc; // Sqrt(2) * wc
		double k2 = wc * wc;
		double a = k2 / (1 + k1 + k2);
		double b = 2 * a;
		double c = a;
		double k3 = b / k2;
		double d = -2 * a + k3;
		double e = 1 - (2 * a) - k3;

		// RECURSIVE TRIGGERS - ENABLE filter is performed (first, last points constant)
		// double[] datYt = new double[dF2 + 4];
		datYt[1] = datYt[0] = inData[0];
		for (long s = 2; s < dF2 + 2; s++)
		{
			datYt[s] = a * _dataCopy[s] + b * _dataCopy[s - 1] + c * _dataCopy[s - 2]
					   + d * datYt[s - 1] + e * datYt[s - 2];
		}
		datYt[dF2 + 3] = datYt[dF2 + 2] = datYt[dF2 + 1];

		// FORWARD filter
		// double[] datZt = new double[dF2 + 2];
		datZt[dF2] = datYt[dF2 + 2];
		datZt[dF2 + 1] = datYt[dF2 + 3];
		for (long t = -dF2 + 1; t <= 0; t++)
		{
			datZt[-t] = a * datYt[-t + 2] + b * datYt[-t + 3] + c * datYt[-t + 4]
						+ d * datZt[-t + 1] + e * datZt[-t + 2];
		}

		// Calculated points copied for return
		for (long p = 0; p < dF2; p++)
		{
			data[p] = datZt[p];
		}

		return data;
	}
}