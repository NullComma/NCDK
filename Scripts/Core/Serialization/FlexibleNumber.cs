using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace NullCore
{
    /// <summary>
    /// A float value that can be either a constant or a random range.
    /// </summary>
    [Serializable]
    public struct FlexibleFloat
    {
        public enum Mode { Constant, Range }

        [SerializeField] private Mode _mode;
        [SerializeField] private float _value;
        [SerializeField] private float _min;
        [SerializeField] private float _max;

        public Mode ValueMode => _mode;
        public float ConstantValue => _value;
        public float MinValue => _min;
        public float MaxValue => _max;

        public FlexibleFloat(float value)
        {
            _mode = Mode.Constant;
            _value = value;
            _min = 0f;
            _max = 0f;
        }

        public FlexibleFloat(float min, float max)
        {
            _mode = Mode.Range;
            _value = 0f;
            _min = min;
            _max = max;
        }

        /// <summary>
        /// Returns the constant value or a new random value within the range.
        /// </summary>
        public float GetValue()
        {
            return _mode == Mode.Constant ? _value : Random.Range(_min, _max);
        }

        public override string ToString()
        {
            return _mode == Mode.Constant ? _value.ToString("F2") : $"[{_min:F2}, {_max:F2}]";
        }
    }

    /// <summary>
    /// An integer value that can be either a constant or a random range.
    /// </summary>
    [Serializable]
    public struct FlexibleInt
    {
        public enum Mode { Constant, Range }

        [SerializeField] private Mode _mode;
        [SerializeField] private int _value;
        [SerializeField] private int _min;
        [SerializeField] private int _max;

        public Mode ValueMode => _mode;
        public int ConstantValue => _value;
        public int MinValue => _min;
        public int MaxValue => _max;

        public FlexibleInt(int value)
        {
            _mode = Mode.Constant;
            _value = value;
            _min = 0;
            _max = 0;
        }

        public FlexibleInt(int min, int max)
        {
            _mode = Mode.Range;
            _value = 0;
            _min = min;
            _max = max;
        }

        /// <summary>
        /// Returns the constant value or a new random value within the range (inclusive).
        /// </summary>
        public int GetValue()
        {
            return _mode == Mode.Constant ? _value : Random.Range(_min, _max + 1);
        }

        public override string ToString()
        {
            return _mode == Mode.Constant ? _value.ToString() : $"[{_min}, {_max}]";
        }
    }
}
