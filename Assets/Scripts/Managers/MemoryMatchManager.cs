using Game.Cards;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace Game.Managers
{
    public class MemoryMatchManager : MonoBehaviour
    {
        public event Action<CardView, CardView> OnMatch;
        public event Action<CardView, CardView> OnMismatch;
        public event Action OnQueueCleared;

        public event Action<int> OnScoreUpdated;
        public event Action<int> OnComboUpdated;

        [Header("Scoring Settings")]
        public int baseMatchScore = 100;
        public int comboBonus = 25;
        public float comboDecayTime = 3f;

        [Header("Gameplay")]
        public float mismatchHideDelay = 1f;
        public bool continuousMatching = true;

        private Queue<CardView> selectionQueue = new Queue<CardView>();
        private int score = 0;
        private int comboCount = 0;
        private float lastMatchTime;

        public void EnqueueCard(CardView _card)
        {
            if (_card == null || _card.isMatched || selectionQueue.Contains(_card)) 
                return;

            Debug.Log($"Card Enqueued: {_card.cardID}");

            selectionQueue.Enqueue(_card);
            _card.Reveal();

            if (selectionQueue.Count >= 2)
            {
                var first = selectionQueue.Dequeue();
                var second = selectionQueue.Dequeue();
                CompareCards(first, second);
            }
        }

        private async void CompareCards(CardView _first, CardView _second)
        {
            if (_first.cardID == _second.cardID)
            {
                OnMatch?.Invoke(_first, _second);

                _first.SetMatched(true);
                _second.SetMatched(true);

                UpdateCombo();
                int _points = baseMatchScore + (comboBonus * (comboCount - 1));
                score += _points;
                OnScoreUpdated?.Invoke(score);
                OnComboUpdated?.Invoke(comboCount);
            }
            else
            {
                OnMismatch?.Invoke(_first, _second);

                // Reset combo if mismatch
                comboCount = 0;
                OnComboUpdated?.Invoke(comboCount);

                // Hide cards after small delay
                await Task.Delay(TimeSpan.FromSeconds(mismatchHideDelay));
                _first.Hide();
                _second.Hide();
            }

            OnQueueCleared?.Invoke();
        }

        private void UpdateCombo()
        {
            if (Time.time - lastMatchTime <= comboDecayTime)
            {
                comboCount++;
            }
            else
            {
                comboCount = 1;
            }

            lastMatchTime = Time.time;
        }

        public void ResetScore()
        {
            score = 0;
            comboCount = 0;
            lastMatchTime = 0;
            OnScoreUpdated?.Invoke(score);
            OnComboUpdated?.Invoke(comboCount);
        }

        public int GetScore() => score;
        public int GetCombo() => comboCount;
    }
}