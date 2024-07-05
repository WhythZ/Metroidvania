using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class SerializableDictionary<Tkey, TValue> : Dictionary<Tkey, TValue>, ISerializationCallbackReceiver
//用于处理物品栏的字典问题
{
    [SerializeField] List<Tkey> keys = new List<Tkey>();
    [SerializeField] List<TValue> values = new List<TValue>();


    public void OnBeforeSerialize()
    {
        //清除原来的数据，因为物品栏字典中的映射可能会发生变化
        keys.Clear();
        values.Clear();

        //写入新的物品栏数据
        foreach (KeyValuePair<Tkey, TValue> _pair in this)
        {
            keys.Add(_pair.Key);
            values.Add(_pair.Value);
        }
    }
    public void OnAfterDeserialize()
    {
        this.Clear();

        if(keys.Count != values.Count)
        {
            //Debug.Log("Keys Count Not Equals to Values Count");
        }

        for (int i = 0; i < keys.Count; i++)
        {
            this.Add(keys[i], values[i]);
        }
    }
}
