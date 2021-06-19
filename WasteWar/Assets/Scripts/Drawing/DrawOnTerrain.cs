using System.Collections.Generic;
using UnityEngine;

public class DrawOnTerrain : MonoBehaviour
{
    [SerializeField]
    private PrefabPlaceable placeablePrefabs;
    [SerializeField]
    private GameObject camParent;
    [SerializeField]
    private GameObject pipeMethods;

    public GameObject TemplateStructure { get; set; }
    public Vector3 TemplateStructureSize { get; private set; }

    private List<GameObject> structures = new List<GameObject>();
    private int rotationState = 0;

    private void Start()
    {
        GameEvents.MouseMovListeners += DrawTemplateStructureAt;
        GameEvents.BuildingRotationListeners += RotateTemplate;
        GameEvents.TemplateSelectedListeners += DestroyOldAndCreateNewTemplate;
        GameEvents.LeftClickPressedListeners += DrawStructure;
        GameEvents.ContinuousLeftClickPressListeners += DrawStructuresConsecutively;
        GameEvents.RightClickPressedListeners += DeleteStructure;
    }

    private void DrawTemplateStructureAt(object sender,Vector3 loc)
    {
        if (CheckIfLocationIsFree())
            TemplateStructure.GetComponent<PaintModel>().Paint("placement", true);
        else
            TemplateStructure.GetComponent<PaintModel>().Paint("placement", false);
        SetTemplateStructurePos(loc);
    }

    //TODO make placement near edges of the map smoother
    private void DrawStructure(object sender, TemplateData data)
    {
        if (data.TemplateStructure != null && CheckIfLocationIsFree())
        {
            GameObject Structure= PlaceablePrefabInstantiator(placeablePrefabs.GetPlaceablePrefab(TemplateStructure), data.MousePos,TemplateStructure.transform.rotation);
            if (Structure.CompareTag("Building"))
               GameEvents.FireBuildingPlaced(this, Structure);
           
            structures.Add(Structure);
        }
        else if(data.TemplateStructure != null && !CheckIfLocationIsFree())
            StartCoroutine(TemplateStructure.GetComponent<StructureShake>().ShakeTemplateForXSec());
    }

    private void DrawStructuresConsecutively(object sender, TemplateData data)
    {
        if (data.TemplateStructure != null && data.TemplateStructure.tag.Contains("Pipe") && CheckIfLocationIsFree())
        {
            GameObject Structure = PlaceablePrefabInstantiator(placeablePrefabs.GetPlaceablePrefab(TemplateStructure), data.MousePos);
        
            structures.Add(Structure);

            GameEvents.FirePipePlaced(this, Structure);  
        }
    }

    private void DeleteStructure(object sender, RaycastHit data)
    {
        GameEvents.FirePipeDeleted(this, data.collider.gameObject);
        Destroy(data.collider.gameObject);
        structures.Remove(data.collider.gameObject);
    }

    private void DestroyOldAndCreateNewTemplate(object sender, TemplateData data)
    {
        if (data.TemplateStructure != null)
        {
            TemplateInstantiator(data.TemplateStructure, data.MousePos);
        }
        else
            Destroy(TemplateStructure);
    }

    private void TemplateInstantiator(GameObject template, Vector3 mousePos)
    {
        Destroy(TemplateStructure);
        TemplateStructure = Instantiate(template,
                            ObjectSnapper.SnapToGridCell(mousePos),
                            template.transform.rotation);
        TemplateStructure.layer = LayerMasks.Instance.IGNORE_RAYCAST_LAYER;
        TemplateStructureSize = TemplateStructure.GetComponent<BoxCollider>().size;
    }

    private GameObject PlaceablePrefabInstantiator(GameObject template, Vector3 mousePos)
    {

        template.layer = LayerMasks.Instance.ATTACKABLE_LAYER;

        return Instantiate(template,
                           ObjectSnapper.SnapToGridCell(mousePos, template.GetComponent<BoxCollider>().size),
                           template.transform.rotation);
    }

