using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Variables
{
    [CreateAssetMenu(fileName = "New String Variable", menuName = "Variables/String Variable")]
    public class StringVar : BaseVar<string>
    {

        public override void UpdateValue(string _value)
        {
            value += _value;
            OnValueChanged?.Invoke(value);
        }

        public static implicit operator string(StringVar stringVar)
        {
            return stringVar.value;
        }
    }
}