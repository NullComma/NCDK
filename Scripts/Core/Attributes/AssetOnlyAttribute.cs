using System;
using UnityEngine;

namespace NCDK
{
    [AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public class AssetOnlyAttribute : PropertyAttribute { }
}