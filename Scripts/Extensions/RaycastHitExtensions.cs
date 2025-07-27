using UnityEngine;

namespace EnigmaCore
{
    public static class RaycastHitExtensions
    {
        public static Material GetMaterialAtHit(this RaycastHit hit)
        {
            if (!hit.collider.TryGetComponent(out Renderer renderer) || !hit.collider.TryGetComponent(out MeshFilter meshFilter))
            {
                return null;
            }
    
            Mesh mesh = meshFilter.sharedMesh;
            if (mesh == null || !mesh.isReadable || !(hit.collider is MeshCollider))
            {
                return renderer.sharedMaterial; 
            }

            int triangleIndex = hit.triangleIndex;
            int triangleCounter = 0;
    
            for (int i = 0; i < mesh.subMeshCount; i++)
            {
                var submesh = mesh.GetSubMesh(i);
                int numTrianglesInSubmesh = submesh.indexCount / 3;

                if (triangleIndex >= triangleCounter && triangleIndex < triangleCounter + numTrianglesInSubmesh)
                {
                    if (i < renderer.sharedMaterials.Length)
                    {
                        return renderer.sharedMaterials[i];
                    }
                    break; 
                }
                triangleCounter += numTrianglesInSubmesh;
            }

            return renderer.sharedMaterial;
        }
        
    }
}