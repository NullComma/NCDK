using System;
using UnityEngine;

namespace NCDK
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, Inherited = true, AllowMultiple = false)]
    public class ReadOnlyAttribute : PropertyAttribute { }
}