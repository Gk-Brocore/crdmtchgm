using Game.Data;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.Managers
{
    public class GameDataManager : MonoBehaviour
    {

        [SerializeField]
        private GameData gameData;


        public static GameDataManager Instance { get; private set; }
        public GameData GameData { get => gameData; set => gameData = value; }

        private void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(this.gameObject);
            }
            else
            {
                Instance = this;   
            }
        }

        
    }
}