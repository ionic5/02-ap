using System;

namespace TaskForce.AP.Client.Core
{
    public struct Variant
    {
        private readonly string _stringValue;
        private readonly int _intValue;
        private readonly float _floatValue;

        public Variant(string stringValue)
        {
            _stringValue = stringValue;
            _intValue = 0;
            _floatValue = 0.0f;
        }

        public Variant(int value)
        {
            _intValue = value;
            _floatValue = value;
            _stringValue = value.ToString();
        }

        public Variant(float value)
        {
            _floatValue = value;
            _intValue = (int)Math.Floor(value);
            _stringValue = value.ToString();
        }

        public float AsFloat()
        {
            return _floatValue;
        }

        public int AsInt()
        {
            return _intValue;
        }

        public string AsString()
        {
            return _stringValue;
        }
    }
}
