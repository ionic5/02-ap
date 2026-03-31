using System;
using System.Numerics;

namespace TaskForce.AP.Client.Core
{
    public class Random
    {
        private static readonly System.Random _random;

        static Random()
        {
            _random = new System.Random();
        }

        public float Next(float a, float b)
        {
            return (float)NextDouble(a, b);
        }

        public double NextDouble()
        {
            return _random.NextDouble();
        }

        public double NextDouble(double a, double b)
        {
            var min = Math.Min(a, b);
            var max = Math.Max(a, b);
            return _random.NextDouble() * (max - min) + min;
        }

        public float NextFloat(float a, float b)
        {
            return (float)NextDouble(a, b);
        }

        public int Next(int maxValue)
        {
            return _random.Next(maxValue);
        }

        public int Next(int minValue, int maxValue)
        {
            return _random.Next(minValue, maxValue);
        }

        public Vector2 NextPosition(Vector2 center, float radius)
        {
            return NextPosition(center, 0.0f, radius);
        }

        public Vector2 NextPosition(Vector2 center, float minRadius, float maxRadius)
        {
            float theta = Next(0f, 2f * (float)Math.PI);

            float minRSq = minRadius * minRadius;
            float maxRSq = maxRadius * maxRadius;
            float r = (float)Math.Sqrt(Next(minRSq, maxRSq));

            float x = (float)(center.X + r * Math.Cos(theta));
            float y = (float)(center.Y + r * Math.Sin(theta));

            return new Vector2(x, y);
        }

        public void Shuffle<T>(T[] array)
        {
            int n = array.Length;
            while (n > 1)
            {
                int k = _random.Next(n--);
                T temp = array[n];
                array[n] = array[k];
                array[k] = temp;
            }
        }
    }
}
