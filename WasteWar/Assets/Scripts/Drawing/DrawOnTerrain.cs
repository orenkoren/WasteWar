using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DrawOnTerrain : MonoBehaviour
{
    [SerializeField]
    private PrefabPlaceable placeablePrefabs;
    [SerializeField]
    private Camera cam;
    [SerializeField]
    private GameObject pipeMethods;
    [SerializeField]
    RuntimeGameObjRefs runtimeGameObjRefs;

    public GameObject TemplateStructure { get; set; }
    public Vector3 TemplateStructureSize { get; private set; }

    private Terrain terrain;
    private RaycastHit hit;
    private Ray ray;    
    private List<GameObject> structures = new List<GameObject>();
    private int rotationState = 0;

    private void Start()
    {
        terrain = runtimeGameObjRefs.terrain;

        GameEvents.BuildingRotationListeners += RotateTemplate;
        GameEvents.TemplateSelectedListeners += DestroyOldAndCreateNewTemplate;
        GameEvents.LeftClickPressedListeners += DrawStructure;
        GameEvents.ContinuousLeftClickPressListeners += DrawStructuresConsecutively;
        GameEvents.RightClickPressedListeners += DeleteStructure;
    }

    private void Update()
    {
        ray = cam.ScreenPointToRay(Input.mousePosition);
        if (Physics.Raycast(ray, out hit, CameraConstants.Instance.RAYCAST_DISTANCE, LayerMasks.Instance.GROUND)
            && TemplateStructure != null && MathUtils.CursorIsWithinBounds(hit.point, terrain.terrainData.size))
            DrawTemplateStructureAt(hit.point);
    }

    public void DrawTemplateStructureAt(Vector3 loc)
    {
        if (CheckIfLocationIsFree())
            SetTemplateStructureColor(Color.green);
        else
            SetTemplateStructureColor(Color.red);
        SetTemplateStructurePos(loc);
    }

    //TODO make placement near edges of the map smoother
    private void DrawStructure(object sender, TemplateData data)
    {
        if (data.TemplateStructure != null && CheckIfLocationIsFree())
        {
            GameObject Structure= PlaceablePrefabInstantiator(placeablePrefabs.GetPlaceablePrefab(TemplateStructure), data.MousePos);
            if (Structure.CompareTag("Building"))
               GameEvents.FireBuildingPlaced(this, Structure);
           
            structures.Add(Structure);
        }
        else if(data.TemplateStructure != null && !CheckIfLocationIsFree())
            StartCoroutine(ShakeTemplateForXSec());
    }
    private void DrawStructuresConsecutively(object sender, TemplateData data)
    {
        if (data.TemplateStructure != null && data.TemplateStructure.tag.Contains("Pipe") && CheckIfLocationIsFree())
        {
            GameObject Structure = PlaceablePrefabInstantiator(placeablePrefabs.GetPlaceablePrefab(TemplateStructure), data.MousePos);
        
            structures.Add(Structure);

            GameObject curvedPipeTemplate = pipeMethods.GetComponent<PipeLogic>().ChooseNextPipe(structures[structures.Count - 1], data.MousePos);

            if (curvedPipeTemplate != null)
            {
                //TODO refactor for performance (always going to the end of the list)
                Vector3 prevPos = structures[structures.Count - 2].transform.position;
                Destroy(structures[structures.Count - 2]);
                structures[structures.Count - 2] = PlaceablePrefabInstantiatorWithKnownPos(placeablePrefabs.GetPlaceablePrefab(curvedPipeTemplate), prevPos);
                pipeMethods.GetComponent<PipeLogic>().pipeList[pipeMethods.GetComponent<PipeLogic>().pipeList.Count - 2] = structures[structures.Count - 2];
            }

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
        TemplateStructureSize = TemplateStructure.GetComponent<Collider>().bounds.size;
    }
    //necessary because our pipes (the ones made in blender) are not 1x1x1
    private void TemplateInstantiatorForCurvedPipes(GameObject template, Vector3 mousePos)
    {
        //TODO quickhack until the model is fixed (it goes out of bounds of the 1,1,1 cube,model needs to fit the square basically)
        Vector3 size = new Vector3(1f, 1f, 1f);

        Destroy(TemplateStructure);
        TemplateStructure = Instantiate(template,
                            ObjectSnapper.SnapToGridCell(mousePos,size),
                            template.transform.rotation);
        TemplateStructure.layer = LayerMasks.Instance.IGNORE_RAYCAST_LAYER;
        TemplateStructureSize = TemplateStructure.GetComponent<Renderer>().bounds.size;
    }

    private GameObject PlaceablePrefabInstantiator(GameObject template, Vector3 mousePos)
    {
        Vector3 size;

        //TODO quickhack until the model is fixed (it goes out of bounds of the 1,1,1 cube,model needs to fit the square basically)
        if (template.tag.Contains("PipeB") || (template.tag.Equals("PipeTL") || template.tag.Equals("PipeTR")))
            size = new Vector3(1f, 1f, 1f);
        else
            size = template.GetComponent<Collider>().bounds.size;

        template.layer = LayerMasks.Instance.ATTACKABLE_LAYER;

        return Instantiate(template,
                           ObjectSnapper.SnapToGridCell(mousePos, size),
                           template.transform.rotation);
    }

    private GameObject PlaceablePrefabInstantiatorWithKnownPos(GameObject template, Vector3 pos)
    {
        Vector3 size;

        //TODO quickhack until the model is fixed (it goes out of bounds of the 1,1,1 cube,model needs to fit the square basically)
        if (template.tag.Contains("Pipe") && TemplateStructure.tag.Length > 4)
            size = new Vector3(1f, 1f, 1f);
        else
            size = template.GetComponent<Collider>().bounds.size;

        template.layer = LayerMasks.Instance.ATTACKABLE_LAYER;

        return Instantiate(template,
                           pos,
                           template.transform.rotation);
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
                        TemplateInstantiatorForCurvedPipes(pipeMethods.GetComponent<PipeLogic>().templates.PipeTopLeft, pos);
                        break;
                    case 1:
                        TemplateInstantiatorForCurvedPipes(pipeMethods.GetComponent<PipeLogic>().templates.PipeTopRight, pos);
                        break;
                    case 2:
                        TemplateInstantiatorForCurvedPipes(pipeMethods.GetComponent<PipeLogic>().templates.PipeBottomRight, pos);
                        break;
                    case 3:
                        TemplateInstantiatorForCurvedPipes(pipeMethods.GetComponent<PipeLogic>().templates.PipeBottomLeft, pos);
                        break;
                }
                rotationState = (rotationState + 1) % 4;         
            }
            GameEvents.FirePipePlaced2(this, TemplateStructure.tag);
        }
    }

    private void SetTemplateStructureColor(Color color)
    {
        if(TemplateStructure.GetComponent<MeshRenderer>() !=null)
        TemplateStructure.GetComponent<MeshRenderer>().material.color = color;
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
                   Physics.Raycast(rayTwo, CameraConstants.Instance.RAYCAST_DISTANCE, LayerMasks.Instance.RESOURCE);
        }
        else
            return !Physics.CheckBox(TemplateStructure.GetComponent<Collider>().bounds.center, TemplateStructure.GetComponent<Collider>().bounds.extents,
                TemplateStructure.transform.rotation, LayerMasks.Instance.ATTACKABLE);
    }

    private IEnumerator ShakeTemplateForXSec()
    {
        float countdown = 0.66f;
        Vector3 tempPos = new Vector3(TemplateStructure.transform.position.x, TemplateStructure.transform.position.y, TemplateStructure.transform.position.z);
        float time = 0;

        while (countdown >= 0)
        {
            ShakeTemplate();
            countdown -= Time.deltaTime;
            yield return null;
        }
        TemplateStructure.transform.position = tempPos;

        void ShakeTemplate()
        {
             float speed = 50f;
             float amount = 0.1f;

            TemplateStructure.transform.position = new Vector3(TemplateStructure.transform.position.x + Mathf.Sin(time * speed) * amount,
                                                               TemplateStructure.transform.position.y,
                                                               TemplateStructure.transform.position.z + Mathf.Sin(time * speed) * amount
                                                               );
            time += Time.deltaTime;
        }
    }

    private void OnDestroy()
    {
        GameEvents.BuildingRotationListeners -= RotateTemplate;
        GameEvents.TemplateSelectedListeners -= DestroyOldAndCreateNewTemplate;
        GameEvents.LeftClickPressedListeners -= DrawStructure;
        GameEvents.ContinuousLeftClickPressListeners -= DrawStructuresConsecutively;
        GameEvents.RightClickPressedListeners -= DeleteStructure;
    }
}
