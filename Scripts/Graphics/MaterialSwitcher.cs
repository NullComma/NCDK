using System;
using UnityEngine;

namespace EnigmaCore
{
    /// <summary>
    /// Switches the material of a Renderer between an original and an alternative material.
    /// </summary>
    [RequireComponent(typeof(Renderer))]
    public class MaterialSwitcher : MonoBehaviour
    {
        [Header("Configuration")]
        [Min(0)] [SerializeField] int materialIndex = 0;

        [Header("Materials")]
        [SerializeField, ReadOnly] Material originalMaterial;
        [SerializeField] Material alternativeMaterial;
    
        [SerializeField, ReadOnly] new Renderer renderer;

#if UNITY_EDITOR
        void Reset() => OnValidate();
        void OnValidate()
        {
            TryGetComponent(out renderer);
            if(materialIndex < renderer.sharedMaterials.Length)
            {
                originalMaterial = renderer.sharedMaterials[materialIndex];
            }
        }
#endif

        /// <summary>
        /// Sets the Renderer's material to the original or the alternative.
        /// </summary>
        /// <param name="useAlternative">If true, applies the alternative material. If false, applies the original.</param>
        public void SetAlternativeMaterial(bool useAlternative)
        {
            if (renderer == null)
            {
                Debug.LogError("Renderer not found on this GameObject.", this);
                return;
            }

            Material targetMaterial = useAlternative ? alternativeMaterial : originalMaterial;

            if (targetMaterial == null)
            {
                Debug.LogWarning($"Target material is not set (Alternative: {useAlternative}).", this);
                return;
            }
        
            // Accessing .materials creates a copy of the array. We must modify the copy and then assign it back.
            var currentMaterials = renderer.materials;

            // Safety check to ensure the index is within the bounds of the materials array.
            if (materialIndex >= currentMaterials.Length)
            {
                Debug.LogError($"Material Index {materialIndex} is out of bounds. This renderer has {currentMaterials.Length} materials.", this);
                return;
            }

            // Update the material at the specified index in our local copy.
            currentMaterials[materialIndex] = targetMaterial;
        
            // Assign the modified array back to the renderer.
            renderer.materials = currentMaterials;
        }
    }
}