using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Variables
{
    public abstract class BaseVar<T> : ScriptableObject
    {
        public T value;

        public Action<T> OnValueChanged;

        public virtual void SetValue(T _value)
        {
            value = _value; 
            OnValueChanged?.Invoke(value);
        }
        public virtual T GetValue() 
        {  
            return value; 
        }

        public virtual void UpdateValue(T _value)
        {
            SetValue(_value);
        }

    }
}