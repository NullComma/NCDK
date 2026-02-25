using System;
using UnityEngine;

namespace EnigmaCore
{
    /// <summary>
    /// Adds a button to the Inspector that executes the decorated method.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class ButtonAttribute : PropertyAttribute
    {
        public string Label { get; private set; }
        public float Height { get; private set; }

        public ButtonAttribute(string label = null, float height = 0)
        {
            Label = label;
            Height = height;
        }
    }
}