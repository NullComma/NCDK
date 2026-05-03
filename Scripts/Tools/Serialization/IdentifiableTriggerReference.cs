using System;
using Object = UnityEngine.Object;
using UnityEngine;

namespace NCDK
{
    /// <summary>
    /// Concrete serializable reference for <see cref="IIdentifiableTrigger"/> targets.
    /// <para>
    /// Use this in serialized lists to allow drag-and-drop assignment of any
    /// <see cref="UnityEngine.Object"/> that implements <see cref="IIdentifiableTrigger"/>
    /// (for example, a <see cref="MonoBehaviour"/> or a <see cref="ScriptableObject"/>).
    /// </para>
    /// </summary>
    [Serializable]
    public sealed class IdentifiableTriggerReference : InterfaceReference<IIdentifiableTrigger, Object>
    {
        public IdentifiableTriggerReference() { }

        public IdentifiableTriggerReference(Object underlyingObject) : base(underlyingObject) { }
    }
}
