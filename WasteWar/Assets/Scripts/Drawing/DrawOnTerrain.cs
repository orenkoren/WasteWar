
using Constants;
using System;
using System.Collections.Generic;
using UnityEngine;

public class DrawOnTerrain : MonoBehaviour
{
    public GameObject TemplateStructure { get; set; }
    public Vector3 TemplateStructureSize { get; private set; }
    public bool IsAStructureToBuildSelected { get; private set; } = false;

    private readonly List<GameObject> Structures = new List<GameObject>();
    private bool isCellOccupied;

    public bool IsCellOccupied
    {
        get { return isCellOccupied; }
        set { isCellOccupied = value; }
    } 

    void Start()
    {
        GameEvents.TemplateSelectedListeners += DestroyPreviousAndPrepareNewTemplate;
        GameEvents.StructurePlacedListeners += DrawStructure;
    }
    void Update()
    {
        
    }

    private void DestroyPreviousAndPrepareNewTemplate(object sender, TemplateData data)
    {
        Destroy(TemplateStructure);
        if (data.IsSelected)
        {
            TemplateStructure = Instantiate(data.TemplateStructure,
                                ObjectSnapper.SnapToGridCell(data.mousePos, GridConstants.Instance.FloatCellSize()),
                                Quaternion.Euler(GameConstants.Instance.DEFAULT_OBJECT_ROTATION));
            IsAStructureToBuildSelected = true;
            TemplateStructureSize = TemplateStructure.GetComponent<Renderer>().bounds.size;
        }
        else
            IsAStructureToBuildSelected = false;
    }

    public void DrawTemplateStructure(Vector3 loc)
    {
            if (!isCellOccupied)
                SetTemplateStructureColor(Color.green);
            else
                SetTemplateStructureColor(Color.red);
        SetTemplateStructurePos(loc);
    }

    public void DrawStructure(object sender, Vector3 mousePos)
    {
        if (IsAStructureToBuildSelected == true && !IsCellOccupied)
        { 
            TemplateStructure.GetComponent<MeshRenderer>().material.SetColor("_Color", Color.white);
            Structures.Add(Instantiate(TemplateStructure, ObjectSnapper.SnapToGridCell(
                mousePos,
                GridConstants.Instance.FloatCellSize(), TemplateStructureSize),
                Quaternion.Euler(GameConstants.Instance.DEFAULT_OBJECT_ROTATION)
                ));
            Destroy(TemplateStructure);
            IsAStructureToBuildSelected = false;
        }
    }
    public void SetTemplateStructureColor(Color color)
    {
        TemplateStructure.GetComponent<MeshRenderer>().material.SetColor("_Color", color);
    }

    public void SetTemplateStructurePos(Vector3 pos)
    {
        TemplateStructure.transform.position = ObjectSnapper.SnapToGridCell(pos, GridConstants.Instance.FloatCellSize(), TemplateStructureSize);
    }
}
