using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Linq;

//Based off of https://github.com/upscalebaby/generic-serializable-dictionary
namespace PhantasmicGames.Common
{
    [Serializable]
    public class SerializableDictionary<TKey, TValue> : IDictionary<TKey, TValue>, ISerializationCallbackReceiver
    {
        // Internal
        [SerializeField] List<KeyValuePair> m_KeyValuePairs = new List<KeyValuePair>();
        [SerializeField] Dictionary<TKey, int> m_IndexByKey = new Dictionary<TKey, int>();
        [SerializeField, HideInInspector] Dictionary<TKey, TValue> m_Dictionary = new Dictionary<TKey, TValue>();

#pragma warning disable 0414
        [SerializeField, HideInInspector]
        private bool m_KeyCollision;
#pragma warning restore 0414

        // Serializable KeyValuePair struct
        [Serializable]
        struct KeyValuePair
        {
            public TKey Key;
            public TValue Value;

            public KeyValuePair(TKey Key, TValue Value)
            {
                this.Key = Key;
                this.Value = Value;
            }
        }

        // Since lists can be serialized natively by unity no custom implementation is needed
        public void OnBeforeSerialize() { }

        // Fill dictionary with list pairs and flag key-collisions.
        public void OnAfterDeserialize()
        {
            m_Dictionary.Clear();
            m_IndexByKey.Clear();
            m_KeyCollision = false;

            for (int i = 0; i < m_KeyValuePairs.Count; i++)
            {
                var key = m_KeyValuePairs[i].Key;
                if (key != null && !ContainsKey(key))
                {
                    m_Dictionary.Add(key, m_KeyValuePairs[i].Value);
                    m_IndexByKey.Add(key, i);
                }
                else
                    m_KeyCollision = true;
            }
        }

        // IDictionary
        public TValue this[TKey key]
        {
            get => m_Dictionary[key];
            set
            {
                m_Dictionary[key] = value;

                if (m_IndexByKey.ContainsKey(key))
                {
                    var index = m_IndexByKey[key];
                    m_KeyValuePairs[index] = new KeyValuePair(key, value);
                }
                else
                {
                    m_KeyValuePairs.Add(new KeyValuePair(key, value));
                    m_IndexByKey.Add(key, m_KeyValuePairs.Count - 1);
                }
            }
        }

        public ICollection<TKey> Keys => m_Dictionary.Keys;
        public ICollection<TValue> Values => m_Dictionary.Values;

        public void Add(TKey key, TValue value)
        {
            m_Dictionary.Add(key, value);
            m_KeyValuePairs.Add(new KeyValuePair(key, value));
            m_IndexByKey.Add(key, m_KeyValuePairs.Count - 1);
        }

        public bool ContainsKey(TKey key) => m_Dictionary.ContainsKey(key);

        public bool ContainsValue(TValue value) => m_Dictionary.ContainsValue(value);

        public bool Remove(TKey key)
        {
            if (m_Dictionary.Remove(key))
            {
                var index = m_IndexByKey[key];
                m_KeyValuePairs.RemoveAt(index);
                UpdateIndexes(index);
                m_IndexByKey.Remove(key);
                return true;
            }
            else
            {
                return false;
            }
        }

        void UpdateIndexes(int removedIndex)
        {
            for (int i = removedIndex; i < m_KeyValuePairs.Count; i++)
            {
                var key = m_KeyValuePairs[i].Key;
                m_IndexByKey[key]--;
            }
        }

        public int RemoveAll(Predicate<TKey> match)  //Could be optimized like List<T> to avoid using Linq
        {
            if (match == null)
                throw new ArgumentNullException("match");

            int result = 0;
            foreach (var key in Keys.ToArray().Where(key => match(key)))
            {
                Remove(key);
                result++;
            }

            return result;
        }

        public bool TryGetValue(TKey key, out TValue value) => m_Dictionary.TryGetValue(key, out value);

        // ICollection
        public int Count => m_Dictionary.Count;
        public bool IsReadOnly { get; set; }

        public void Add(KeyValuePair<TKey, TValue> pair)
        {
            Add(pair.Key, pair.Value);
        }

        public void Clear()
        {
            m_Dictionary.Clear();
            m_KeyValuePairs.Clear();
            m_IndexByKey.Clear();
        }

        public bool Contains(KeyValuePair<TKey, TValue> pair)
        {
            if (m_Dictionary.TryGetValue(pair.Key, out TValue value))
                return EqualityComparer<TValue>.Default.Equals(value, pair.Value);
            else
                return false;
        }

        public void CopyTo(KeyValuePair<TKey, TValue>[] array, int arrayIndex)
        {
            if (array == null)
                throw new ArgumentException("The array cannot be null.");
            if (arrayIndex < 0)
                throw new ArgumentOutOfRangeException("The starting array index cannot be negative.");
            if (array.Length - arrayIndex < m_Dictionary.Count)
                throw new ArgumentException("The destination array has fewer elements than the collection.");

            foreach (var pair in m_Dictionary)
            {
                array[arrayIndex] = pair;
                arrayIndex++;
            }
        }

        public bool Remove(KeyValuePair<TKey, TValue> pair)
        {
            if (m_Dictionary.TryGetValue(pair.Key, out TValue value))
            {
                bool valueMatch = EqualityComparer<TValue>.Default.Equals(value, pair.Value);
                if (valueMatch)
                    return Remove(pair.Key);
            }
            return false;
        }

        // IEnumerable
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator() => m_Dictionary.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => m_Dictionary.GetEnumerator();
    }
}