    private GameObject PlaceablePrefabInstantiator(GameObject template, Vector3 mousePos, Quaternion rotation)
    {

        template.layer = LayerMasks.Instance.ATTACKABLE_LAYER;

        return Instantiate(template,
                           ObjectSnapper.SnapToGridCell(mousePos, template.GetComponent<BoxCollider>().size),
                           rotation);
    }

    private void RotateTemplate(object sender, bool isCurvedModeOn)
    {
        if (TemplateStructure.tag.Contains("Pipe"))
        {
            var pos = TemplateStructure.transform.position;
            if (!isCurvedModeOn)
            {
                rotationState = 0;
                if (TemplateStructure.CompareTag("PipeTBTemplate"))
                    TemplateInstantiator(pipeMethods.GetComponent<PipeLogic>().templates.PipeLeftRight, pos);
                else if (TemplateStructure.CompareTag("PipeLRTemplate"))
                    TemplateInstantiator(pipeMethods.GetComponent<PipeLogic>().templates.PipeTopBottom, pos);
            }
            else
            {
                switch (rotationState)
                {
                    case 0:
                        TemplateInstantiator(pipeMethods.GetComponent<PipeLogic>().templates.PipeTopLeft, pos);
                        break;
                    case 1:
                        TemplateInstantiator(pipeMethods.GetComponent<PipeLogic>().templates.PipeTopRight, pos);
                        break;
                    case 2:
                        TemplateInstantiator(pipeMethods.GetComponent<PipeLogic>().templates.PipeBottomRight, pos);
                        break;
                    case 3:
                        TemplateInstantiator(pipeMethods.GetComponent<PipeLogic>().templates.PipeBottomLeft, pos);
                        break;
                }
                rotationState = (rotationState + 1) % 4;         
            }
            GameEvents.FirePipePlaced2(this, TemplateStructure.tag);
        }
        else if (TemplateStructure.tag.Contains("Wall")) {
            TemplateStructure.transform.Rotate(new Vector3(0f, 90f,0f));
        }
    }

    private void SetTemplateStructurePos(Vector3 pos)
    {
        TemplateStructure.transform.position = ObjectSnapper.SnapToGridCell(pos, TemplateStructureSize);
    }

    private bool CheckIfLocationIsFree()
    {
        // logic https://gyazo.com/d7bca8a2098f36b1deb686ddd22978b9
        if (TemplateStructure.tag.Contains("Building")) {

            Vector3 lowerBound = TemplateStructure.GetComponent<Collider>().bounds.min;
            Vector3 upperBound = TemplateStructure.GetComponent<Collider>().bounds.max;
            
            //elevate the Vectors so that the raycast works properly
            lowerBound.y += 1;
            upperBound.y += 1;

            Ray rayOne = new Ray(lowerBound, Vector3.down);
            Ray rayTwo = new Ray(upperBound, Vector3.down);

            return Physics.Raycast(rayOne, CameraConstants.Instance.RAYCAST_DISTANCE, LayerMasks.Instance.RESOURCE) &&
                   Physics.Raycast(rayTwo, CameraConstants.Instance.RAYCAST_DISTANCE, LayerMasks.Instance.RESOURCE) &&
                   !Physics.CheckBox(TemplateStructure.GetComponent<Collider>().bounds.center, TemplateStructure.GetComponent<Collider>().bounds.extents,
                   TemplateStructure.transform.rotation, LayerMasks.Instance.ATTACKABLE)
                   ;
        }
        else
            return !Physics.CheckBox(TemplateStructure.GetComponent<Collider>().bounds.center, TemplateStructure.GetComponent<Collider>().bounds.extents,
                TemplateStructure.transform.rotation, LayerMasks.Instance.ATTACKABLE);
    }

    private void OnDestroy()
    {
        GameEvents.MouseMovListeners -= DrawTemplateStructureAt;
        GameEvents.BuildingRotationListeners -= RotateTemplate;
        GameEvents.TemplateSelectedListeners -= DestroyOldAndCreateNewTemplate;
        GameEvents.LeftClickPressedListeners -= DrawStructure;
        GameEvents.ContinuousLeftClickPressListeners -= DrawStructuresConsecutively;
        GameEvents.RightClickPressedListeners -= DeleteStructure;
    }
}
