using System;
using System.Collections.Generic;
using UnityEngine;

namespace NCDK
{
    /// <summary>
    /// Serializable material swap configuration.
    /// A single rule replaces every occurrence of <see cref="SourceMaterial"/>
    /// with <see cref="ReplacementMaterial"/> in all configured targets.
    /// </summary>
    [Serializable]
    public sealed class MaterialSwapRule
    {
        [SerializeField]
        [Tooltip("Material to search for in the target renderers.")]
        private Material _sourceMaterial;

        [SerializeField]
        [Tooltip("Material that will replace every matching occurrence of the source material.")]
        private Material _replacementMaterial;

        [SerializeField]
        [Tooltip("Identifiable objects whose Renderer components should receive this swap.")]
        private List<SerializableGuidReference> _targetIds = new();

        /// <summary>
        /// Material that will be replaced.
        /// </summary>
        public Material SourceMaterial => _sourceMaterial;

        /// <summary>
        /// Material applied in place of <see cref="SourceMaterial"/>.
        /// </summary>
        public Material ReplacementMaterial => _replacementMaterial;

        /// <summary>
        /// Identifiable targets resolved through <see cref="IdentifiableMonoBehaviour.Registry"/>.
        /// </summary>
        public IReadOnlyList<SerializableGuidReference> TargetIds => _targetIds;

        /// <summary>
        /// Returns true when the rule has enough information to run.
        /// </summary>
        public bool IsValid => _sourceMaterial != null && _replacementMaterial != null;
    }

    /// <summary>
    /// Trigger that swaps materials on one or more <see cref="Renderer"/> components at runtime.
    /// <para>
    /// The trigger resolves renderers by <see cref="SerializableGuidReference"/> so it can work
    /// across scenes. Each configured rule replaces every matching slot in every resolved renderer.
    /// </para>
    /// </summary>
    [DisallowMultipleComponent]
    public class MaterialSwapTrigger : IdentifiableMonoBehaviourTrigger
    {
        [SerializeField]
        [Tooltip("Material swap rules executed when the trigger fires.")]
        private List<MaterialSwapRule> _swapRules = new();

        /// <summary>
        /// Read-only access to the configured swap rules.
        /// </summary>
        public IReadOnlyList<MaterialSwapRule> SwapRules => _swapRules;

        protected override void OnTrigger()
        {
            for (int ruleIndex = 0; ruleIndex < _swapRules.Count; ruleIndex++)
            {
                MaterialSwapRule rule = _swapRules[ruleIndex];
                if (rule == null)
                {
                    Debug.LogError($"[MaterialSwapTrigger] Swap rule at index {ruleIndex} is null on '{name}'.", this);
                    continue;
                }

                if (!rule.IsValid)
                {
                    Debug.LogError($"[MaterialSwapTrigger] Swap rule at index {ruleIndex} on '{name}' is missing a source or replacement material.", this);
                    continue;
                }

                var targets = rule.TargetIds;
                for (int targetIndex = 0; targetIndex < targets.Count; targetIndex++)
                {
                    if (!TryResolveRenderer(targets[targetIndex], out Renderer targetRenderer))
                    {
                        Debug.LogError($"[MaterialSwapTrigger] Could not resolve a Renderer for target index {targetIndex} in rule {ruleIndex} on '{name}'.", this);
                        continue;
                    }

                    ReplaceMaterialEverywhere(targetRenderer, rule.SourceMaterial, rule.ReplacementMaterial);
                }
            }
        }

        private static bool TryResolveRenderer(SerializableGuidReference targetId, out Renderer renderer)
        {
            renderer = null;

            if (targetId.Value == SerializableGuid.Empty)
            {
                return false;
            }

            if (!IdentifiableMonoBehaviour.Registry.TryGetValue(targetId.Value, out IdentifiableMonoBehaviour target))
            {
                Debug.LogError($"[MaterialSwapTrigger] No IdentifiableMonoBehaviour found with ID {targetId.Value}.", null);
                return false;
            }

            if (target is Component component)
            {
                renderer = component.GetComponent<Renderer>();
                return renderer != null;
            }

            return false;
        }

        private static void ReplaceMaterialEverywhere(Renderer targetRenderer, Material sourceMaterial, Material replacementMaterial)
        {
            var materials = targetRenderer.sharedMaterials;
            if (materials == null || materials.Length == 0)
            {
                return;
            }

            bool changed = false;
            for (int i = 0; i < materials.Length; i++)
            {
                if (materials[i] == sourceMaterial)
                {
                    materials[i] = replacementMaterial;
                    changed = true;
                }
            }

            if (changed)
            {
                targetRenderer.sharedMaterials = materials;
            }
        }
    }
}
