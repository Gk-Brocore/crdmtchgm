using Game.Addressable;
using Game.Managers;
using System;
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
        public Image backSprite;
        public Image frontSprite; 
        private bool isRevealed = false;

        public string clickSound = "CardClick";

        private MemoryMatchManager matchManager;
        private Animator animator;

        private int frontAnimHash;
        private int BackAnimHash;

        private int matchHash;
        private int mismatchHash;

        private bool isAnimDone;

        public bool IsAnimDone { get => isAnimDone; set => isAnimDone = value; }

        private void Awake()
        {
            animator = GetComponent<Animator>();
            frontAnimHash = Animator.StringToHash("Front");
            BackAnimHash = Animator.StringToHash("Back");
            matchHash = Animator.StringToHash("Match");
            mismatchHash = Animator.StringToHash("Mismatch");

        }

        public void Initialize(string _id, Sprite _sprite, Vector2 _imgSize, MemoryMatchManager _manager, bool _showDebug = false)
        {

            if(_sprite == null)
            {
                SetHidden(); 
                return;
            }

            cardID = _id;
            matchManager = _manager;
            isMatched = false;
            isRevealed = false;

            if (frontSprite != null)
            {
                frontSprite.rectTransform.sizeDelta = _imgSize;
                frontSprite.sprite = _sprite;
            }

            if (debugLabel != null)
                debugLabel.text = _showDebug ? $"ID:{cardID}" : string.Empty;

            if (button != null)
            {
                button.onClick.RemoveAllListeners();
                button.onClick.AddListener(() => OnCardClick());
            }
        }

        private void OnCardClick()
        {
            AudioConductor.PlaySfx(clickSound);
            matchManager.EnqueueCard(this);
        }

        public void SetHidden()
        {
            cardImage.color = new Color(0, 0, 0, 0);
            if(button!=null) 
                button.interactable = false;
            backSprite.gameObject.SetActive(false);
            animator.enabled = false;
        }

        public void Reveal()
        {
            if (isRevealed || isMatched) 
                return;
            isRevealed = true;
            animator.SetTrigger(frontAnimHash);
        }

        public void Hide()
        {
            if (isMatched) 
                return;
            isRevealed = false;
            animator.SetTrigger(BackAnimHash);
        }

        public void SetMatched(bool matched)
        {
            isMatched = matched;
            if (matched)
            {
                if (button != null) 
                    button.interactable = false;

                animator.SetTrigger(matchHash);
            }
        }

        public void MatchedAnim()
        {
            isAnimDone = true;
        }

        public void Mismatch()
        {
            animator.SetTrigger(mismatchHash);
        }

        private void OnDisable()
        {
            button.onClick.RemoveAllListeners();
        }

        
    }
}