using System;
using System.Collections.Generic;
using EnigmaCore.DependencyInjection;
using UnityEngine;
using UnityEngine.Events;

#if UNITY_EDITOR
using UnityEditor;
using System.Reflection;
#endif

namespace EnigmaCore
{
    [Obsolete("Use BlockingEventsTrigger instead. Please use the 'MIGRATE' button in Inspector.", false)]
    public class CBlockingEventsTrigger : MonoBehaviour
    {
        [NonSerialized, Inject] CBlockingEventsManager _blockingEventsManager;

        // --- Combined 
#if ODIN_INSPECTOR
        [FoldoutGroup("Default")]
#else
        [Header("Default")]
#endif
        [SerializeField]
        public CUnityEventBool AnyBlockingEvent = new();

#if ODIN_INSPECTOR
        [FoldoutGroup("Default")]
#endif
        [SerializeField]
        public CUnityEventBool OnMenuOrPlayingCutsceneEvent = new();

        // --- Individual 
#if ODIN_INSPECTOR
        [FoldoutGroup("Individual")]
#else
        [Header("Individual")]
#endif
        [SerializeField]
        public CUnityEventBool OnMenuEvent = new();

#if ODIN_INSPECTOR
        [FoldoutGroup("Individual")]
#endif
        [SerializeField]
        public CUnityEventBool PlayingCutsceneEvent = new();

#if ODIN_INSPECTOR
        [FoldoutGroup("Individual")]
#endif
        [SerializeField]
        public CUnityEventBool PlayingCutsceneInvertedEvent = new();

        // --- Inverted 
#if ODIN_INSPECTOR
        [FoldoutGroup("Inverted")]
#else
        [Header("Inverted")]
#endif
        [SerializeField]
        public CUnityEventBool NotOnMenuEvent = new();

#if ODIN_INSPECTOR
        [FoldoutGroup("Inverted")]
#endif
        [SerializeField]
        public CUnityEventBool NotPlayingCutsceneEvent = new();

#if ODIN_INSPECTOR
        [FoldoutGroup("Inverted")]
#endif
        [SerializeField]
        public CUnityEventBool NotOnMenuAndNotPlayingCutsceneEvent = new();

        void Awake()
        {
            this.Inject();
            if (_blockingEventsManager != null)
                BlockingEvent(_blockingEventsManager.InMenuOrPlayingCutscene);
        }

        void OnEnable()
        {
            if (_blockingEventsManager != null)
                _blockingEventsManager.InMenuOrPlayingCutsceneEvent += BlockingEvent;
        }

        void OnDisable()
        {
            if (_blockingEventsManager != null)
                _blockingEventsManager.InMenuOrPlayingCutsceneEvent -= BlockingEvent;
        }

        void BlockingEvent(bool inMenuOrPlayingCutscene)
        {
            var isInMenu = _blockingEventsManager.IsInMenu;
            var isPlayingCutscene = _blockingEventsManager.IsPlayingCutscene;

            // inverted
            NotOnMenuEvent.Invoke(!isInMenu);
            NotPlayingCutsceneEvent.Invoke(!isPlayingCutscene);
            NotOnMenuAndNotPlayingCutsceneEvent.Invoke(!isInMenu && !isPlayingCutscene);
            
            AnyBlockingEvent.Invoke(inMenuOrPlayingCutscene);
            OnMenuOrPlayingCutsceneEvent.Invoke(isInMenu || isPlayingCutscene);
            OnMenuEvent.Invoke(isInMenu);
            PlayingCutsceneEvent.Invoke(isPlayingCutscene);
            PlayingCutsceneInvertedEvent.Invoke(!isPlayingCutscene);
        }
    }

#if UNITY_EDITOR
    [CustomEditor(typeof(CBlockingEventsTrigger))]
    public class CBlockingEventsTriggerEditor : Editor
    {
        // Nomes das propriedades dentro de StateUnityEvents
        private const string PROP_STATE = "State";           // Para eventos "Normais" (True)
        private const string PROP_STATE_INVERTED = "StateInverted"; // Para eventos "Invertidos" (False)

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            EditorGUILayout.Space(15);
            GUI.backgroundColor = new Color(0.4f, 1f, 0.4f); // Verde claro
            if (GUILayout.Button("MIGRATE TO NEW SYSTEM\n(Auto-Fix References)", GUILayout.Height(40)))
            {
                MigrateComponent();
            }
            GUI.backgroundColor = Color.white;
        }

