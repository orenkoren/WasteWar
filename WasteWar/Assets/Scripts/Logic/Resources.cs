using System;
using Random = UnityEngine.Random;

public class Resource
{
    public int Count { get; set; }
}

public class Coal : Resource
{
    public Coal()
    {
        Count = Random.Range(1000, 10000);
    }
}