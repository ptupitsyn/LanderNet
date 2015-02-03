using System;

namespace LanderNet.Game.Util
{
    internal class MovingAverage
    {
        public MovingAverage(int size)
        {
            if (size <= 0)
                throw new ArgumentException();
            _size = size;
            _values = new double[_size];
        }

        public double Add(double newValue)
        {
            var sumDelta = newValue - _values[_valuesIndex];
            _values[_valuesIndex] = newValue;
            _sum += sumDelta;

            _valuesIndex++;
            _valuesIndex %= _size;

            if (_valueCount < _size)
                _valueCount++;

            return _sum/_valueCount;
        }

        private readonly int _size;
        private readonly double[] _values;
        private double _sum;
        private int _valueCount;
        private int _valuesIndex;
    }
}