using UnityEngine;


//need an explanation as to how this works
public class GridConstants : MonoBehaviour
{
    public enum CellSize
    {
        QUARTER,
        THIRD,
        HALF,
        ONE,
        TWO,
        FOUR
    }

    public CellSize CELL_SIZE;

    public float FloatCellSize()
    {
        switch (CELL_SIZE)
        {
            case CellSize.QUARTER:
                return 0.25f;
            case CellSize.THIRD:
                return 0.33f;
            case CellSize.HALF:
                return 0.5f;
            case CellSize.ONE:
                return 1f;
            case CellSize.TWO:
                return 2f;
            case CellSize.FOUR:
                return 4f;
        }
        return 1f;
    }


    public static GridConstants Instance { get; private set; }

    //????????
    private void Awake()
    {
        if (Instance == null)
        {
            //what's this here?
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        else
        {
            Destroy(gameObject);
        }
    }
}
