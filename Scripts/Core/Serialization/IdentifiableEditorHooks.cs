using System;
using System.Collections.Generic;
using UnityEngine;

namespace NCDK
{
    /// <summary>
    /// Static hooks to allow Runtime scripts to access Editor-only registry features without direct assembly references.
    /// </summary>
    public static class IdentifiableEditorHooks
    {
#if UNITY_EDITOR
        public static Func<SerializableGuid, List<UnityEngine.Object>> GetAllObjects;
        public static Func<UnityEngine.Object, string> GetObjectPath;
#endif
    }
}
