using UnityEngine;
namespace Constants
{
    public class GridConstants : MonoBehaviour
    {
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

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                //what's this here?
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}