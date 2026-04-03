using System;
using UnityEngine;

namespace NullCore
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public class RequiredListLengthAttribute : PropertyAttribute
    {
        public readonly int MinLength;
        public readonly int MaxLength;

        public RequiredListLengthAttribute(int minLength = 0, int maxLength = int.MaxValue)
        {
            MinLength = minLength;
            MaxLength = maxLength;
        }
    }
}
