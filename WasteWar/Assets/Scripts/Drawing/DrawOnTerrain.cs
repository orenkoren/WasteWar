
using Constants;
using System;
using System.Collections.Generic;
using UnityEngine;

public class DrawOnTerrain : MonoBehaviour
{
    [SerializeField]
    private GameObject structurePrefab1;
    [SerializeField]
    private GameObject structurePrefab2;

    public GameObject TemplateStructure { get; set; }
    public Vector3 TemplateStructureSize { get; private set; }
    private readonly List<GameObject> Structures = new List<GameObject>();
    private bool IsAStructureSelected = false;

    private void Start()
    {
        GameEvents.DrawStateChangedListeners += DestroyPreviousAndPrepareNew;
    }

    private void DestroyPreviousAndPrepareNew(object sender, DrawData data)
    {
        print("hpo");
        Destroy(TemplateStructure);
        TemplateStructure = Instantiate(data.StructurePrefab,
                            ObjectSnapper.SnapToGridCell(data.StructureLocation, GridConstants.Instance.FloatCellSize()),
                            Quaternion.Euler(GameConstants.Instance.DEFAULT_OBJECT_ROTATION));
        IsAStructureSelected = true;
        TemplateStructureSize = TemplateStructure.GetComponent<Renderer>().bounds.size;
    }

    public void DrawStructure(Vector3 snapLoc)
    {
        TemplateStructure.GetComponent<MeshRenderer>().material.SetColor("_Color", Color.white);
        Structures.Add(Instantiate(TemplateStructure, ObjectSnapper.SnapToGridCell(
            snapLoc,
            GridConstants.Instance.FloatCellSize(), TemplateStructureSize),
            Quaternion.Euler(GameConstants.Instance.DEFAULT_OBJECT_ROTATION)
            ));
    }
    public bool DrawTemplateStructure(Vector3 structLoc)
    {
        foreach (var key in GameKeys.Instance.StructureKeybinds)
        {
            if (Input.GetKey(key))
            {
                switch (key)
                {
                    //case KeyCode.B:
                    //    destroyPreviousAndPrepareNew(structurePrefab1);
                    //    break;
                    //case KeyCode.C:
                    //    destroyPreviousAndPrepareNew(structurePrefab2);
                    //    break;
                    case KeyCode.Escape:
                        Destroy(TemplateStructure);
                        IsAStructureSelected = false;
                        break;
                }
            }
        }
        return IsAStructureSelected;

        void DestroyPreviousAndPrepareNew(object sender, GameObject structure)
        {
            
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

    public void DestroyTemplateStructure()
    {
        Destroy(TemplateStructure);
        IsAStructureSelected = false;
    }

    private void DoSomething(object sender, int something)
    {
       
    }
}
