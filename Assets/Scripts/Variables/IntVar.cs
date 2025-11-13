using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Variables
{
    [CreateAssetMenu(fileName = "New Int Variable", menuName = "Variables/Int Variable")]
    public class IntVar : BaseVar<int>
    {
      
        public override void UpdateValue(int _value)
        {
            value += _value;
            OnValueChanged?.Invoke(value);
        }

        

        public static implicit operator int(IntVar intVar)
        {
            return intVar.value;
        }
    }
}