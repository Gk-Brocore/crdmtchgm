using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Grid
{
    [System.Serializable]
    public class GridCellData
    {
        public string id; 
        public GridSettings.State state;
        public bool revealed;
        public bool matched;

        public GridCellData() { id = ""; state = GridSettings.State.Hidden; revealed = false; matched = false; }
        public GridCellData(string id, GridSettings.State state)
        {
            this.id = id;
            this.state = state;
            this.revealed = false;
            this.matched = false;
        }

        public override string ToString()
        {
            return $"id:{id} state:{state} revealed:{revealed} matched:{matched}";
        }
    }
}