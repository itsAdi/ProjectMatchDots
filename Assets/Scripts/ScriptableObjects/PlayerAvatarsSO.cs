using System;
using System.Collections.Generic;
using UnityEngine;

namespace KemothStudios
{
    /// <summary>
    /// Collection of avatars that players can use
    /// </summary>
    public class PlayerAvatarsSO : ScriptableObject
    {
        [SerializeField]
        private Sprite[] _avatars;

        public IEnumerable<Sprite> GetAvatars;

        [NonSerialized]
        private int _avatarsCount;

        public int GetAvatarsCount
        {
            get
            {
                if(_avatarsCount == 0) _avatarsCount = _avatars.Length;
                return _avatarsCount;
            }
        }

        public bool TryGetAvatarAtIndex(int index, out Sprite avatar)
        {
            if(_avatars != null && index >= 0)
            {
                int length = GetAvatarsCount;
                if (index != length)
                {
                    avatar = _avatars[index];
                    return true;
                }
            }
            avatar = null;
            return false;
        }
    }
}