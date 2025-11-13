using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Variables
{
    [CreateAssetMenu(fileName = "New Float Variable", menuName = "Variables/Float Variable")]
    public class FloatVar : BaseVar<float>
    {

        public override void UpdateValue(float _value)
        {
            value += _value;
            OnValueChanged?.Invoke(value);
        }

        public static implicit operator float(FloatVar _float)
        {
            return _float.value;
        }
    }
}