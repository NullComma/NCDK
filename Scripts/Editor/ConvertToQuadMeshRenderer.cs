using UnityEngine;
using UnityEditor;

namespace NCDK.Editor
{
    public static class ConvertToQuadMeshRenderer
    {
        // ── Single via component context menu ──────────────────────────────
        [MenuItem("CONTEXT/SpriteRenderer/Replace with Quad MeshRenderer")]
        static void ReplaceWithQuadMeshRenderer(MenuCommand command)
        {
            if (command.context is SpriteRenderer sr)
                Replace(sr);
        }

        // ── Batch via GameObject menu ──────────────────────────────────────
        [MenuItem("GameObject/Replace SpriteRenderers with Quad MeshRenderers", false, 33)]
        static void ReplaceMultiple()
        {
            foreach (GameObject go in Selection.gameObjects)
            {
                foreach (SpriteRenderer sr in go.GetComponents<SpriteRenderer>())
                    Replace(sr);
            }
        }

        [MenuItem("GameObject/Replace SpriteRenderers with Quad MeshRenderers", true)]
        static bool ValidateReplaceMultiple()
        {
            foreach (GameObject go in Selection.gameObjects)
                if (go.GetComponent<SpriteRenderer>() != null)
                    return true;
            return false;
        }

        // ── Shared logic ───────────────────────────────────────────────────
        static void Replace(SpriteRenderer sr)
        {
            if (sr == null) return;

            GameObject go = sr.gameObject;
            Material mat = sr.sharedMaterial;
            int sortingLayerId = sr.sortingLayerID;
            int sortingOrder   = sr.sortingOrder;

            // Read sprite bounds for exact local-space match
            Sprite sprite = sr.sprite;
            Vector3[] verts;
            Vector2[] uvs;

            if (sprite != null)
            {
                Bounds b = sprite.bounds;
                float left   = b.min.x;
                float right  = b.max.x;
                float bottom = b.min.y;
                float top    = b.max.y;

                // Handle flipX / flipY by swapping the corresponding edges
                if (sr.flipX) { (left, right)   = (right, left);   }
                if (sr.flipY) { (bottom, top)   = (top, bottom);   }

                verts = new Vector3[]
                {
                    new Vector3(left,   bottom, 0),
                    new Vector3(right,  bottom, 0),
                    new Vector3(left,   top,    0),
                    new Vector3(right,  top,    0),
                };

                Vector2 texSize = new Vector2(sprite.textureRect.width, sprite.textureRect.height);
                if (texSize.x > 0f && texSize.y > 0f)
                {
                    float uLeft   = sprite.textureRect.xMin / sprite.texture.width;
                    float uRight  = sprite.textureRect.xMax / sprite.texture.width;
                    float vBottom = sprite.textureRect.yMin / sprite.texture.height;
                    float vTop    = sprite.textureRect.yMax / sprite.texture.height;

                    // When an axis is flipped, swap the corresponding UVs so the
                    // texture still faces forward.
                    if (sr.flipX) { (uLeft, uRight)   = (uRight, uLeft);   }
                    if (sr.flipY) { (vBottom, vTop)   = (vTop, vBottom);   }

                    uvs = new Vector2[]
                    {
                        new Vector2(uLeft,   vBottom),
                        new Vector2(uRight,  vBottom),
                        new Vector2(uLeft,   vTop),
                        new Vector2(uRight,  vTop),
                    };
                }
                else
                {
                    uvs = new Vector2[]
                    {
                        new Vector2(0, 0), new Vector2(1, 0),
                        new Vector2(0, 1), new Vector2(1, 1),
                    };
                }
            }
            else
            {
                // Fallback: unit quad (centered, 1x1)
                verts = new Vector3[]
                {
                    new Vector3(-0.5f, -0.5f, 0),
                    new Vector3( 0.5f, -0.5f, 0),
                    new Vector3(-0.5f,  0.5f, 0),
                    new Vector3( 0.5f,  0.5f, 0),
                };
                uvs = new Vector2[]
                {
                    new Vector2(0, 0), new Vector2(1, 0),
                    new Vector2(0, 1), new Vector2(1, 1),
                };
            }

            Mesh mesh = new Mesh
            {
                name = "Quad",
                vertices = verts,
                triangles = new int[] { 0, 2, 1, 2, 3, 1 },
                uv = uvs,
                normals = new Vector3[]
                {
                    Vector3.forward, Vector3.forward,
                    Vector3.forward, Vector3.forward,
                },
            };
            mesh.RecalculateBounds();

            Undo.RecordObject(go.transform, "Replace SpriteRenderer with Quad MeshRenderer");
            Undo.DestroyObjectImmediate(sr);

            MeshFilter mf = Undo.AddComponent<MeshFilter>(go);
            mf.sharedMesh = mesh;

            MeshRenderer mr = Undo.AddComponent<MeshRenderer>(go);
            mr.sharedMaterial = mat;

            // Copy sorting properties (Renderer base class)
            mr.sortingLayerID = sortingLayerId;
            mr.sortingOrder   = sortingOrder;
        }
    }
}
