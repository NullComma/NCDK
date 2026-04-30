using System;
using UnityEngine;

namespace NCDK
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public class ShowInInspectorAttribute : Attribute { }
}