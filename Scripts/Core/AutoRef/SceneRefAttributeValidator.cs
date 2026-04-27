using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using Object = UnityEngine.Object;
using System.Collections;
using System.Diagnostics.CodeAnalysis;

#if UNITY_EDITOR

using System.Text;
 using UnityEditor;
 
 #endif

namespace NullCore.Refs
{
    public static class SceneRefAttributeValidator
    {
        private static readonly IList<ReflectionUtil.AttributedField<SceneRefAttribute>> ATTRIBUTED_FIELDS_CACHE = new List<ReflectionUtil.AttributedField<SceneRefAttribute>>();

#if UNITY_EDITOR

        /// <summary>
        /// Validate all references for every script and every game object in the scene.
        /// </summary>
        [MenuItem("Tools/NullCore/Validate All Refs")]
        public static bool ValidateAllRefs()
        {
            var validationSuccess = true;
            var scripts = MonoImporter.GetAllRuntimeMonoScripts();
            
            var summary = new StringBuilder("<b>[NullCore AutoRef Validation]</b>\n");
            var validatedScriptCount = 0;
            var validatedObjectCount = 0;
            var validatedFieldCount = 0;
            var failedValidationDetails = new StringBuilder();

            foreach (var runtimeMonoScript in scripts)
            {
                var scriptType = runtimeMonoScript.GetClass();
                if (scriptType == null || !typeof(MonoBehaviour).IsAssignableFrom(scriptType))
                {
                    continue;
                }

                try
                {
                    ReflectionUtil.GetFieldsWithAttributeFromType(
                        scriptType,
                        ATTRIBUTED_FIELDS_CACHE,
                        BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance
                    );
                    
                    if (ATTRIBUTED_FIELDS_CACHE.Count == 0)
                    {
                        continue;
                    }

                    var objects = Object.FindObjectsByType(scriptType, FindObjectsSortMode.None);
                    if (objects.Length == 0)
                    {
                        continue;
                    }

                    validatedScriptCount++;
                    
                    for (int o = 0; o < objects.Length; o++)
                    {
                        var obj = objects[o] as MonoBehaviour;
                        if (obj == null) continue;

                        validatedObjectCount++;
                        validatedFieldCount += ATTRIBUTED_FIELDS_CACHE.Count;

                        if (!Validate(obj, ATTRIBUTED_FIELDS_CACHE, false))
                        {
                            validationSuccess = false;
                            failedValidationDetails.AppendLine($"- <b>{obj.name}</b> ({scriptType.Name})");
                        }
                    }
                }
                finally
                {
                    ATTRIBUTED_FIELDS_CACHE.Clear();
                }
            }

            summary.AppendLine($"Validated {validatedFieldCount} fields across {validatedObjectCount} objects from {validatedScriptCount} script types.");

            if (validationSuccess)
            {
                summary.Append("<color=green>All references are valid.</color>");
            }
            else
            {
                summary.Append($"<color=red><b>Validation Failed!</b></color> {failedValidationDetails.Length} objects have missing references:\n");
                summary.Append(failedValidationDetails);
            }
            
            Debug.Log(summary.ToString());
            
            return validationSuccess;
        }

