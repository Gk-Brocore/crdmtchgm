using Game.Grid;
using Game.Variables;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Managers
{
    public class GameUIManager : MonoBehaviour
    {

        public GameObject button;

        public GameObject pnl_Grids;
        public GameObject pnl_Images;
        public GameObject pnl_Save;
        public GameObject pnl_Hud;
        public GameObject pnl_LevelComplete;

        public Transform gridButton;
        public Transform imageButton;


        private string currentGrid;
        private string currentImage;

        public GridController gridController;


        private void OnEnable()
        {
            gridController.LevelComplete += GridController_LevelComplete;
        }

       

        public void Start()
        {
            SetupMenu();
        }

        public void SetupMenu()
        {
            var _gameData = GameDataManager.Instance.GameData;
            foreach (var _item in _gameData.Grids.List)
            {
                GameObject _btn = Instantiate(button, gridButton);
                _btn.transform.SetParent(gridButton, false);
                TMP_Text _text = _btn.GetComponentInChildren<TMP_Text>();
                if (_text != null)
                {
                    _text.text = _item.value.gridName;
                }
                Button _buttonComp = _btn.GetComponent<Button>();
                _buttonComp.onClick.AddListener(() =>
                {
                    currentGrid = _item.value.gridName;
                    gridController.SetGridSettings(_item.value);
                    pnl_Grids.SetActive(false);
                    pnl_Images.SetActive(true);
                });
            }

            foreach (var _item in _gameData.ImageBanks.List)
            {
                GameObject _btn = Instantiate(button, imageButton);
                _btn.transform.SetParent(imageButton, false);
                TMP_Text _text = _btn.GetComponentInChildren<TMP_Text>();
                if (_text != null)
                {
                    _text.text = _item.value.BankName;
                }
                Button _buttonComp = _btn.GetComponent<Button>();
                _buttonComp.onClick.AddListener(() =>
                {
                    currentImage = _item.value.BankName;
                    gridController.SetImageBank(_item.value);
                    var _id = $"{currentGrid}_{currentImage}";
                    pnl_Images.SetActive(false);

                    if (GameDataManager.Instance.HasSave(currentGrid, currentImage) 
                        && !GameDataManager.Instance.IsLevelCompleted(currentGrid,currentImage))
                    {
                       
                        pnl_Save.SetActive(true);
                    }
                    else
                    {
                        NewGame();
                    }
                });
            }
        }

        public void NewGame()
        {
            pnl_Hud.SetActive(true);
            pnl_Save.SetActive(false);
            pnl_LevelComplete.SetActive(false);
            gridController.StartGame();
        }

        public void LoadGame()
        {
            pnl_Hud.SetActive(true);
            pnl_Save.SetActive(false);
            gridController.LoadGame();
        }

        public void Menu()
        {
            pnl_Hud.SetActive(false);
            pnl_LevelComplete.SetActive(false);
            pnl_Grids.SetActive(true);
        }
        private void GridController_LevelComplete()
        {
            pnl_Hud.SetActive(false);
            pnl_LevelComplete.SetActive(true);
        }

        private void OnDisable()
        {
            gridController.LevelComplete -= GridController_LevelComplete;
        }
    }
}