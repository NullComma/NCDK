using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEditor;
using UnityEngine;

namespace EnigmaCore
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(MonoBehaviour), true)]
    public class GlobalEditor : UnityEditor.Editor
    {
        private Dictionary<string, object[]> _methodParameters = new Dictionary<string, object[]>();

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            // 1. Draw Default Fields manually to inject InfoBox
            DrawSerializedFields();

            // 2. Draw Custom Properties (Properties { get; set; })
            DrawCustomProperties();

            // 3. Draw Buttons
            DrawButtons();

            serializedObject.ApplyModifiedProperties();
        }

        private void DrawSerializedFields()
        {
            SerializedProperty iterator = serializedObject.GetIterator();
            bool enterChildren = true;

            while (iterator.NextVisible(enterChildren))
            {
                enterChildren = false;

                // Ignora o script field padrão para evitar duplicação se algo der errado, 
                // ou desenha desabilitado. O padrão Unity já desenha isso no topo geralmente, 
                // mas ao sobrescrever OnInspectorGUI, perdemos. Vamos desenhar:
                if (iterator.name == "m_Script")
                {
                    using (new EditorGUI.DisabledScope(true))
                    {
                        EditorGUILayout.PropertyField(iterator);
                    }
                    continue;
                }

                // Tenta pegar o FieldInfo para checar atributos
                FieldInfo fieldInfo = target.GetType().GetField(iterator.name, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
                
                if (fieldInfo != null)
                {
                    // Check InfoBox
                    var infoBoxes = fieldInfo.GetCustomAttributes(typeof(InfoBoxAttribute), true);
                    foreach (InfoBoxAttribute attr in infoBoxes)
                    {
                        bool isVisible = EnigmaEditorUtils.EvaluateCondition(target, attr.VisibleIfMemberName);
                        EnigmaEditorUtils.DrawInfoBox(attr.Message, attr.Type, isVisible);
                    }
                }

                EditorGUILayout.PropertyField(iterator, true);
            }
        }

        private void DrawCustomProperties()
        {
            var properties = target.GetType()
                .GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(p => p.GetCustomAttributes(typeof(ShowInInspectorAttribute), true).Length > 0)
                .ToArray();

            if (properties.Length == 0) return;

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Properties", EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            foreach (var prop in properties)
            {
                if (!prop.CanRead) continue;

                // 1. Check InfoBox on Property
                var infoBoxes = prop.GetCustomAttributes(typeof(InfoBoxAttribute), true);
                foreach (InfoBoxAttribute box in infoBoxes)
                {
                    bool isVisible = EnigmaEditorUtils.EvaluateCondition(target, box.VisibleIfMemberName);
                    EnigmaEditorUtils.DrawInfoBox(box.Message, box.Type, isVisible);
                }

                // 2. Prepare Drawing
                object value = prop.GetValue(target);
                bool isReadOnly = prop.GetCustomAttributes(typeof(ReadOnlyAttribute), true).Length > 0 || !prop.CanWrite;
                string label = ObjectNames.NicifyVariableName(prop.Name);
                Rect rect = EditorGUILayout.GetControlRect();

                // 3. Check for Special Drawers
                
                // MinMaxSlider
                var sliderAttr = (MinMaxSliderAttribute)prop.GetCustomAttribute(typeof(MinMaxSliderAttribute));
                if (sliderAttr != null)
                {
                    object newValue = EnigmaEditorUtils.DrawMinMaxSliderRaw(rect, label, value, prop.PropertyType, target, sliderAttr, isReadOnly);
                    if (!isReadOnly && !newValue.Equals(value)) prop.SetValue(target, newValue);
                    continue;
                }

                // ProgressBar
                var progressAttr = (ProgressBarAttribute)prop.GetCustomAttribute(typeof(ProgressBarAttribute));
                if (progressAttr != null && (prop.PropertyType == typeof(float) || prop.PropertyType == typeof(int)))
                {
                    // Nota: Precisaria mover a logica de desenho de label do DrawProgressBar para fora se quiser alinhar perfeito, 
                    // mas aqui vamos usar o rect total
                    EditorGUI.PrefixLabel(rect, new GUIContent(label));
                    // Ajusta rect para a barra (metade direita) ou desenha full se preferir
                    Rect barRect = new Rect(rect.x + EditorGUIUtility.labelWidth, rect.y, rect.width - EditorGUIUtility.labelWidth, rect.height);
                    
                    float current = Convert.ToSingle(value);
                    float min = progressAttr.Min;
                    // Busca Max dinamicamente se precisar (logica simplificada aqui)
                    float max = progressAttr.Max; 
                    if (!string.IsNullOrEmpty(progressAttr.MaxMemberName)) 
                        max = EnigmaEditorUtils.GetValueFromMember(target, progressAttr.MaxMemberName, max);

                    float newVal = EnigmaEditorUtils.DrawProgressBar(barRect, current, min, max, progressAttr, isReadOnly);
                    
                    if (!isReadOnly && Math.Abs(newVal - current) > 0.001f)
                    {
                        if (prop.PropertyType == typeof(int)) prop.SetValue(target, Mathf.RoundToInt(newVal));
                        else prop.SetValue(target, newVal);
                    }
                    continue;
                }

                // 4. Standard Draw
                DrawStandardProperty(rect, prop, value, isReadOnly);
            }
            EditorGUILayout.EndVertical();
        }

        private void DrawStandardProperty(Rect rect, PropertyInfo prop, object value, bool isReadOnly)
        {
            EditorGUI.BeginDisabledGroup(isReadOnly);
            string label = ObjectNames.NicifyVariableName(prop.Name);
            Type t = prop.PropertyType;

            if (t == typeof(float))
            {
                float val = EditorGUI.FloatField(rect, label, (float)value);
                if (!isReadOnly) prop.SetValue(target, val);
            }
            else if (t == typeof(int))
            {
                int val = EditorGUI.IntField(rect, label, (int)value);
                if (!isReadOnly) prop.SetValue(target, val);
            }
            else if (t == typeof(string))
            {
                string val = EditorGUI.TextField(rect, label, (string)value ?? "");
                if (!isReadOnly) prop.SetValue(target, val);
            }
            else if (t == typeof(bool))
            {
                bool val = EditorGUI.Toggle(rect, label, (bool)value);
                if (!isReadOnly) prop.SetValue(target, val);
            }
            else if (t == typeof(Vector2))
            {
                Vector2 val = EditorGUI.Vector2Field(rect, label, (Vector2)value);
                if (!isReadOnly) prop.SetValue(target, val);
            }
            else if (t == typeof(Vector3))
            {
                Vector3 val = EditorGUI.Vector3Field(rect, label, (Vector3)value);
                if (!isReadOnly) prop.SetValue(target, val);
            }
            else if (t == typeof(Color))
            {
                Color val = EditorGUI.ColorField(rect, label, (Color)value);
                if (!isReadOnly) prop.SetValue(target, val);
            }
            else if (typeof(UnityEngine.Object).IsAssignableFrom(t))
            {
                var val = EditorGUI.ObjectField(rect, label, (UnityEngine.Object)value, t, true);
                if (!isReadOnly) prop.SetValue(target, val);
            }
            else
            {
                EditorGUI.LabelField(rect, label, value?.ToString() ?? "null");
            }
            EditorGUI.EndDisabledGroup();
        }

        private void DrawButtons()
        {
            var methods = target.GetType()
                .GetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)
                .Where(m => m.GetCustomAttributes(typeof(ButtonAttribute), true).Length > 0)
                .ToArray();

            if (methods.Length == 0) return;

            EditorGUILayout.Space();
            EditorGUILayout.LabelField("Buttons", EditorStyles.boldLabel);
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);

            foreach (var method in methods)
            {
                var attr = (ButtonAttribute)method.GetCustomAttributes(typeof(ButtonAttribute), true)[0];
                var parameters = method.GetParameters();
                string key = method.Name + parameters.Length;

                if (!_methodParameters.ContainsKey(key) || _methodParameters[key].Length != parameters.Length)
                {
                    _methodParameters[key] = new object[parameters.Length];
                }

                // Parametros
                if (parameters.Length > 0)
                {
                    EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                    EditorGUILayout.LabelField($"Arguments for {method.Name}", EditorStyles.miniBoldLabel);
                    for (int i = 0; i < parameters.Length; i++)
                    {
                        _methodParameters[key][i] = DrawParameterField(parameters[i], _methodParameters[key][i]);
                    }
                    EditorGUILayout.EndVertical();
                }
                
                // InfoBox em métodos (sim, é possível)
                var infoBoxes = method.GetCustomAttributes(typeof(InfoBoxAttribute), true);
                foreach (InfoBoxAttribute box in infoBoxes)
                {
                    bool isVisible = EnigmaEditorUtils.EvaluateCondition(target, box.VisibleIfMemberName);
                    EnigmaEditorUtils.DrawInfoBox(box.Message, box.Type, isVisible);
                }

                // Botão
                string buttonLabel = string.IsNullOrEmpty(attr.Label) ? ObjectNames.NicifyVariableName(method.Name) : attr.Label;
                float height = attr.Height > 0 ? attr.Height : EditorGUIUtility.singleLineHeight * 1.5f;

                if (GUILayout.Button(buttonLabel, GUILayout.Height(height)))
                {
                    foreach (var t in targets)
                    {
                        // Inicializa structs default se null
                        for (int i = 0; i < parameters.Length; i++)
                        {
                            if (_methodParameters[key][i] == null && parameters[i].ParameterType.IsValueType)
                                _methodParameters[key][i] = Activator.CreateInstance(parameters[i].ParameterType);
                        }
                        method.Invoke(t, _methodParameters[key]);
                    }
                }
                EditorGUILayout.Space(2);
            }
            EditorGUILayout.EndVertical();
        }

        private object DrawParameterField(ParameterInfo paramInfo, object currentValue)
        {
            Type type = paramInfo.ParameterType;
            string label = ObjectNames.NicifyVariableName(paramInfo.Name);

            if (type == typeof(int)) return EditorGUILayout.IntField(label, currentValue == null ? 0 : (int)currentValue);
            if (type == typeof(float)) return EditorGUILayout.FloatField(label, currentValue == null ? 0f : (float)currentValue);
            if (type == typeof(string)) return EditorGUILayout.TextField(label, currentValue == null ? "" : (string)currentValue);
            if (type == typeof(bool)) return EditorGUILayout.Toggle(label, currentValue == null ? false : (bool)currentValue);
            if (type == typeof(Vector3)) return EditorGUILayout.Vector3Field(label, currentValue == null ? Vector3.zero : (Vector3)currentValue);
            if (type == typeof(Color)) return EditorGUILayout.ColorField(label, currentValue == null ? Color.white : (Color)currentValue);
            if (typeof(UnityEngine.Object).IsAssignableFrom(type))
            {
                return EditorGUILayout.ObjectField(label, (UnityEngine.Object)currentValue, type, true);
            }

            return null;
        }
    }
}
