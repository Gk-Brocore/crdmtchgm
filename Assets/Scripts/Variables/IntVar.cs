using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Variables
{
    [CreateAssetMenu(fileName = "New Int Variable", menuName = "Variables/Int Variable")]
    public class IntVar : ScriptableObject
    {
        public int value;

        public Action<int> OnValueChanged;

        public void SetValue(int newValue)
        {
            value = newValue;
            OnValueChanged?.Invoke(value);
        }
        public int GetValue()
        {
            return value;
        }

        public void Increment(int amount)
        {
            value += amount;
            OnValueChanged?.Invoke(value);
        }

        

        public static implicit operator int(IntVar intVar)
        {
            return intVar.value;
        }
    }
}