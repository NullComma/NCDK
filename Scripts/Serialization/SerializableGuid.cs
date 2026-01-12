using System;
using System.Runtime.InteropServices;
using UnityEngine;
#if UNITY_NETCODE
using Unity.Netcode;
#endif

namespace EnigmaCore
{
    /// <summary>
    /// Represents a globally unique identifier (GUID) that is serializable with Unity and usable in game scripts.
    /// From: https://github.com/adammyhre/Unity-Inventory-System/tree/master/Assets/_Project/Scripts/Inventory/Helpers/SerializableGuid.cs
    /// Modified by Christopher Ravailhe.
    /// </summary>
    [Serializable]
    [StructLayout(LayoutKind.Sequential)]
    public struct SerializableGuid : IEquatable<SerializableGuid>
#if UNITY_NETCODE
        , INetworkSerializable
#endif
    {
        [SerializeField, HideInInspector] public uint Part1;
        [SerializeField, HideInInspector] public uint Part2;
        [SerializeField, HideInInspector] public uint Part3;
        [SerializeField, HideInInspector] public uint Part4;

        public static SerializableGuid NewGuid() => new SerializableGuid(Guid.NewGuid());
        public static readonly SerializableGuid Empty = new(0, 0, 0, 0);

        public SerializableGuid(uint val1, uint val2, uint val3, uint val4)
        {
            Part1 = val1;
            Part2 = val2;
            Part3 = val3;
            Part4 = val4;
        }

        public SerializableGuid(Guid guid)
        {
#if UNITY_2021_2_OR_NEWER
            // Optimized "Safe" approach using Span on the Stack (Zero Allocation).
            Span<byte> guidBytes = stackalloc byte[16];
            guid.TryWriteBytes(guidBytes);

            Span<uint> uints = MemoryMarshal.Cast<byte, uint>(guidBytes);
            Part1 = uints[0];
            Part2 = uints[1];
            Part3 = uints[2];
            Part4 = uints[3];
#else
            // Legacy fallback (Allocates byte array)
            byte[] bytes = guid.ToByteArray();
            Part1 = BitConverter.ToUInt32(bytes, 0);
            Part2 = BitConverter.ToUInt32(bytes, 4);
            Part3 = BitConverter.ToUInt32(bytes, 8);
            Part4 = BitConverter.ToUInt32(bytes, 12);
#endif
        }

        public Guid ToGuid()
        {
#if UNITY_2021_2_OR_NEWER
            Span<uint> uints = stackalloc uint[4];
            uints[0] = Part1;
            uints[1] = Part2;
            uints[2] = Part3;
            uints[3] = Part4;

            // Guid constructor accepting ReadOnlySpan<byte> (Zero Alloc)
            return new Guid(MemoryMarshal.Cast<uint, byte>(uints));
#else
            // Legacy fallback
            var bytes = new byte[16];
            BitConverter.GetBytes(Part1).CopyTo(bytes, 0);
            BitConverter.GetBytes(Part2).CopyTo(bytes, 4);
            BitConverter.GetBytes(Part3).CopyTo(bytes, 8);
            BitConverter.GetBytes(Part4).CopyTo(bytes, 12);
            return new Guid(bytes);
#endif
        }

        /// <summary>
        /// Returns a URL-safe Base64 string representation of the GUID.
        /// Useful for compact serialization (e.g., JSON or file names).
        /// </summary>
        public string ToShortString()
        {
#if UNITY_2021_2_OR_NEWER
            Span<uint> uints = stackalloc uint[4];
            uints[0] = Part1;
            uints[1] = Part2;
            uints[2] = Part3;
            uints[3] = Part4;

            ReadOnlySpan<byte> bytes = MemoryMarshal.Cast<uint, byte>(uints);
            
            // Convert.ToBase64String accepting Span was added in .NET Standard 2.1
            string base64 = Convert.ToBase64String(bytes);
#else
            var bytesArray = new byte[16];
            BitConverter.GetBytes(Part1).CopyTo(bytesArray, 0);
            BitConverter.GetBytes(Part2).CopyTo(bytesArray, 4);
            BitConverter.GetBytes(Part3).CopyTo(bytesArray, 8);
            BitConverter.GetBytes(Part4).CopyTo(bytesArray, 12);
            
            string base64 = Convert.ToBase64String(bytesArray);
#endif
            return base64.Replace('+', '-').Replace('/', '_').TrimEnd('=');
        }

        public static SerializableGuid FromShortString(string shortString)
        {
            if (string.IsNullOrEmpty(shortString)) return Empty;

            string base64 = shortString.Replace('-', '+').Replace('_', '/');
            
            switch (base64.Length % 4)
            {
                case 2: base64 += "=="; break;
                case 3: base64 += "="; break;
            }
    
            try 
            {
                byte[] bytes = Convert.FromBase64String(base64);
                return new SerializableGuid(new Guid(bytes));
            } 
            catch 
            {
                return Empty;
            }
        }

#if UNITY_NETCODE
        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref Part1);
            serializer.SerializeValue(ref Part2);
            serializer.SerializeValue(ref Part3);
            serializer.SerializeValue(ref Part4);
        }
#endif

        public static implicit operator Guid(SerializableGuid serializableGuid) => serializableGuid.ToGuid();
        public static implicit operator SerializableGuid(Guid guid) => new SerializableGuid(guid);

        public override bool Equals(object obj) => obj is SerializableGuid other && Equals(other);
        public bool Equals(SerializableGuid other) => Part1 == other.Part1 && Part2 == other.Part2 && Part3 == other.Part3 && Part4 == other.Part4;
        public override int GetHashCode() => HashCode.Combine(Part1, Part2, Part3, Part4);
        public static bool operator ==(SerializableGuid left, SerializableGuid right) => left.Equals(right);
        public static bool operator !=(SerializableGuid left, SerializableGuid right) => !left.Equals(right);
        public override string ToString() => ToGuid().ToString();
    }
}