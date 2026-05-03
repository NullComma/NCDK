using System;
using UnityEngine;

namespace NCDK
{
    /// <summary>
    /// Wrapper for a SerializableGuid to allow custom Editor drawing and referencing.
    /// </summary>
    [Serializable]
    public struct SerializableGuidReference
    {
        [SerializeField]
        public SerializableGuid Value;

        public bool HasValue => Value != SerializableGuid.Empty;

        public SerializableGuidReference(SerializableGuid guid) => Value = guid;

        public static implicit operator SerializableGuid(SerializableGuidReference r) => r.Value;
        public static implicit operator SerializableGuidReference(SerializableGuid guid) => new SerializableGuidReference(guid);
    }
}