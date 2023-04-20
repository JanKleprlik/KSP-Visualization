namespace AudioVisualizer.Utils;

/// Primitive value converters.
public static class Converter
{
	/// Converts an array of two bytes to one short.
 	/// <exception cref="ArgumentException">If argument is not of size 2.</exception>
	public static short BytesToShort(byte[] bytes)
	{
		if (bytes.Length != 2)
			throw new ArgumentException($"Exactly 2 bytes required, got {bytes.Length} bytes.");
		short res = bytes[1];
		res = (short)(res << 8);
		res += bytes[0];
		return res;
	}

	/// Converts an array of four bytes to one int.
	/// <exception cref="ArgumentException">If argument is not of size 4.</exception>
	public static int BytesToInt(byte[] bytes)
	{
		if (bytes.Length != 4)
			throw new ArgumentException($"Exactly 4 bytes required, got {bytes.Length} bytes.");
		int res = bytes[3];
		for (int i = 2; i >= 0; i--)
		{
			res <<= 8;
			res += bytes[i];
		}

		return res;
	}

	/// Converts an array of bytes to one uint.
	/// <exception cref="ArgumentException">If argument is not of size 4.</exception>
	public static uint BytesToUInt(byte[] bytes)
	{
		
		if (bytes.Length != 4)
			throw new ArgumentException($"Exactly 4 bytes required, got {bytes.Length} bytes.");
		
		uint res = bytes[3];
		for (int i = 2; i >= 0; i--)
		{
			res <<= 8;
			res += bytes[i];
		}

		return res;
	}


	/// Transforms byte audio data into short audio data
	public static short[] BytesToShorts(byte[] data)
	{
		int dataLength = data.Length % 2 == 0 ? data.Length : data.Length-1;

		short[] dataShorts = new short[dataLength / 2];
		for (int i = 0; i < dataLength; i += 2)
		{
			dataShorts[i / 2] = BytesToShort(new[] { data[i], data[i + 1] });
		}

		return dataShorts;
	}
}