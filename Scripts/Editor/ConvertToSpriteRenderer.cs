using UnityEngine;
using UnityEditor;

namespace NCDK.Editor
{
    public static class ConvertToSpriteRenderer
    {
        [MenuItem("CONTEXT/MeshRenderer/Replace with SpriteRenderer")]
        static void ReplaceWithSpriteRenderer(MenuCommand command)
        {
            MeshRenderer mr = (MeshRenderer)command.context;
            if (mr == null) return;

            GameObject go = mr.gameObject;
            MeshFilter mf = go.GetComponent<MeshFilter>();
            Material mat = mr.sharedMaterial;

            Texture2D texture = null;
            if (mat != null)
            {
                if (mat.HasProperty("_MainTex"))
                    texture = mat.GetTexture("_MainTex") as Texture2D;
                else if (mat.HasProperty("_BaseMap"))
                    texture = mat.GetTexture("_BaseMap") as Texture2D;
            }

            Undo.RecordObject(go.transform, "Replace MeshRenderer with SpriteRenderer");
            if (mf != null) Undo.DestroyObjectImmediate(mf);
            Undo.DestroyObjectImmediate(mr);

            SpriteRenderer sr = Undo.AddComponent<SpriteRenderer>(go);
            sr.sharedMaterial = mat;

            if (texture != null)
            {
                string texPath = AssetDatabase.GetAssetPath(texture);
                if (!string.IsNullOrEmpty(texPath))
                {
                    foreach (Object asset in AssetDatabase.LoadAllAssetsAtPath(texPath))
                    {
                        if (asset is Sprite sprite)
                        {
                            sr.sprite = sprite;
                            break;
                        }
                    }
                }
            }
        }

    }
}
