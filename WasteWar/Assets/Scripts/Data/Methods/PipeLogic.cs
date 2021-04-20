using UnityEngine;

public class PipeLogic : MonoBehaviour
{
    [SerializeField]
    public Prefabs pipes;

    private void Start()
    {
        GameEvents.PipePlacedListeners += ConstructPipeline;
    }

    public bool CheckIfPipeAligns(Vector3 dir, ActiveSides activeSides)
    {
        if (dir == Vector3.forward)
            return activeSides.IsBottom;
        else if (dir == Vector3.right)
            return activeSides.IsLeft;
        else if (dir == Vector3.back)
            return activeSides.IsTop;
        else if (dir == Vector3.left)
            return activeSides.IsRight;
        return false;
    }

    public GameObject CheckNeighbors(GameObject template)
    {
        bool isUp = false;
        bool isRight = false;
        bool isDown = false;
        bool isLeft = false;

        Vector3 templateCenter = new Vector3(template.transform.position.x, template.transform.position.y, template.transform.position.z);
        RaycastHit hit;
        Ray rayUp = new Ray(templateCenter, Vector3.forward);
        Ray rayRight = new Ray(templateCenter, Vector3.right);
        Ray rayDown = new Ray(templateCenter, Vector3.back);
        Ray rayLeft = new Ray(templateCenter, Vector3.left);


        if(CheckForCondition(rayUp, out hit, Vector3.forward))
            isUp = true;
        if(CheckForCondition(rayRight, out hit, Vector3.right))
            isRight = true;
        if (CheckForCondition(rayDown, out hit, Vector3.back))
            isDown = true;
        if (CheckForCondition(rayLeft, out hit, Vector3.left))
            isLeft = true;

        if (isUp && isDown)
            return pipes.TopBottom;
        else if (isLeft && isRight)
            return pipes.LeftRight;
        else if (isUp && isRight)
            return pipes.TopRight;
        else if (isDown && isRight)
            return pipes.BottomRight;
        else if (isDown && isLeft)
            return pipes.BottomLeft;
        else if (isUp && isLeft)
            return pipes.TopLeft;

        return template;
    }

    public void ConstructPipeline(object sender, GameObject structure)
    {
        ActiveSides activeSides = structure.GetComponent<PipeState>().activeSides;



        //if (!(structure.tag.Contains("Building"))){
        //    ConstructPipeline(sender, structure);
        //    ConstructPipeline(sender, structure);
        //}

    }

    private bool CheckForCondition(Ray rayDirection,out RaycastHit hit,Vector3 directedUnitVector)
    {
       return 
       Physics.Raycast(rayDirection, out hit, 1f, LayerMasks.Instance.ATTACKABLE)
       && hit.collider.gameObject.tag.Contains("Pipe")
       && CheckIfPipeAligns(directedUnitVector, hit.collider.gameObject.GetComponent<PipeState>().activeSides) 
       ? true : false;
    }

    private void OnDestroy()
    {
        GameEvents.PipePlacedListeners -= ConstructPipeline;
    }
}
