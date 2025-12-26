using System;
using UnityEngine;

namespace EnigmaCore
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public class MinMaxSliderAttribute : PropertyAttribute
    {
        public float Min { get; private set; }
        public float Max { get; private set; }
        public string MinMember { get; private set; }
        public string MaxMember { get; private set; }
        
        // Construtor Fixo (0 a 10)
        public MinMaxSliderAttribute(float min, float max)
        {
            Min = min;
            Max = max;
        }

        // Construtor Dinâmico ("_minVal", "_maxVal")
        public MinMaxSliderAttribute(string minMember, string maxMember)
        {
            MinMember = minMember;
            MaxMember = maxMember;
        }

        // Construtor Híbrido (0, "_maxVal")
        public MinMaxSliderAttribute(float min, string maxMember)
        {
            Min = min;
            MaxMember = maxMember;
        }
    }
}