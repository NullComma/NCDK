using System;
using UnityEngine;

namespace NullCore
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public class RequiredAttribute : PropertyAttribute
    {
        public readonly string Message;

        public RequiredAttribute(string message = "")
        {
            Message = message;
        }
    }
}
