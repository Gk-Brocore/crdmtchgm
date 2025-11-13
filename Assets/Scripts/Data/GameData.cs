using Game.Grid;
using Game.Images;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Data
{
    [CreateAssetMenu(fileName = "GameData", menuName = "Game/Game Data")]
    public class GameData : ScriptableObject
    {

        [SerializeField]
        private List<GridSettings> Grids;

        [SerializeField]
        private List<ImageBank> ImageBanks;

       
    }
}