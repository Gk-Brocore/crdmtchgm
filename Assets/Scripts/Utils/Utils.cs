using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace Game.Utilities
{
    public static class Utils 
    {
        private static System.Random rng = new System.Random();
     
        public static void Shuffle<T>(this List<T> _list)
        {
            int _next = _list.Count;
            while (_next > 1)
            {
                _next--;
                int _index = rng.Next(_next);
                T _temp = _list[_index];
                _list[_index] = _list[_next];
                _list[_next] = _temp;
            }
        }

        public static List<T> GetShuffled<T>(this List<T> _list)
        {
            var _outList = new List<T>();
            _outList.AddRange(_list);
            _outList.Shuffle();
            return _outList;
        }
    }
}