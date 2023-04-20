namespace AudioVisualizer.Utils;

public static class Arithmetics
{
    /// Gets average of two values
    /// <param name="left"></param>
    /// <param name="right"></param>
    /// <returns>Average value of given values.</returns>
    public static short Average(short left, short right)
    {
        return (short)((left + right) / 2);
    }

    /// Swap two values
    /// <typeparam name="T"></typeparam>
    /// <param name="first">Value 1</param>
    /// <param name="second">Value 2</param>
    /// <returns>Value 2</returns>
    public static T Swap<T>(this T first, ref T second)
    {
        T tmp = second;
        second = first;
        return tmp;
    }

    /// Returns <c>_base</c> to the power of <c>exp</c>
    public static int ToPowOf(this int _base, int exp)
    {
        int res = _base;
        for (int i = 0; i < exp; i++)
        {
            res *= _base;
        }

        return res;
    }

    /// Returns second power of <c>_base</c>
    public static double Pow2(this double _base)
    {
        return _base * _base;
    }
    /// Get absolute value of complex number.
    /// <param name="real">Real part of the complex number.</param>
    /// <param name="img">Imaginary part of the complex number.</param>
    /// <returns>Absolute value of the complex number.</returns>
    public static double GetComplexAbs(double real, double img)
    {
        return Math.Sqrt(real.Pow2() + img.Pow2());
    }
    /// Determines if n is a power of two.
    /// <param name="n"></param>
    /// <returns>True if it is power of two, false otherwise.</returns>
    public static bool IsPowOfTwo(int n)
    {
        if ((n & (n - 1)) != 0)
            return false;
        return true;
    }
}