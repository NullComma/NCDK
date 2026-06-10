using System;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace NCDK
{
    public static class ObfuscatedConstants
    {
        const uint SteamKey = 0xABCD1234u;
        const uint SteamMask = 0xAB896000u; // 4500460 ^ SteamKey

        const uint DemoSteamMask = 0xAB85F098u; // 4776780 ^ SteamKey

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static uint DecodeSteamAppId(bool isDemo)
        {
            if (isDemo)
                return DemoSteamMask ^ SteamKey;
            return SteamMask ^ SteamKey;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static uint DecodeUint(uint masked, uint key)
        {
            return masked ^ key;
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        public static string DecodeString(byte[] data, byte[] key)
        {
            int len = Mathf.Min(data.Length, key.Length);
            char[] chars = new char[len];
            for (int i = 0; i < len; i++)
                chars[i] = (char)(data[i] ^ key[i]);
            return new string(chars);
        }
    }
}
