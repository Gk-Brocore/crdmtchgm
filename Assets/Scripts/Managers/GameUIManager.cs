using Game.Variables;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Game.Managers
{
    public class GameUIManager : MonoBehaviour
    {
        [SerializeField]
        private IntVar scoreVar;
        [SerializeField]
        private TMP_Text txt_score;
        [SerializeField]
        private IntVar comboVar;
        [SerializeField]
        private TMP_Text txt_combo;

        private void OnEnable()
        {
            scoreVar.OnValueChanged += OnScoreChanged;
            comboVar.OnValueChanged += OnComboChanged;
        }

        private void OnDisable()
        {
            scoreVar.OnValueChanged -= OnScoreChanged;
            comboVar.OnValueChanged -= OnComboChanged;
        }

        private void OnComboChanged(int _newValue)
        {
            txt_combo.text = _newValue.ToString();
        }

        private void OnScoreChanged(int _newVal)
        {
            txt_score.text = _newVal.ToString();
        }

    }
}