using Game.Variables;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Game.UI
{
    public class TextUpdate : MonoBehaviour
    {
        [SerializeField]
        private FloatVar floatVar;
        [SerializeField] 
        private IntVar intVar;
        [SerializeField]
        private StringVar stringVar;
        [SerializeField]
        private TMPro.TMP_Text uiText;

        public FloatVar FloatVar { get => floatVar; set => floatVar = value; }
        public IntVar IntVar { get => intVar; set => intVar = value; }
        public StringVar StringVar { get => stringVar; set => stringVar = value; }
        public TMP_Text UiText { get => uiText; set => uiText = value; }

        private void OnEnable()
        {
            if (FloatVar != null)
                FloatVar.OnValueChanged += OnFloatValueChanged;
            if (IntVar != null)
                IntVar.OnValueChanged += OnIntValueChanged;
            if (StringVar!=null)
                StringVar.OnValueChanged += OnStringValueChanged;
        }

        private void OnIntValueChanged(int _value)
        {
            OnStringValueChanged(_value.ToString());
        }

        private void OnFloatValueChanged(float _value)
        {
            OnFloatValueChanged(_value);
        }

        private void OnStringValueChanged(string _value)
        {
            UiText.text = _value;
        }

        private void OnDisable()
        {
            if (FloatVar != null)
                FloatVar.OnValueChanged -= OnFloatValueChanged;
            if (IntVar != null)
                IntVar.OnValueChanged -= OnIntValueChanged;
            if (StringVar != null)
                StringVar.OnValueChanged -= OnStringValueChanged;
        }
    }
}