        private void MigrateComponent()
        {
            CBlockingEventsTrigger oldComp = (CBlockingEventsTrigger)target;
            GameObject go = oldComp.gameObject;

            Undo.RegisterCompleteObjectUndo(go, "Migrate Blocking Events");

            // 1. Criar novo componente se não existir
            BlockingEventsTrigger newComp = go.GetComponent<BlockingEventsTrigger>();
            if (newComp == null)
            {
                newComp = Undo.AddComponent<BlockingEventsTrigger>(go);
            }

            SerializedObject oldSo = new SerializedObject(oldComp);
            SerializedObject newSo = new SerializedObject(newComp);
            newSo.Update();

            Debug.Log($"<color=cyan>[Migration]</color> Iniciando migração no objeto '{go.name}'...");

            // 2. Transferir Eventos
            
            // --- COMBINED (_blockingState) ---
            // AnyBlockingEvent -> State
            TransferCalls(oldSo, "AnyBlockingEvent", newSo, "_blockingState", PROP_STATE);
            // OnMenuOrPlayingCutsceneEvent -> State
            TransferCalls(oldSo, "OnMenuOrPlayingCutsceneEvent", newSo, "_blockingState", PROP_STATE);
            // NotOnMenuAndNotPlayingCutsceneEvent -> StateInverted (Pois é disparado quando NÃO está bloqueado)
            TransferCalls(oldSo, "NotOnMenuAndNotPlayingCutsceneEvent", newSo, "_blockingState", PROP_STATE_INVERTED);

            // --- MENU (_menuState) ---
            // OnMenuEvent -> State
            TransferCalls(oldSo, "OnMenuEvent", newSo, "_menuState", PROP_STATE);
            // NotOnMenuEvent -> StateInverted
            TransferCalls(oldSo, "NotOnMenuEvent", newSo, "_menuState", PROP_STATE_INVERTED);

            // --- CUTSCENE (_cutsceneState) ---
            // PlayingCutsceneEvent -> State
            TransferCalls(oldSo, "PlayingCutsceneEvent", newSo, "_cutsceneState", PROP_STATE);
            // PlayingCutsceneInvertedEvent -> StateInverted
            TransferCalls(oldSo, "PlayingCutsceneInvertedEvent", newSo, "_cutsceneState", PROP_STATE_INVERTED);
            // NotPlayingCutsceneEvent -> StateInverted
            TransferCalls(oldSo, "NotPlayingCutsceneEvent", newSo, "_cutsceneState", PROP_STATE_INVERTED);

            newSo.ApplyModifiedProperties();

            // 3. Corrigir Referências Externas
            FixExternalReferences(oldComp, newComp);

            // 4. Finalização
            if (EditorUtility.DisplayDialog("Migration Complete", 
                "Migration finished successfully.\n\nEvents copied and compatible references updated.\n\nDo you want to REMOVE the old component now?", 
                "Yes, Remove Old", "No, Keep Both"))
            {
                Undo.DestroyObjectImmediate(oldComp);
            }
        }

        private void FixExternalReferences(CBlockingEventsTrigger oldComp, BlockingEventsTrigger newComp)
        {
            // Busca todos os MonoBehaviours na cena ativa (incluindo inativos)
            MonoBehaviour[] allScripts = Resources.FindObjectsOfTypeAll<MonoBehaviour>();
            int fixCount = 0;
            List<string> unfixableRefs = new List<string>();

            foreach (var script in allScripts)
            {
                // Ignorar assets do projeto, focar apenas na cena
                if (EditorUtility.IsPersistent(script.gameObject)) continue;
                if (script == oldComp || script == newComp) continue;

                SerializedObject so = new SerializedObject(script);
                SerializedProperty iterator = so.GetIterator();
                bool scriptChanged = false;

                while (iterator.Next(true))
                {
                    if (iterator.propertyType == SerializedPropertyType.ObjectReference)
                    {
                        // Se encontrar uma referência apontando para o componente velho
                        if (iterator.objectReferenceValue == oldComp)
                        {
                            // Tenta atribuir o novo
                            iterator.objectReferenceValue = newComp;
                            
                            // Verifica se o Unity aceitou a atribuição (compatibilidade de tipo)
                            if (iterator.objectReferenceValue == newComp)
                            {
                                scriptChanged = true;
                                fixCount++;
                                Debug.Log($"<color=green>[Fixed]</color> Atualizado referência em '{script.name}' (Campo: {iterator.propertyPath})");
                            }
                            else
                            {
                                // Reverte para o velho pois o tipo não é compatível
                                iterator.objectReferenceValue = oldComp;
                                unfixableRefs.Add($"Script: {script.name} | Objeto: {script.gameObject.name} | Campo: {iterator.propertyPath}");
                            }
                        }
                    }
                }

                if (scriptChanged)
                    so.ApplyModifiedProperties();
            }

            // Relatório
            if (fixCount > 0)
                Debug.Log($"<color=green>[Success]</color> Total de referências corrigidas automaticamente: {fixCount}");

            if (unfixableRefs.Count > 0)
            {
                string msg = "<b>[ATENÇÃO]</b> Existem scripts referenciando 'CBlockingEventsTrigger' explicitamente no código.\n" +
                             "A ferramenta não pode corrigir isso pois o tipo da variável no código C# é diferente.\n" +
                             "Você precisa abrir esses scripts e mudar o tipo da variável para 'BlockingEventsTrigger':\n\n";
                foreach (var r in unfixableRefs) msg += $"- {r}\n";
                Debug.LogError(msg);
            }
        }

