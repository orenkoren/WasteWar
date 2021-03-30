public abstract class Structure
{
    protected int Health { get; set; }
    protected int Armor { get; set; }
}

public class Building : Structure
{
    public int PollutionIndex { get; set; }

    public Building()
    {
        Health = 100;
        Armor = 0;
        PollutionIndex = 1;
    }
}

public class Turret : Structure
{
    public int PollutionIndex { get; set; }

    public Turret()
    {
        Health = 200;
        Armor = 0;
        PollutionIndex = 1;
    }
}

public class Wall : Structure
{
    public Wall()
    {
        Health = 100;
        Armor = 5;
    }
}
