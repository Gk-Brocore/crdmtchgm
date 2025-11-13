using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Variables
{
    [CreateAssetMenu(fileName = "New Float Variable", menuName = "Variables/Float Variable")]
    public class FloatVar : ScriptableObject
    {
        [SerializeField]
        private float value;

        public Action<float> OnValueChanged;

        public void SetValue(float _newValue)
        {
            value = _newValue;
            OnValueChanged?.Invoke(value);
        }
        public float GetValue()
        {
            return value;
        }

        public void Increment(float _delta)
        {
            value += _delta;
            OnValueChanged?.Invoke(value);
        }

 

        public static implicit operator float(FloatVar _float)
        {
            return _float.value;
        }
    }
}