        private void TransferCalls(SerializedObject srcObj, string srcPropName, 
                                   SerializedObject dstObj, string dstStructName, string dstEventPropName)
        {
            SerializedProperty srcProp = srcObj.FindProperty(srcPropName);
            if (srcProp == null) return;

            // Encontrar a lista de chamadas (m_Calls) no evento antigo
            // Assume-se que CUnityEventBool herda de UnityEvent ou UnityEvent<T>
            SerializedProperty srcCalls = srcProp.FindPropertyRelative("m_PersistentCalls.m_Calls");
            if (srcCalls == null) return; // Talvez seja um wrapper customizado, ajuste se necessário

            if (srcCalls.arraySize == 0) return;

            // Encontrar o evento de destino dentro da struct StateUnityEvents
            SerializedProperty dstStruct = dstObj.FindProperty(dstStructName);
            if (dstStruct == null) { Debug.LogError($"Struct '{dstStructName}' não encontrada no novo script."); return; }

            SerializedProperty dstEvent = dstStruct.FindPropertyRelative(dstEventPropName);
            if (dstEvent == null) { Debug.LogError($"Evento '{dstEventPropName}' não encontrado dentro de '{dstStructName}'."); return; }

            SerializedProperty dstCalls = dstEvent.FindPropertyRelative("m_PersistentCalls.m_Calls");

            // Copiar
            int startIndex = dstCalls.arraySize;
            dstCalls.arraySize += srcCalls.arraySize;

            for (int i = 0; i < srcCalls.arraySize; i++)
            {
                SerializedProperty srcCall = srcCalls.GetArrayElementAtIndex(i);
                SerializedProperty dstCall = dstCalls.GetArrayElementAtIndex(startIndex + i);

                // Copia Target, Method, Mode, State
                dstCall.FindPropertyRelative("m_Target").objectReferenceValue = srcCall.FindPropertyRelative("m_Target").objectReferenceValue;
                dstCall.FindPropertyRelative("m_MethodName").stringValue = srcCall.FindPropertyRelative("m_MethodName").stringValue;
                dstCall.FindPropertyRelative("m_Mode").enumValueIndex = srcCall.FindPropertyRelative("m_Mode").enumValueIndex;
                dstCall.FindPropertyRelative("m_CallState").enumValueIndex = srcCall.FindPropertyRelative("m_CallState").enumValueIndex;

                // Copia Argumentos
                SerializedProperty srcArgs = srcCall.FindPropertyRelative("m_Arguments");
                SerializedProperty dstArgs = dstCall.FindPropertyRelative("m_Arguments");

                CopyArg(srcArgs, dstArgs, "m_ObjectArgument");
                CopyArg(srcArgs, dstArgs, "m_IntArgument");
                CopyArg(srcArgs, dstArgs, "m_FloatArgument");
                CopyArg(srcArgs, dstArgs, "m_StringArgument");
                CopyArg(srcArgs, dstArgs, "m_BoolArgument");
            }
        }

        private void CopyArg(SerializedProperty src, SerializedProperty dst, string name)
        {
            var s = src.FindPropertyRelative(name);
            var d = dst.FindPropertyRelative(name);
            if (s != null && d != null)
            {
                switch (s.propertyType)
                {
                    case SerializedPropertyType.ObjectReference: d.objectReferenceValue = s.objectReferenceValue; break;
                    case SerializedPropertyType.Integer: d.intValue = s.intValue; break;
                    case SerializedPropertyType.Float: d.floatValue = s.floatValue; break;
                    case SerializedPropertyType.String: d.stringValue = s.stringValue; break;
                    case SerializedPropertyType.Boolean: d.boolValue = s.boolValue; break;
                }
            }
        }
    }
#endif
}