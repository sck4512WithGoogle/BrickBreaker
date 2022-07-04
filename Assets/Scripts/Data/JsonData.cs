using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlayData
{
    public CommonBlockData[] commonBlockData;
    public IceBlockData[] iceBlockData;
    public int ballCount;
    public Vector3 totalBallPos;
    public int round;
    public int score;
}

[Serializable]
public class CommonBlockData
{
    public CommonBlockData(Vector3 _Position, int _PosY, int _Count, int _DamageCount)
    {
        position = _Position;
        posY = _PosY;
        leftCount = _Count;
        damageCount = _DamageCount;
    }
    public Vector3 position;
    public int posY;
    public int leftCount;
    public int damageCount;
}


[Serializable]
public class IceBlockData
{
    public IceBlockData(Vector3 _Position, int _PosY)
    {
        position = _Position;
        posY = _PosY;
    }
    public Vector3 position;
    public int posY;
}



[Serializable]
public class DictionaryData<TKey, TValue> : ISerializationCallbackReceiver
{
    public List<TKey> keys;
    public List<TValue> values;


    private Dictionary<TKey, TValue> myDictionary;
    public Dictionary<TKey, TValue> MyDictionary => myDictionary;


    public DictionaryData(Dictionary<TKey, TValue> _DictionaryData)
    {
        myDictionary = _DictionaryData;
    }


    public void OnAfterDeserialize()
    {
        myDictionary = new Dictionary<TKey, TValue>();
        for (int i = 0; i < keys.Count; i++)
        {
            myDictionary.Add(keys[i], values[i]);
        }
    }

    public void OnBeforeSerialize()
    {
        keys = new List<TKey>(myDictionary.Keys);
        values = new List<TValue>(myDictionary.Values);
    }
}