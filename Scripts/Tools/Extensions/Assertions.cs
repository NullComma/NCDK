using UnityEngine;

namespace EnigmaCore
{
    public static class Assertions
    {
        public static void CAssertIfFalse(this bool condition, string errorMessage = null, Object source = null)
        {
            condition.ThrowIfFalse(errorMessage, source);
        }
        public static void ThrowIfFalse(this bool condition, string errorMessage = null, Object source = null)
        {
            if (condition) return;
            if (errorMessage == null) {
                Debug.LogError("Assertion failed", source);
                return;
            }

            Debug.LogError($"Assertion failed: {errorMessage}", source);
        }
    }
}