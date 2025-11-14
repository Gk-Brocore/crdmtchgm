using Game.Addressable;
using Game.Cards;
using Game.Images;
using Game.Managers;
using Game.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.UI;

namespace Game.Grid
{
    public class GridController : MonoBehaviour
    {
        [Header("Grid Setup")]

        [SerializeField]
        private GridSettings gridSettings;
        [SerializeField]
        private ImageBank imageBank;
        [SerializeField]
        private MemoryMatchManager matchQueue;
        [Header("Game")]
        [SerializeField]
        private float firstRevealedTime;

        [Header("UI Elements")]
        [SerializeField]
        private GridLayout gridLayout;
        [SerializeField]
        private AssetReference cardPrefab; 


        private GridContainer<GridCellData> grid;
        private List<CardView> spawnedCards = new List<CardView>();

        private RectTransform rectTransform;

        private int matchesFound = 0;

        private void Awake()
        {
            rectTransform = GetComponent<RectTransform>();

            if (gridLayout == null)
                gridLayout = GetComponent<GridLayout>();
        }

        private void Start()
        {

            if (!ValidateSetup())
                return;

            if (GameDataManager.Instance.HasSave(gridSettings.gridName,imageBank.BankName))
            {
                LoadGame();
            }
            else
            { 
                StartGame();
            }
        }

        private void LoadGame()
        {
            List<GridCellData> _savedGrid = GameDataManager.Instance.GetGridData(gridSettings.gridName, imageBank.BankName);
            InitilizeGrid();
            grid.FromList(_savedGrid);
            SetupUILayout();
            SpawnUICards(true);
        }

        private void StartGame()
        {

            InitilizeGrid();

            SetupUILayout();

            SpawnUICards(false);

            SaveCurrentGrid();


        }

        private void InitilizeGrid()
        {

            gridSettings.CalulateUseable();
            grid = new GridContainer<GridCellData>(
                gridSettings.Width,
                gridSettings.Height,
                gridSettings.CellSize,
                gridSettings.CellSpacing,
                gridSettings.Origin,
                () => CreateNewCell()
            );

            foreach (var _cell in gridSettings.cells)
            {
                var _gridCell = grid.GetValue(_cell.position.x, _cell.position.y);
                _gridCell.state = _cell.state;
            }

        }

        private GridCellData CreateNewCell()
        {
            return new GridCellData("", GridSettings.State.Hidden);
        }

        private bool ValidateSetup()
        {
            if (gridSettings == null || cardPrefab == null  || matchQueue == null)
            {
                Debug.LogError("GridUIController setup missing references!", this);
                return false;
            }
            return true;
        }
        private void SetupUILayout()
        {
            if (gridLayout == null)
            {
                gridLayout = gameObject.AddComponent<GridLayout>();
            }

            gridLayout.fitType = GridLayout.FitType.FIXEDROWS;
            gridLayout.rows = gridSettings.Height;
            gridLayout.columns = gridSettings.Width;
            gridLayout.spacing = gridSettings.CellSpacing;
            gridLayout.cellSize = gridSettings.CellSize;
            gridLayout.padding = gridSettings.Padding;

        }

        private async void SpawnUICards(bool _load)
        {

            foreach (var _card in spawnedCards)
            {
                if (_card != null)
                    Destroy(_card);
            }
            spawnedCards.Clear();


            
            var _imagesId = imageBank.GetShuffled(gridSettings.TotalCombinations);

            var _deck = new List<string>();
            if(!_load)
            {
                _deck = MakeDeck(_imagesId);
            }
          


            int _index = 0;
            for (int y = 0; y < gridSettings.Height; y++)
            {
                for (int x = 0; x < gridSettings.Width; x++)
                {

                    var _gridPos = new Vector2Int(x, y);
                    var _data = grid.GetValue(x, y);
                   

                    GameObject _card = await AddressableManager.Instance.InstantiateAsync(cardPrefab, Vector3.zero, Quaternion.identity, gridLayout.transform);
                    _card.name = $"Card_{x}_{y}";
                    var _cardView = _card.GetComponent<CardView>();

                    spawnedCards.Add(_cardView);
                    if (_data.state != GridSettings.State.Hidden && !_data.matched)
                    {
                        string _id = _load ? _data.id : _deck[_index];
                        if (!_load)
                            _data.id = _id;

                        var _sprite = await imageBank.GetSprite(_id);
                        _cardView.Initialize(_id, _gridPos, _sprite, imageBank.imageSize , matchQueue);
                        _index++;
                        continue;
                    }
                    //empty init to hide
                    _cardView.Initialize($"Card_{x}_{y}",_gridPos, null,Vector2.zero , matchQueue);
                }
            }

            foreach (var _card in spawnedCards)
            {
                _card.Reveal();
            }
            await Task.Delay(TimeSpan.FromSeconds(firstRevealedTime));
            foreach (var _card in spawnedCards)
            {
                _card.Hide();
            }
        }


        private List<string> MakeDeck(List<string> _input)
        {
            List<string> _all = new();
            _all.AddRange(_input);
            _all.AddRange(_input);
            _all.Shuffle();
            return _all;
        }

      

        public GridSettings.State GetCellState(Vector2Int _gridPos)
        {
            return grid.GetValue(_gridPos.x, _gridPos.y).state;
        }

        public void SetCellState(Vector2Int _gridPos, bool _newState)
        {
            var _gridCell = grid.GetValue(_gridPos.x, _gridPos.y);
            _gridCell.matched = _newState;
        }

        private void OnEnable()
        {
            matchQueue.OnMatch += MatchQueue_OnMatch;
            matchQueue.OnMismatch += MatchQueue_OnMismatch;
            matchQueue.OnQueueCleared += MatchQueue_OnQueueCleared;
        }

        private void MatchQueue_OnQueueCleared()
        {
            Debug.Log("Match Queue Cleared");
        }

        private void MatchQueue_OnMismatch(CardView _card1, CardView _card2)
        {
            Debug.Log($"Mismatch: {_card1.cardID} vs {_card2.cardID}");
        }

        private void MatchQueue_OnMatch(CardView _card1, CardView _card2)
        {
            Debug.Log($"Match: {_card1.cardID} vs {_card2.cardID}");

            SetCellState(_card1.GetGridPosition(), true);
            SetCellState( _card2.GetGridPosition(), true);

            StartCoroutine(WaitForAnim(_card1));
            StartCoroutine(WaitForAnim(_card2));

            matchesFound++;

            IEnumerator WaitForAnim(CardView _card)
            {
                while (_card.IsAnimDone == false)
                {
                    yield return null;
                }
                _card.SetHidden();
                // AddressableManager.Instance.ReleaseInstance(cardPrefab, _card.gameObject);
            }
        }

        private void OnDisable()
        {
            matchQueue.OnMatch -= MatchQueue_OnMatch;
            matchQueue.OnMismatch -= MatchQueue_OnMismatch;
            matchQueue.OnQueueCleared -= MatchQueue_OnQueueCleared;
            AddressableManager.Instance.ReleaseAll();
        }

        private void OnApplicationPause(bool pause)
        {
            if (pause)
            {
                SaveCurrentGrid();
            }
        }

        private void OnApplicationQuit()
        {
            SaveCurrentGrid();   
        }

        private void SaveCurrentGrid()
        {
            var _fromGrid = grid.ToList();
            GameDataManager.Instance.SaveGridData(gridSettings.gridName, imageBank.BankName, _fromGrid);
        }
    }
}