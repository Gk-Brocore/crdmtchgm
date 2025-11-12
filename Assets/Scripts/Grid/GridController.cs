using Game.Addressable;
using Game.Cards;
using Game.Images;
using Game.Managers;
using System;
using System.Collections;
using System.Collections.Generic;
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
        public MemoryMatchManager matchQueue;

        [Header("UI Elements")]
        [SerializeField]
        private GridLayout gridLayout;
        [SerializeField]
        private AssetReference cardPrefab; 


        private GridContainer<GridCellData> grid;
        private List<GameObject> spawnedCards = new List<GameObject>();

        private RectTransform rectTransform;

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

            
            InitilizeGrid();

            SetupUILayout();

            SpawnUICards();


            if (gridSettings.showDebug)
                grid.inGameDebug = true;
        }

        private void InitilizeGrid()
        {
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

        private async void SpawnUICards()
        {

            foreach (var _card in spawnedCards)
            {
                if (_card != null)
                    Destroy(_card);
            }
            spawnedCards.Clear();

            int _total = gridSettings.Width * gridSettings.Height;

            for (int y = 0; y < gridSettings.Height; y++)
            {
                for (int x = 0; x < gridSettings.Width; x++)
                {
                    GameObject _card = await AddressableManager.Instance.InstantiateAsync(cardPrefab, Vector3.zero, Quaternion.identity, gridLayout.transform);

                    var _cardView = _card.GetComponent<CardView>();
                    _cardView.Initialize($"Card_{x}_{y}", null, matchQueue);

                    _card.name = $"Card_{x}_{y}";
                    var _img = _card.GetComponent<Image>();

                    if (_img != null)
                    {
                        _img.color = (grid.GetValue(x, y).state == GridSettings.State.Active)
                        ? Color.green
                        : Color.white;
                     }


                    spawnedCards.Add(_card);

                    var _button = _card.GetComponent<Button>();
                    if (_button != null)
                    {
                        int cx = x, cy = y;
                        _button.onClick.AddListener(() => OnCardClicked(cx, cy, _card));
                    }
                }
            }
        }

        private void OnCardClicked(int _x, int _y, GameObject _card)
        {
            var _current = grid.GetValue(_x, _y);
            var _next = (_current.state == GridSettings.State.Static)
                ? GridSettings.State.Active
                : GridSettings.State.Static;

            var _cellData = grid.GetValue(_x, _y);
            _cellData.state = _next;

            var _img = _card.GetComponent<Image>();
            if (_img != null)
            {
                _img.color = (_next == GridSettings.State.Active)
                    ? Color.green
                    : Color.white;
            }

        }

        public GridSettings.State GetCellState(Vector2Int _gridPos)
        {
            return grid.GetValue(_gridPos.x, _gridPos.y).state;
        }

        public void SetCellState(Vector2Int _gridPos, GridSettings.State _newState)
        {
            var _gridCell = grid.GetValue(_gridPos.x, _gridPos.y);
            _gridCell.state = _newState;
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
        }

        private void OnDisable()
        {
            matchQueue.OnMatch -= MatchQueue_OnMatch;
            matchQueue.OnMismatch -= MatchQueue_OnMismatch;
            matchQueue.OnQueueCleared -= MatchQueue_OnQueueCleared;
            AddressableManager.Instance.ReleaseAll();
        }
    }
}