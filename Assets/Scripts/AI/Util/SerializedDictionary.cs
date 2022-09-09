using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Nixtor.AI
{
    [Serializable]
    public class SerializedDictionary<TKey, TValue> : Dictionary<TKey, TValue>, ISerializationCallbackReceiver
    {
        [SerializeField] private List<TKey> keys = new List<TKey>();
        [SerializeField] private List<TValue> values = new List<TValue>();

        //Save to list
        public void OnBeforeSerialize()
        {
            keys.Clear();
            values.Clear();

            foreach(KeyValuePair<TKey, TValue> pair in this)
            {
                keys.Add(pair.Key);
                values.Add(pair.Value);
            }
        }

        //Load to dictionary
        public void OnAfterDeserialize()
        {
            Clear();

            if (keys.Count != values.Count)
                throw new System.Exception($"There are {keys.Count} and {values.Count}. Make sure both keys and values used are serializables.");

            for (int i = 0; i < keys.Count; i++)
            {
                Add(keys[i], values[i]);
            }
        }

    }
}