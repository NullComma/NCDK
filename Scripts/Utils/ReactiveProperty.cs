using System;
using System.Collections.Generic;
using UnityEngine;

namespace EnigmaCore
{
    [Serializable]
    public class ReactiveProperty<T>
    {
        [SerializeField]
        T _value;

        public event Action<T> OnValueChanged = delegate { };

        public ReactiveProperty(T initialValue)
        {
            _value = initialValue;
        }

        public ReactiveProperty()
        {
            _value = default;
        }

        public T Value
        {
            get => _value;
            set
            {
                if(EqualityComparer<T>.Default.Equals(_value, value))
                {
                    return;
                }

                _value = value;
                OnValueChanged.Invoke(_value);
            }
        }

        public void Subscribe(Action<T> callback, bool fireImmediately = false)
        {
            OnValueChanged += callback;
            if(fireImmediately)
            {
                callback.Invoke(_value);
            }
        }

        public void Unsubscribe(Action<T> callback)
        {
            OnValueChanged -= callback;
        }

        public static implicit operator T(ReactiveProperty<T> property) => property.Value;

        public override string ToString() => _value?.ToString() ?? "null";
    }
}