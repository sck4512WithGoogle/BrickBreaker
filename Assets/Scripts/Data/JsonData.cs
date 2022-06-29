using System;
using UnityEngine;

[Serializable]
public class PlayData
{
    public CommonBlockData[] commonBlockData;
    public IceBlockData[] iceBlockData;
    public int ballCount;
    public Vector3 totalBallPos;
    public int round;
}

[Serializable]
public class CommonBlockData
{
    public CommonBlockData(Vector3 _Position, int _PosY, int _Count, bool _HasTakeDamaged)
    {
        position = _Position;
        posY = _PosY;
        leftCount = _Count;
        hasTakeDamaged = _HasTakeDamaged;
    }
    public Vector3 position;
    public int posY;
    public int leftCount;
    public bool hasTakeDamaged;
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

