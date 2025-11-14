using Game.Grid;
using Game.Images;
using Game.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Data
{
    [CreateAssetMenu(fileName = "GameData", menuName = "Game/Game Data")]
    public class GameData : ScriptableObject
    {

        [SerializeField]
        private SerializableDictionary<string,GridSettings> grids;

        [SerializeField]
        private SerializableDictionary<string, ImageBank> imageBanks;

        public SerializableDictionary<string, GridSettings> Grids { get => grids; set => grids = value; }
        public SerializableDictionary<string, ImageBank> ImageBanks { get => imageBanks; set => imageBanks = value; }
    }
}