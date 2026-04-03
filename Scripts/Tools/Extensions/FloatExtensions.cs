using UnityEngine;

namespace NullCore
{
    public static class FloatExtensions 
    {
        /// <summary>
        /// Clamps an angle to the range [-360, 360] before clamping it between a min and max value.
        /// </summary>
        public static float ClampAngle(this float value, float min, float max) {
            if (value < -360)
                value += 360;
            if (value > 360)
                value -= 360;
            return Mathf.Clamp(value, min, max);
        }

        /// <summary>
        /// Clamps the float value between a minimum and maximum value.
        /// </summary>
        public static float Clamp(this float value, float min, float max) {
            return Mathf.Clamp(value, min, max);
        }
        
        /// <summary>
        /// Clamps the float value between 0.0 and 1.0.
        /// </summary>
        public static float Clamp01(this float value) {
            return Mathf.Clamp(value, 0f, 1f);
        }

        /// <summary>
        /// Remaps a float value from one range to another.
        /// </summary>
        public static float Remap(this float value, float beginOld, float endOld, float beginNew, float endNew) {
            return (value - beginOld) / (endOld - beginOld) * (endNew - beginNew) + beginNew;
        }

        /// <summary>
        /// Checks if the float value is within a specified range (inclusive).
        /// </summary>
        public static bool IsInRange(this float value, float a, float b) {
            return value >= a && value <= b;
        }

        /// <summary>
        /// Returns the absolute value of the float.
        /// </summary>
        public static float Abs(this float value) {
            return Mathf.Abs(value);
        }

        /// <summary>
        /// Returns whichever of the two provided values (a or b) is closer to the original float value.
        /// </summary>
        public static float GetCloserValue(this float value, float a, float b) {
            float distanceFromA = (a - value).Abs();
            float distanceFromB = (b - value).Abs();
            return distanceFromA < distanceFromB ? a : b;
        }

        /// <summary>
        /// Linearly interpolates between two float values.
        /// </summary>
        public static float Lerp(this float a, float b, float time) {
            return Mathf.Lerp(a, b, time);
        }

        /// <summary>
        /// Truncates a float to a specified number of decimal places.
        /// </summary>
        /// <param name="value">The float value to truncate.</param>
        /// <param name="decimalPlaces">The number of decimal places to keep. Must be non-negative.</param>
        /// <returns>The truncated float value.</returns>
        public static float Imprecise(this float value, int decimalPlaces = 3)
        {
            if (decimalPlaces < 0)
            {
                decimalPlaces = 0;
            }

            float multiplier = Mathf.Pow(10f, decimalPlaces);
            
            return ((int)(value * multiplier)) / multiplier;
        }
        
        /// <summary>
        /// Rounds a float to a specified number of decimal places.
        /// </summary>
        /// <param name="value">The float value to round.</param>
        /// <param name="decimalPlaces">The number of decimal places to round to. Must be non-negative.</param>
        /// <returns>The rounded float value.</returns>
        public static float Rounded(this float value, int decimalPlaces = 3)
        {
            if (decimalPlaces < 0)
            {
                decimalPlaces = 0;
            }

            float multiplier = Mathf.Pow(10f, decimalPlaces);
            
            return Mathf.Round(value * multiplier) / multiplier;
        }
    }
}