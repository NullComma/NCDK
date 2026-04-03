using System;
using UnityEngine;

namespace NullCore
{
    [AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public class AssetOnlyAttribute : PropertyAttribute { }
}