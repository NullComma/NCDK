using System;
using UnityEngine;

namespace NullCore
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public class ReadOnlyAttribute : PropertyAttribute { }
}