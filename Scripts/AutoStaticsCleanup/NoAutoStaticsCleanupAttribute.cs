#if !UNITY_6000_5_OR_NEWER
using System;

namespace Unity.Scripting.LifecycleManagement
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Struct | AttributeTargets.Field
            | AttributeTargets.Property | AttributeTargets.Event, AllowMultiple = false)]
    public class NoAutoStaticsCleanupAttribute : Attribute { }
}
#endif
