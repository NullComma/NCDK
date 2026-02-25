using UnityEngine;

namespace EnigmaCore
{
    public static class RaycastHitExtensions
    {
        public static Material GetMaterialAtHit(this RaycastHit hit) {
            if (!hit.collider.TryGetComponent(out Renderer renderer) || !hit.collider.TryGetComponent(out MeshFilter meshFilter))
            {
                return null;
            }
            if (meshFilter.sharedMesh == null) return null;
            var sharedMesh = meshFilter.sharedMesh;

            var materialId = -1;

            if (!sharedMesh.isReadable || !(hit.collider is MeshCollider)) {
                materialId = 0;
            }
            else {
                var triangleIndex = hit.triangleIndex;
                if (triangleIndex <= -1) return null;
                var triangles = sharedMesh.triangles;
                int lookupIndex1 = triangles[triangleIndex * 3];
                int lookupIndex2 = triangles[triangleIndex * 3 + 1];
                int lookupIndex3 = triangles[triangleIndex * 3 + 2];
                var subMeshCount = sharedMesh.subMeshCount;

                for (int i = 0; i < subMeshCount; i++) {
                    var tr = sharedMesh.GetTriangles(i);
                    for (int j = 0; j < tr.Length; j++) {
                        if (tr[j] != lookupIndex1 || 
                            tr[j + 1] != lookupIndex2 || 
                            tr[j + 2] != lookupIndex3) continue;
                        materialId = i;
                        break;
                    }
                    if (materialId != -1) break;
                }

                if (materialId == -1) return null;
            }

            return renderer.sharedMaterials[materialId];
        }
        
    }
}