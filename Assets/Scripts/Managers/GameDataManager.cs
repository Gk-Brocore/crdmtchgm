using Game.Data;
using Game.Grid;
using Game.Utilities;
using Game.Variables;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Game.Managers
{
    public class GameDataManager : MonoBehaviour
    {

        [SerializeField]
        private GameData gameData;
        [SerializeField]
        private IntVar score;

        private GameSaveData saveData;


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
            Load();
        }




        public void Save()
        {

            string _jsonData = JsonUtility.ToJson(saveData);

            string _path = Path.Combine(Application.persistentDataPath, "gamesave.json");
            File.WriteAllText(_path, _jsonData);
        
            Debug.Log($"Game saved to {_path}");


        }

        public bool Load()
        {
            var _path = Path.Combine(Application.persistentDataPath, "gamesave.json");
            if (File.Exists(_path))
            {
                string _jsonData = File.ReadAllText(_path);
                saveData = JsonUtility.FromJson<GameSaveData>(_jsonData);
                return true;
            }
            else
            {
                saveData = new GameSaveData();
                Save();
            }
            return false;
        }

        public List<GridCellData> GetGridData(string gridName, string bankName)
        {

            GameLevelData _saveData = saveData.Get(gridName, bankName);
            score.SetValue(_saveData.score);
            return _saveData?.cells;
        }

        public bool HasSave(string gridName, string bankName)
        {
            return saveData.Contains(gridName, bankName);
        }

        public void SaveGridData(string gridName, string bankName, List<GridCellData> fromGrid)
        {
            saveData.Add(gridName, bankName, fromGrid);
            saveData.AddScore(gridName, bankName, score.value);
            Save();
        }

        public void SetLevelCompleted(string gridName, string bankName, bool completed)
        {
            saveData.SetCompleted(gridName, bankName, completed);
            Save();
        }

        public bool IsLevelCompleted(string gridName, string bankName)
        {
            var _levelData = saveData.Get(gridName, bankName);
            if (_levelData != null)
            {
                return _levelData.completed;
            }
            return false;
        }
    }
}

namespace Game.Data
{
    [Serializable]
    public class GameSaveData
    {
        public SerializableDictionary<string, GameLevelData> levels;
        public GameSaveData()
        {
            levels = new SerializableDictionary<string, GameLevelData>();
            levels.New();
        }

        public void AddScore(string _gridId, string _imagesId, int _score)
        {
            var _id = $"{_gridId}_{_imagesId}";
            if (levels.Contains(_id))
            {
                levels.Get(_id).score = _score;
            }
            else
            {
                GameLevelData _newLevel = new GameLevelData
                {
                    gridId = _gridId,
                    imagesId = _imagesId,
                    score = _score,
                    cells = new List<GridCellData>()
                };
                levels.Add(_id, _newLevel);
            }
        }

        public void Add(string _gridId, string _imagesId, List<GridCellData> _cells)
        {

            var _id = $"{_gridId}_{_imagesId}";

            if(levels.Contains(_id))
            {
                levels.Get(_id).cells = _cells;
            }
            else
            {
                GameLevelData _newLevel = new GameLevelData
                {
                    gridId = _gridId,
                    imagesId = _imagesId,
                    cells = _cells
                };
                levels.Add(_id, _newLevel);
            }
        }

        public void SetCompleted(string _gridId, string _imgId, bool _completed)
        {
            var _id = $"{_gridId}_{_imgId}";
            if (levels.Contains(_id))
            {
                levels.Get(_id).completed = _completed;
            }
        }

        public GameLevelData Get(string _gridId, string _imgId)
        {
            var _id = $"{_gridId}_{_imgId}";
            return levels.Get(_id);
        }

        public bool Contains(string _gridId, string _imgId)
        {
            var _id = $"{_gridId}_{_imgId}";
            return levels.Contains(_id);
        }




    }

    [Serializable]
    public class GameLevelData
    {
        public string gridId;
        public string imagesId;
        public List<GridCellData> cells;


        public int score;
        public int combo;

        public bool completed;

        public int GetMatchedCount()
        {
            int _count = 0;
            foreach (var cell in cells)
            {
                if (cell.matched)
                    _count++;
            }
            return _count;
        }

        // Add other game-related data as needed
    }
}