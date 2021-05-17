using UnityEngine;

public class BuildingState : MonoBehaviour
{
    public readonly int TOTAL_CAPACITY = 100;
    public const float INITIAL_YIELD_SPEED = 1.1f;

    public UnityEngine.UI.Text CubeTextComponent;
    public bool IsGenerator { get; private set; } = true;
    public int PollutionIndex { get; private set; } = 1;
    public int Health { get; private set; } = 100;
    public int Armor { get; private set; } = 1;
    public int Level { get; private set; } = 1;
    public int _storage = 0;
    public float _yieldFrequency = INITIAL_YIELD_SPEED;
    public Resource AvailableResources { get; set; }
    public int KeyOfNodeBeingUsed { get; set; }

    public float YieldFrequency
    {
        get
        {
            return _yieldFrequency;
        }
        private set
        {
            _yieldFrequency -= _yieldFrequency * (value / 10);
        }
    }
    public int Storage
    {
        get
        {
            return _storage;
        }
        set
        {
            --_storage;
        }
    }
}
