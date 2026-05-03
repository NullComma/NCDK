using System;
using UnityEngine;

namespace NCDK
{
    /// <summary>
    /// Serializable wrapper for interface references backed by a <see cref="UnityEngine.Object"/>.
    /// <para>
    /// Unity cannot serialize interface fields directly. This class stores a concrete
    /// <see cref="UnityEngine.Object"/> reference (assignable in the Inspector) and resolves
    /// it to <typeparamref name="TInterface"/> after deserialization via
    /// <see cref="ISerializationCallbackReceiver"/>.
    /// </para>
    /// </summary>
    /// <typeparam name="TInterface">Interface type to expose at runtime.</typeparam>
    /// <typeparam name="TObject">Backing Unity object type for Inspector assignment.</typeparam>
    [Serializable]
    public class InterfaceReference<TInterface, TObject> : ISerializationCallbackReceiver
        where TObject : UnityEngine.Object
        where TInterface : class
    {
        [SerializeField] private TObject _underlyingObject;

        [NonSerialized] private TInterface _interfaceValue;

        /// <summary>
        /// Resolved interface value after deserialization.
        /// </summary>
        public TInterface Value => _interfaceValue;

        /// <summary>
        /// Backing Unity object assigned in the Inspector.
        /// </summary>
        public TObject UnderlyingObject => _underlyingObject;

        /// <summary>
        /// Returns <c>true</c> when the reference resolves to a valid interface instance.
        /// </summary>
        public bool HasValue => _interfaceValue != null;

        public InterfaceReference() { }

        public InterfaceReference(TObject underlyingObject)
        {
            _underlyingObject = underlyingObject;
            _interfaceValue = _underlyingObject as TInterface;
        }

        public void OnAfterDeserialize()
        {
            _interfaceValue = _underlyingObject as TInterface;
        }

        public void OnBeforeSerialize() { }
    }
}
