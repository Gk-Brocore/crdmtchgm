using Game.Addressable;
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

    }
}