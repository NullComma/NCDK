using System;
using UnityEngine;

namespace NullCore
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public class ShowInInspectorAttribute : Attribute { }
}