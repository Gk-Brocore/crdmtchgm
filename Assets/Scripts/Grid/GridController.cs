using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Grid
{
    public class GridController : MonoBehaviour
    {
        [Header("Grid Setup")]
        [SerializeField]
        private GridSettings gridSettings;

        [Header("UI Elements")]
        [SerializeField]
        private GridLayout gridLayout;
        [SerializeField]
        private GameObject cardPrefab; 


        private GridContainer<GridSettings.State> grid;
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
            if (gridSettings == null)
            {
                Debug.LogError("No grid settings assigned!", this);
                return;
            }

            if (cardPrefab == null)
            {
                Debug.LogError("No Card Prefab assigned!", this);
                return;
            }

            grid = new GridContainer<GridSettings.State>(
                gridSettings.Width,
                gridSettings.Height,
                gridSettings.CellSize,
                gridSettings.CellSpacing,
                gridSettings.Origin,
                () => GridSettings.State.Static
            );

            foreach (var _cell in gridSettings.cells)
            {
                grid.SetValue(_cell.position.x, _cell.position.y, _cell.state);
            }

            SetupUILayout();

            SpawnUICards();

            if (gridSettings.showDebug)
                grid.inGameDebug = true;
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

        private void SpawnUICards()
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
                    GameObject _card = Instantiate(cardPrefab, gridLayout.transform);
                    _card.name = $"Card_{x}_{y}";
                    var _img = _card.GetComponent<Image>();

                    if (_img != null)
                    {
                        _img.color = (grid.GetValue(x, y) == GridSettings.State.Active)
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
            var _next = (_current == GridSettings.State.Static)
                ? GridSettings.State.Active
                : GridSettings.State.Static;

            grid.SetValue(_x, _y, _next);

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
            return grid.GetValue(_gridPos.x, _gridPos.y);
        }

        public void SetCellState(Vector2Int _gridPos, GridSettings.State _newState)
        {
            grid.SetValue(_gridPos.x, _gridPos.y, _newState);
        }
    }
}