using System;
using UnityEngine;

namespace EnigmaCore
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public class ProgressBarAttribute : PropertyAttribute
    {
        public float Min { get; private set; }
        public float Max { get; private set; }
        public string MaxMemberName { get; private set; }
        public bool IsEditable { get; private set; }
        public Color Color { get; private set; }

        public ProgressBarAttribute(float min, float max, float r = 0.3f, float g = 0.8f, float b = 0.3f)
        {
            Min = min;
            Max = max;
            IsEditable = true;
            Color = new Color(r, g, b);
        }

        public ProgressBarAttribute(float min, string maxMemberName, float r = 0.3f, float g = 0.8f, float b = 0.3f)
        {
            Min = min;
            MaxMemberName = maxMemberName;
            IsEditable = true;
            Color = new Color(r, g, b);
        }
    }
}