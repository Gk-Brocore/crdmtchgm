using Game.Managers;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Cards
{
    public class CardView : MonoBehaviour
    {
        public string cardID;
        public bool isMatched = false;
        [Header("UI Elements")]
        public Image cardImage;
        public Button button;
        public TextMeshProUGUI debugLabel;

        [Header("Flip States")]
        public Sprite backSprite;
        public Sprite frontSprite; 
        private bool isRevealed = false;

        private MemoryMatchManager matchManager;

        public void Initialize(string _id, Sprite _sprite, MemoryMatchManager _manager, bool _showDebug = false)
        {
            cardID = _id;
            frontSprite = _sprite;
            matchManager = _manager;
            isMatched = false;
            isRevealed = false;

            if (cardImage != null)
                cardImage.sprite = backSprite;

            if (debugLabel != null)
                debugLabel.text = _showDebug ? $"ID:{cardID}" : string.Empty;

            if (button != null)
            {
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(() => matchManager.EnqueueCard(this));
            }
        }

        public void Reveal()
        {
            if (isRevealed || isMatched) 
                return;
            isRevealed = true;
            if (cardImage != null && frontSprite != null)
                cardImage.sprite = frontSprite;
        }

        public void Hide()
        {
            if (isMatched) 
                return;
            isRevealed = false;
            if (cardImage != null)
                cardImage.sprite = backSprite;
        }

        public void SetMatched(bool matched)
        {
            isMatched = matched;
            if (matched)
            {
                if (button != null) 
                    button.interactable = false;
            }
        }
    }
}