using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace EnigmaCore.Text
{
    [RequireComponent(typeof(TextMeshProUGUI))]
    public class CopyTextOnClick : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField] TextMeshProUGUI _text;

        #if UNITY_EDITOR
        [MenuItem("CONTEXT/TextMeshProUGUI/Add CopyTextOnClick component")]
        static void ConvertTextUGUI(MenuCommand data)
        {
            if (data.context is not TextMeshProUGUI comp) return;
            Undo.AddComponent<CopyTextOnClick>(comp.gameObject);
        }
        #endif
        
        void Awake()
        {
            TryGetComponent(out _text);
        }

        void Reset()
        {
            TryGetComponent(out _text);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (_text == null) return;
            GUIUtility.systemCopyBuffer = _text.text;
        }
    }
}