﻿using UnityEngine;
public class GridConstants : MonoBehaviour
{
    public CellSize CELL_SIZE;
    [HideInInspector]
    public int CELL_COUNT;

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
    {///qegfmqekgjlqgje
        if (Instance == null)
        {
            Instance = this;
         CELL_COUNT = (int)RuntimeGameObjRefs.Instance.TERRAIN.terrainData.size.x * (int)RuntimeGameObjRefs.Instance.TERRAIN.terrainData.size.z;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    private void Start()
    {
    }
}