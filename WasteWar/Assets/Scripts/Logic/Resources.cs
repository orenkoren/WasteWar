using System;

public class Resource
{
    protected int Count { get; set; }
}

public class Coal : Resource
{
    public Coal()
    {
        var rand = new Random();
        Count = rand.Next(1000, 10000);
    }

    public void DecrementCount()
    {
        Count--;
    }
}