        /// <summary>
        /// Validate a single components references, attempting to assign missing references
        /// and logging errors as necessary.
        /// </summary>
        [MenuItem("CONTEXT/Component/Validate Refs")]
        [SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Used as menu item action")]
        private static void ValidateRefs(MenuCommand menuCommand)
            => Validate(menuCommand.context as Component);

        /// <summary>
        /// Clean and validate a single components references. Useful in instances where (for example) Unity has
        /// incorrectly serialized a scene reference within a prefab.
        /// </summary>
        [MenuItem("CONTEXT/Component/Clean and Validate Refs")]
        [SuppressMessage("CodeQuality", "IDE0051:Remove unused private members", Justification = "Used as menu item action")]
        private static void CleanValidateRefs(MenuCommand menuCommand)
            => CleanValidate(menuCommand.context as Component);

#endif

        /// <summary>
        /// Validate a single components references, attempting to assign missing references
        /// and logging errors as necessary.
        /// </summary>
        public static void ValidateRefs(this Component c, bool updateAtRuntime = false)
            => Validate(c, updateAtRuntime);

        /// <summary>
        /// Validate a single components references, attempting to assign missing references
        /// and logging errors as necessary.
        /// </summary>
        public static void Validate(Component c, bool updateAtRuntime = false)
        {
            try
            {
                ReflectionUtil.GetFieldsWithAttributeFromType(
                    c.GetType(),
                    ATTRIBUTED_FIELDS_CACHE,
                    BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance
                );
                Validate(c, ATTRIBUTED_FIELDS_CACHE, updateAtRuntime);
            }
            finally
            {
                ATTRIBUTED_FIELDS_CACHE.Clear();
            }
        }

        /// <summary>
        /// Clean and validate a single components references. Useful in instances where (for example) Unity has
        /// incorrectly serialized a scene reference within a prefab.
        /// </summary>
        public static void CleanValidate(Component c, bool updateAtRuntime = false)
        {
            try
            {
                ReflectionUtil.GetFieldsWithAttributeFromType(
                    c.GetType(),
                    ATTRIBUTED_FIELDS_CACHE,
                    BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance
                );
                Clean(c, ATTRIBUTED_FIELDS_CACHE);
                Validate(c, ATTRIBUTED_FIELDS_CACHE, updateAtRuntime);
            }
            finally
            {
                ATTRIBUTED_FIELDS_CACHE.Clear();
            }
        }

        private static bool Validate(
            Component c,
            IList<ReflectionUtil.AttributedField<SceneRefAttribute>> requiredFields,
            bool updateAtRuntime
        )
        {
            if (requiredFields.Count == 0)
            {
                Debug.LogWarning($"{c.GetType().Name} has no required fields", c.gameObject);
                return true;
            }

            var validationSuccess = true;
            bool isUninstantiatedPrefab = PrefabUtil.IsUninstantiatedPrefab(c.gameObject);
            for (int i = 0; i < requiredFields.Count; i++)
            {
                ReflectionUtil.AttributedField<SceneRefAttribute> attributedField = requiredFields[i];
                SceneRefAttribute attribute = attributedField.Attribute;
                FieldInfo field = attributedField.FieldInfo;

                if (field.FieldType.IsInterface)
                {
                    throw new Exception($"{c.GetType().Name} cannot serialize interface {field.Name} directly, use InterfaceRef instead");
                }

                object fieldValue = field.GetValue(c);
                if (updateAtRuntime || !Application.isPlaying)
                {
                    fieldValue = UpdateRef(attribute, c, field, fieldValue);
                }

                if (isUninstantiatedPrefab)
                {
                    continue;
                }

                validationSuccess &= ValidateRef(attribute, c, field, fieldValue);
            }
            return validationSuccess;
        }

        private static void Clean(
            Component c,
            IList<ReflectionUtil.AttributedField<SceneRefAttribute>> requiredFields
        )
        {
            for (int i = 0; i < requiredFields.Count; i++)
            {
                ReflectionUtil.AttributedField<SceneRefAttribute> attributedField = requiredFields[i];
                SceneRefAttribute attribute = attributedField.Attribute;
                if (attribute.Loc == RefLoc.Anywhere)
                {
                    continue;
                }

                FieldInfo field = attributedField.FieldInfo;
                field.SetValue(c, null);
#if UNITY_EDITOR
                EditorUtility.SetDirty(c);
#endif
            }
        }

        private static object UpdateRef(
            SceneRefAttribute attr,
            Component component,
            FieldInfo field,
            object existingValue
        )
        {
            Type fieldType = field.FieldType;
            bool excludeSelf = attr.HasFlags(Flag.ExcludeSelf);
            bool isCollection = IsCollectionType(fieldType, out bool _, out bool isList);
            bool includeInactive = attr.HasFlags(Flag.IncludeInactive);

            ISerializableRef iSerializable = null;
            if (typeof(ISerializableRef).IsAssignableFrom(fieldType))
            {
                iSerializable = (ISerializableRef)(existingValue ?? Activator.CreateInstance(fieldType));
                fieldType = iSerializable.RefType;
                existingValue = iSerializable.SerializedObject;
            }

            if (attr.HasFlags(Flag.Editable))
            {
                bool isFilledArray = isCollection && (existingValue as IEnumerable).CountEnumerable() > 0;
                if (isFilledArray || existingValue is Object)
                {
                    // If the field is editable and the value has already been set, keep it.
                    return existingValue;
                }
            }

            Type elementType = fieldType;
            if (isCollection)
            {
                elementType = GetElementType(fieldType);
                if (typeof(ISerializableRef).IsAssignableFrom(elementType))
                {
                    Type interfaceType = elementType?
                        .GetInterfaces()
                        .FirstOrDefault(type =>
                            type.IsGenericType &&
                            type.GetGenericTypeDefinition() == typeof(ISerializableRef<>));

                    if (interfaceType != null)
                    {
                        elementType = interfaceType.GetGenericArguments()[0];
                    }
                }
            }

            object value = null;

            //INFO: when minimal unity version will be sufficiently high, explicit casts to object will not be necessary.
            switch (attr.Loc)
            {
                case RefLoc.Anywhere:
                    if (isCollection ? typeof(ISerializableRef).IsAssignableFrom(fieldType.GetElementType()) : iSerializable != null)
                    {
                        value = isCollection
                            ? (object)(existingValue as ISerializableRef[])?.Select(existingRef => GetComponentIfWrongType(existingRef.SerializedObject, elementType)).ToArray()
                            : (object)GetComponentIfWrongType(existingValue, elementType);
                    }
                    break;

                case RefLoc.Self:
                    value = isCollection
                        ? (object)component.GetComponents(elementType)
                        : (object)component.GetComponent(elementType);
                    break;

                case RefLoc.Parent:
                    value = isCollection
                        ? (object)GetComponentsInParent(component, elementType, includeInactive, excludeSelf)
                        : (object)GetComponentInParent(component, elementType, includeInactive, excludeSelf);
                    break;

                case RefLoc.Child:
                    value = isCollection
                        ? (object)GetComponentsInChildren(component, elementType, includeInactive, excludeSelf)
                        : (object)GetComponentInChildren(component, elementType, includeInactive, excludeSelf);
                    break;

                case RefLoc.Scene:
                    value = GetComponentsInScene(elementType, includeInactive, isCollection, excludeSelf);
                    break;
                default:
                    throw new Exception($"Unhandled Loc={attr.Loc}");
            }

            if (value == null)
            {
                return existingValue;
            }

            SceneRefFilter filter = attr.Filter;

            if (isCollection)
            {
                Type realElementType = GetElementType(fieldType);

                Array componentArray = (Array)value;
                if (filter != null)
                {
                    // TODO: probably a better way to do this without allocating a list
                    IList<object> list = new List<object>();
                    foreach (object o in componentArray)
                    {
                        if (filter.IncludeSceneRef(o))
                        {
                            list.Add(o);
                        }
                    }
                    componentArray = list.ToArray();
                }

                Array typedArray = Array.CreateInstance(
                    realElementType ?? throw new InvalidOperationException(),
                    componentArray.Length
                );

                if (elementType == realElementType)
                {
                    Array.Copy(componentArray, typedArray, typedArray.Length);
                    value = typedArray;
                }
                else if (typeof(ISerializableRef).IsAssignableFrom(realElementType))
                {
                    for (int i = 0; i < typedArray.Length; i++)
                    {
                        ISerializableRef elementValue = Activator.CreateInstance(realElementType) as ISerializableRef;
                        elementValue?.OnSerialize(componentArray.GetValue(i));
                        typedArray.SetValue(elementValue, i);
                    }
                    value = typedArray;
                }
            }
            else if (filter?.IncludeSceneRef(value) == false)
            {
                iSerializable?.Clear();
#if UNITY_EDITOR
                if (existingValue != null)
                {
                    EditorUtility.SetDirty(component);
                }
#endif
                return null;
            }

            if (iSerializable == null)
            {
                bool valueIsEqual = existingValue != null && 
                                    isCollection ? Enumerable.SequenceEqual((IEnumerable<object>)value, (IEnumerable<object>)existingValue) : value.Equals(existingValue);
                if (valueIsEqual)
                {
                    return existingValue;
                }

                if (isList)
                {
                    Type listType = typeof(List<>);
                    Type[] typeArgs = { fieldType.GenericTypeArguments[0] };
                    Type constructedType = listType.MakeGenericType(typeArgs);

                    object newList = Activator.CreateInstance(constructedType);

                    MethodInfo addMethod = newList.GetType().GetMethod(nameof(List<object>.Add));

                    foreach (object s in (IEnumerable)value)
                    {
                        addMethod.Invoke(newList, new [] { s });
                    }

                    field.SetValue(component, newList);
                }
                else
                {
                    field.SetValue(component, value);
                }
            }
            else
            {
                if (!iSerializable.OnSerialize(value))
                {
                    return existingValue;
                }
            }

#if UNITY_EDITOR
            EditorUtility.SetDirty(component);
#endif
            return value;
        }

        private static Type GetElementType(Type fieldType)
        {
            if (fieldType.IsArray)
            {
                return fieldType.GetElementType();
            }
            return fieldType.GenericTypeArguments[0];
        }

        private static object GetComponentIfWrongType(object existingValue, Type elementType)
        {
            if (existingValue is Component existingComponent && existingComponent && !elementType.IsInstanceOfType(existingValue))
            {
                return existingComponent.GetComponent(elementType);
            }

            return existingValue;
        }

        private static bool ValidateRef(SceneRefAttribute attr, Component c, FieldInfo field, object value)
        {
            Type fieldType = field.FieldType;
            bool isCollection = IsCollectionType(fieldType, out bool _, out bool _);
            bool isOverridable = attr.HasFlags(Flag.EditableAnywhere);

            if (value is ISerializableRef ser)
            {
                value = ser.SerializedObject;
            }

            if (IsEmptyOrNull(value, isCollection))
            {
                if (attr.HasFlags(Flag.Optional))
                    return true;

                Type elementType = isCollection ? fieldType.GetElementType() : fieldType;
                elementType = typeof(ISerializableRef).IsAssignableFrom(elementType) ? elementType?.GetGenericArguments()[0] : elementType;
                Debug.LogError($"{c.GetType().Name} missing required {elementType?.Name + (isCollection ? "[]" : "")} ref '{field.Name}'", c.gameObject);

                return false;
            }

            if (isCollection)
            {
                var validationSuccess = true;
                IEnumerable a = (IEnumerable)value;
                IEnumerator enumerator = a.GetEnumerator();

                Type elementType = fieldType.GetElementType();

                while (enumerator.MoveNext())
                {
                    object o = enumerator.Current;
                    if (o is ISerializableRef serObj)
                    {
                        o = serObj.SerializedObject;
                    }
                    
                    if (o != null)
                    {
                        if (isOverridable)
                            continue;
                        
                        if (attr.HasFlags(Flag.ExcludeSelf) && o is Component valueC &&
                            valueC.gameObject == c.gameObject)
                            Debug.LogError($"{c.GetType().Name} {elementType?.Name}[] ref '{field.Name}' cannot contain component from the same GameObject", c.gameObject);

                        validationSuccess &= ValidateRefLocation(attr.Loc, c, field, o);
                    }
                    else
                    {
                        Debug.LogError($"{c.GetType().Name} missing required element ref in array '{field.Name}'", c.gameObject);
                        validationSuccess = false;
                    }
                }
                return validationSuccess;
            }
            else
            {
                if (isOverridable)
                    return true;
                
                if (attr.HasFlags(Flag.ExcludeSelf) && value is Component valueC && valueC.gameObject == c.gameObject)
                    Debug.LogError($"{c.GetType().Name} {fieldType.Name} ref '{field.Name}' cannot be on the same GameObject", c.gameObject);

                return ValidateRefLocation(attr.Loc, c, field, value);
            }
        }

        private static bool ValidateRefLocation(RefLoc loc, Component c, FieldInfo field, object refObj)
        {
            switch (refObj)
            {
                case Component valueC:
                    return ValidateRefLocation(loc, c, field, valueC);

                case ScriptableObject _:
                    return ValidateRefLocationAnywhere(loc, c, field);

                case GameObject _:
                    return ValidateRefLocationAnywhere(loc, c, field);

                default:
                    throw new Exception($"{c.GetType().Name} has unexpected reference type {refObj?.GetType().Name}");
            }
        }

        private static bool ValidateRefLocation(RefLoc loc, Component c, FieldInfo field, Component refObj)
        {
            switch (loc)
            {
                case RefLoc.Anywhere:
                    break;

                case RefLoc.Self:
                    if (refObj.gameObject != c.gameObject)
                    {
                        Debug.LogError($"{c.GetType().Name} requires {field.FieldType.Name} ref '{field.Name}' to be on Self", c.gameObject);
                        return false;
                    }

                    break;

                case RefLoc.Parent:
                    if (!c.transform.IsChildOf(refObj.transform))
                    {
                        Debug.LogError($"{c.GetType().Name} requires {field.FieldType.Name} ref '{field.Name}' to be a Parent", c.gameObject);
                        return false;
                    }

                    break;

                case RefLoc.Child:
                    if (!refObj.transform.IsChildOf(c.transform))
                    {
                        Debug.LogError($"{c.GetType().Name} requires {field.FieldType.Name} ref '{field.Name}' to be a Child", c.gameObject);
                        return false;
                    }

                    break;

                case RefLoc.Scene:
                    if (c == null)
                    {
                        Debug.LogError($"{c.GetType().Name} requires {field.FieldType.Name} ref '{field.Name}' to be in the scene", c.gameObject);
                        return false;
                    }
                    break;

                default:
                    throw new Exception($"Unhandled Loc={loc}");
            }
            return true;
        }

        private static bool ValidateRefLocationAnywhere(RefLoc loc, Component c, FieldInfo field)
        {
            switch (loc)
            {
                case RefLoc.Anywhere:
                    return true;

                case RefLoc.Self:
                case RefLoc.Parent:
                case RefLoc.Child:
                case RefLoc.Scene:
                    Debug.LogError($"{c.GetType().Name} requires {field.FieldType.Name} ref '{field.Name}' to be Anywhere", c.gameObject);
                    return false;

                default:
                    throw new Exception($"Unhandled Loc={loc}");
            }
        }

        private static bool IsEmptyOrNull(object obj, bool isCollection)
        {
            if (obj is ISerializableRef ser)
            {
                return !ser.HasSerializedObject;
            }

            return obj == null || obj.Equals(null) || (isCollection && ((IEnumerable) obj).CountEnumerable() == 0);
        }

        private static bool IsCollectionType(Type t, out bool isArray, out bool isList)
        {
            isList = t.IsGenericType && t.GetGenericTypeDefinition() == typeof(List<>);
            isArray = t.IsArray;
            return isList || isArray;
        }
        
        private static Component[] GetComponentsInParent(Component c, Type elementType, bool includeInactive, bool excludeSelf)
        {
            var element = c;
            if (excludeSelf)
                element = c.transform.parent;

            return element == null
                ? Array.Empty<Component>()
                : element.GetComponentsInParent(elementType, includeInactive);
        }

        private static Component GetComponentInParent(Component c, Type elementType, bool includeInactive,
            bool excludeSelf)
        {
            var element = c;
            if (excludeSelf)
                element = c.transform.parent;

            return element == null
                ? null
                : element.GetComponentInParent(elementType, includeInactive);
        }

        private static Component[] GetComponentsInChildren(Component c, Type elementType, bool includeInactive, bool excludeSelf)
        {
            if (!excludeSelf)
                return c.GetComponentsInChildren(elementType, includeInactive);

            List<Component> components = new List<Component>();

            var transform = c.transform;
            int childCount = transform.childCount;

            for (int i = 0; i < childCount; ++i)
            {
                var child = transform.GetChild(i);
                components.AddRange(child.GetComponentsInChildren(elementType, includeInactive));
            }
            
            return components.ToArray();
        }
        
        private static Component GetComponentInChildren(Component c, Type elementType, bool includeInactive,
            bool excludeSelf)
        {
            if (!excludeSelf)
                return c.GetComponentInChildren(elementType, includeInactive);

            var transform = c.transform;
            int childCount = transform.childCount;

            for (int i = 0; i < childCount; ++i)
            {
                var child = transform.GetChild(i);
                var component = child.GetComponentInChildren(elementType, includeInactive);
                
                if (component != null)
                    return component;
            }
            
            return null;
        }

        private static object GetComponentsInScene(Type elementType, bool includeInactive, bool isCollection, bool excludeSelf)
        {
            bool isUnityType = elementType.IsSubclassOf(typeof(Object));
            
#if UNITY_2021_3_18_OR_NEWER
            if (isUnityType)
                return isCollection
                    ? (object)Object.FindObjectsByType(elementType, includeInactive ? FindObjectsInactive.Include : FindObjectsInactive.Exclude, FindObjectsSortMode.InstanceID)
                    : (object)Object.FindFirstObjectByType(elementType, includeInactive ? FindObjectsInactive.Include : FindObjectsInactive.Exclude);

            var elements = Object.FindObjectsByType<MonoBehaviour>(includeInactive ? FindObjectsInactive.Include : FindObjectsInactive.Exclude, FindObjectsSortMode.InstanceID);
#elif UNITY_2020_1_OR_NEWER
            if (isUnityType)
                return isCollection
                    ? (object)Object.FindObjectsByType(elementType, includeInactive ? FindObjectsInactive.Include : FindObjectsInactive.Exclude, FindObjectsSortMode.None)
                    : (object)Object.FindFirstObjectByType(elementType, includeInactive ? FindObjectsInactive.Include : FindObjectsInactive.Exclude);

            var elements = Object.FindObjectsByType<MonoBehaviour>(includeInactive ? FindObjectsInactive.Include : FindObjectsInactive.Exclude, FindObjectsSortMode.None);
#else
            if (isUnityType)
                return isCollection
                    ? (object)Object.FindObjectsOfType(elementType)
                    : (object)Object.FindObjectOfType(elementType);
            var elements = Object.FindObjectsOfType<MonoBehaviour>();
#endif
            elements = elements.Where(IsCorrectType).ToArray();
            if (isCollection)
                return (object)elements;

            return (object)(elements.Length > 0 ? elements[0] : null);
            
            bool IsCorrectType(MonoBehaviour e) => elementType.IsInterface ? e.GetType().GetInterfaces().Contains(elementType) : e.GetType().IsSubclassOf(elementType);
        }
    }
}
