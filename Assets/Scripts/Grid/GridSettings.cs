using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Grid
{
    [CreateAssetMenu(fileName = "GridSettings", menuName = "Grid/Grid Settings")]
    public class GridSettings : ScriptableObject
    {
        [Header("Grid Settings")]
        [SerializeField]
        private int width = 4;
        [SerializeField]
        private int height = 4;
        [SerializeField]
        private Vector2 cellSize = new Vector2(1f, 1.2f);
        [SerializeField]    
        private Vector2 cellSpacing = new Vector2(0.1f, 0.1f);
        [SerializeField]
        private RectOffset padding;
        [SerializeField]
        private Vector2 origin = Vector2.zero;

        [Header("Editor Display")]
        public bool showDebug = true;
        public bool showEditorPreview = true;

        [Header("Cell States")]
        [Tooltip("Mark cells as active (playable) or static (non-playable)")]
        public List<CellState> cells = new List<CellState>();

        public int Width { get => width; set => width = value; }
        public int Height { get => height; set => height = value; }
        public Vector2 CellSize { get => cellSize; set => cellSize = value; }
        public Vector2 CellSpacing { get => cellSpacing; set => cellSpacing = value; }

        public RectOffset Padding { get => padding; set => padding = value; }
        public Vector2 Origin { get => origin; set => origin = value; }

        public void Initialize()
        {
            cells.Clear();
            for (int x = 0; x < Width; x++)
            {
                for (int y = 0; y < Height; y++)
                {
                    cells.Add(new CellState
                    {
                        position = new Vector2Int(x, y),
                        state = State.Active
                    });
                }
            }
        }

        public State GetCellState(Vector2Int _pos)
        {
            var cell = cells.Find(c => c.position == _pos);
            return cell != null ? cell.state : State.Static;
        }

        public void SetCellState(Vector2Int _pos, State _newState)
        {
            var cell = cells.Find(c => c.position == _pos);
            if (cell != null)
                cell.state = _newState;
        }

        public enum State { Active, Static }

        [System.Serializable]
        public class CellState
        {
            public Vector2Int position;
            public State state;
        }
    }


}