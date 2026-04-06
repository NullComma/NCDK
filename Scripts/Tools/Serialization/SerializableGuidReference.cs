using System;
using UnityEngine;

namespace NullCore
{
    /// <summary>
    /// Wrapper for a SerializableGuid to allow custom Editor drawing and referencing.
    /// </summary>
    [Serializable]
    public struct SerializableGuidReference
    {
        public SerializableGuid Value;

        public static implicit operator SerializableGuid(SerializableGuidReference r) => r.Value;

        public SerializableGuidReference(SerializableGuid guid) => Value = guid;
    }
}