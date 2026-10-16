using UnityEngine;

namespace NCDK
{
    public static class RaycastHitExtensions
    {
        /// <summary>
        /// Material that owns the surface hit, for surface driven systems (footsteps, impact effects).
        /// A collider without a Renderer and MeshFilter on the same GameObject has no material to report.
        /// </summary>
        /// <remarks>
        /// The submesh is read from the MeshCollider mesh when there is one, because hit.triangleIndex
        /// belongs to it: static batching swaps the MeshFilter mesh for the combined one (measured on a
        /// static terrain: "Combined Mesh (root: scene)", 12 submeshes, not readable) while the collider
        /// keeps the original mesh (terrain, 3 submeshes), so looking the triangle up in the combined
        /// mesh always answered with the first material.
        /// When the submesh cannot be resolved the first material is returned instead of null: convex
        /// MeshColliders never report a triangle index (measured: hit.triangleIndex is -1 on a prop with
        /// a convex collider) and Meshes without Read/Write enabled cannot report their triangles.
        /// </remarks>
        public static Material GetMaterialAtHit(this RaycastHit hit) {
            if (!hit.collider.TryGetComponent(out Renderer renderer) || !hit.collider.TryGetComponent(out MeshFilter meshFilter))
            {
                return null;
            }

            var sharedMesh = meshFilter.sharedMesh;
            if (hit.collider is MeshCollider meshCollider && meshCollider.sharedMesh != null) {
                sharedMesh = meshCollider.sharedMesh;
            }
            if (sharedMesh == null) return null;

            var materialId = 0;

            if (sharedMesh.isReadable) {
                var triangleIndex = hit.triangleIndex;
                if (triangleIndex > -1) {
                    var resolvedSubMesh = FindSubMeshOfTriangle(sharedMesh, triangleIndex);
                    if (resolvedSubMesh > -1) materialId = resolvedSubMesh;
                }
            }

            var materials = renderer.sharedMaterials;
            if (materials == null || materialId >= materials.Length) return null;

            return materials[materialId];
        }

        /// <summary>
        /// Sub-mesh that owns the triangle, or -1 when the triangle cannot be matched.
        /// </summary>
        static int FindSubMeshOfTriangle(Mesh sharedMesh, int triangleIndex) {
            var triangles = sharedMesh.triangles;
            if (triangleIndex < 0 || (triangleIndex * 3) + 2 >= triangles.Length) return -1;

            int lookupIndex1 = triangles[triangleIndex * 3];
            int lookupIndex2 = triangles[triangleIndex * 3 + 1];
            int lookupIndex3 = triangles[triangleIndex * 3 + 2];
            var subMeshCount = sharedMesh.subMeshCount;

            for (int subMesh = 0; subMesh < subMeshCount; subMesh++) {
                var subMeshTriangles = sharedMesh.GetTriangles(subMesh);
                // Triangles are stored in groups of three: stepping one index at a time both wasted work
                // and read past the end of the last group.
                for (int j = 0; j + 2 < subMeshTriangles.Length; j += 3) {
                    if (subMeshTriangles[j] != lookupIndex1 ||
                        subMeshTriangles[j + 1] != lookupIndex2 ||
                        subMeshTriangles[j + 2] != lookupIndex3) continue;
                    return subMesh;
                }
            }

            return -1;
        }
    }
}
