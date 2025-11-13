using Game.Addressable;
using Game.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace Game.Images
{
    [CreateAssetMenu(fileName = "ImageBank", menuName = "Game/ImageBank")]
    public class ImageBank : ScriptableObject
    {
        [Serializable]
        public class Entry
        {
            public string id;
            public AssetReference spriteRef;
        }

        public string BankName;
        public Vector2 imageSize;

        public List<Entry> entries = new List<Entry>();

        public AssetReference GetSpriteReferenceById(string _id)
        {
            foreach (var entry in entries)
            {
                if (entry.id == _id)
                {
                    return entry.spriteRef;
                }
            }
            return null;
        }

        public async Task<Sprite> GetSprite(string _id)
        {
            var _assetRef = GetSpriteReferenceById(_id);
            return await AddressableManager.Instance.LoadImageAsync<Sprite>(_assetRef);
        }

        [ContextMenu("Get Names")]
        public void GetNames()
        {
            foreach (var _entry in entries)
            {
                var _name = _entry.spriteRef.SubObjectName;
                if (!string.IsNullOrEmpty(_name))
                {
                    _entry.id = _name;
                }
            }
        }


        public List<string> GetShuffled(int _count)
        {
            var _output = new List<string>();
            foreach (var _entry in entries)
            {
                _output.Add(_entry.id);
            }

            _output.Shuffle();
            
            if (_count > _output.Count)
            {
                Debug.Log("Invalid Count");
                return _output;
            }

            var _toRemove = _output.Count - _count;
           

            for (int _index = _toRemove - 1; _index >= 0; _index--)
            {
                _output.RemoveAt(_index);
            }


            return _output;
        }

    }
}