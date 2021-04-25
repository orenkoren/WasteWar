using UnityEngine;

public class BaseData : MonoBehaviour
{
    public UnityEngine.UI.Text CubeTextComponent;
    public bool IsGenerator { get; private set; } = false;
    private int _storage = 0;

    public int Storage
    {
        get
        {
            return _storage;
        }
        set
        {
            ++_storage;
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
