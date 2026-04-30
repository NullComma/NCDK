using UnityEngine;
using System.Collections.Generic;

namespace NCDK.Triggers
{
    /// <summary>
    /// A generic trigger to invoke methods on runtime using Reflection and Target IDs (SerializableGuid).
    /// </summary>
    public class InvokeMethodsTrigger : MonoBehaviour
    {
        [Header("Methods to Invoke")]
        [Tooltip("Configure method calls to be made by resolving the target object via its GUID reference.")]
        public List<ObjectMethodCall> methodCalls = new List<ObjectMethodCall>();

        public void Trigger()
        {
            foreach (var call in methodCalls)
            {
                call.InvokeCall(this.name);
            }
        }
    }
}
