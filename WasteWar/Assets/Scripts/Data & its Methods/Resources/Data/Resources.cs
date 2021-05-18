using Random = UnityEngine.Random;

public class Resource
{
    public const int RESOURCE_COUNT = 10;
    public int Count { get; set; }
}

public class Coal : Resource
{
    public Coal()
    {
        Count = RESOURCE_COUNT;
    }
}