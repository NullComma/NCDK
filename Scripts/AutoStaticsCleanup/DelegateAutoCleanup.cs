#if !UNITY_6000_5_OR_NEWER
using System;
using System.Collections.Generic;

namespace UnityEngine
{
    public sealed class DelegateAutoCleanup
    {
        private static readonly List<DelegateAutoCleanup> Instances = new();

        private readonly Action _cleanup;
        private readonly string _ownerDescription;

        private DelegateAutoCleanup(Action cleanup, string ownerDescription)
        {
            _cleanup = cleanup ?? throw new ArgumentNullException(nameof(cleanup));
            _ownerDescription = ownerDescription;
            Instances.Add(this);
        }

        public static DelegateAutoCleanup CreateForPlayMode(Action cleanup, string ownerDescription = "")
            => new(cleanup, ownerDescription);

        public static IEnumerable<DelegateAutoCleanup> RegisteredInstances => Instances;

        public void Cleanup() => _cleanup();

        public override string ToString() => _ownerDescription;
    }
}
#endif
