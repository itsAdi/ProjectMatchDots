using System;
using System.Collections.Generic;
using UnityEngine;

namespace KemothStudios.Utility
{
    [Serializable]
    public sealed class SerializableDictionary<Key, Value> : ISerializationCallbackReceiver
    {
        [SerializeField] private KeyValuePair<Key, Value>[] _values;

        private HashSet<Key> _keys;

        private Dictionary<Key, Value> _dictionary;

        public Value this[Key key]
        {
            get
            {
                if (_keys.Contains(key))
                    return _dictionary[key];
                throw new KeyNotFoundException("SerializableDictionary does not contains key, use TryGetValue if unsure about keys");
            }
        }

        public bool TryGetValue(Key key, out Value value)
        {
            if(_keys.Contains(key))
            {
                value = _dictionary[key];
                return true;
            }
            value = default;
            return false;
        }

        public void OnAfterDeserialize()
        {
            _dictionary = new Dictionary<Key, Value>();
            _keys = new HashSet<Key>();
            foreach(KeyValuePair<Key, Value> pair in _values)
            {
                if (_keys.Add(pair.Key))
                    _dictionary.Add(pair.Key, pair.Value);
                else Debug.Log("<color=red>SerializableDictionary cannot have two elements with same, you just added a duplicate key, this element will not be part of the collection</color>");
            }
        }

        public void OnBeforeSerialize()
        {
            
        }

        [Serializable]
        private class KeyValuePair<k, v>
        {
            [SerializeField] private k _key;
            [SerializeField] private v _value;

            public k Key => _key;
            public v Value => _value;
        }
    }
}