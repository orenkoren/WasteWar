using UnityEngine;

public class PipeState : MonoBehaviour
{
    public bool Full { get; private set; }
    public ActiveSides activeSides;

    private void Awake()
    {
        Full = false;
        activeSides = new ActiveSides(gameObject);
    }
   
    public void Rotate()
    {
        activeSides.IsTop = !activeSides.IsTop;
        activeSides.IsRight = !activeSides.IsRight;
        activeSides.IsBottom = !activeSides.IsBottom;
        activeSides.IsLeft = !activeSides.IsLeft; 
    }
    
    public bool CheckIfPipeAligns(Vector3 dir)
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

    public GameObject CheckNeighbors(Prefabs pipes)
    {
        Vector3 templateSize = gameObject.GetComponent<Renderer>().bounds.size;

        bool isUp = false;
        bool isRight = false;
        bool isDown = false;
        bool isLeft = false;

        Vector3 templateCenter = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y, gameObject.transform.position.z );
        RaycastHit hit;
        Ray rayUp = new Ray(templateCenter, Vector3.forward);
        Ray rayRight = new Ray(templateCenter, Vector3.right);
        Ray rayDown = new Ray(templateCenter, Vector3.back);
        Ray rayLeft = new Ray(templateCenter, Vector3.left);
        
        if (Physics.Raycast(rayUp, out hit, 1f, LayerMasks.Instance.ATTACKABLE)
        && hit.collider.gameObject.tag.Contains("Pipe")
        && hit.collider.gameObject.GetComponent<PipeState>().CheckIfPipeAligns(Vector3.forward))
        {
            Debug.Log("kekw");
            isUp = true;
        }
        if (Physics.Raycast(rayRight, out hit, 1f, LayerMasks.Instance.ATTACKABLE)
        && hit.collider.gameObject.tag.Contains("Pipe")
        && hit.collider.gameObject.GetComponent<PipeState>().CheckIfPipeAligns(Vector3.right))
        {
            Debug.Log("kekw");
            isRight = true;
        }
        if (Physics.Raycast(rayDown, out hit, 1f, LayerMasks.Instance.ATTACKABLE)
        && hit.collider.gameObject.tag.Contains("Pipe")
        && hit.collider.gameObject.GetComponent<PipeState>().CheckIfPipeAligns(Vector3.back))
        {
            isDown = true;
            Debug.Log("kekw");
        }
        if (Physics.Raycast(rayLeft, out hit, 1f, LayerMasks.Instance.ATTACKABLE)
        && hit.collider.gameObject.tag.Contains("Pipe")
        && hit.collider.gameObject.GetComponent<PipeState>().CheckIfPipeAligns(Vector3.left)) 
        {
            Debug.Log("kekw");
            isLeft = true;
        }

        if (isUp && isDown)
            return pipes.PipeTopBottom;
        else if (isLeft && isRight)
            return pipes.PipeLeftRight;
        else if (isUp && isRight)
            return pipes.PipeTopRight;
        else if (isDown && isRight)
            return pipes.PipeBottomRight;
        else if (isDown && isLeft)
            return pipes.PipeBottomLeft;
        else if (isUp && isLeft)
            return pipes.PipeTopLeft;

        return gameObject;
    }
}

//DEFAULT PIPEDIRECTION INSTANCE VARIABLES DEPEND ON OBJECT ROTATION, IF YOU CHANGE THE PREFAB OBJECT ROTATION THEY WILL BE WRONG
public class ActiveSides
{
    public bool IsTop { get; set; }
    public bool IsRight { get; set; }
    public bool IsBottom { get; set; } 
    public bool IsLeft { get; set; }

    public ActiveSides(GameObject gameObject)
    {
        switch (gameObject.tag) {
            case ("PipeTB"):
                IsTop = true;
                IsBottom = true;
                IsLeft = false;
                IsRight = false;
                break;
            case ("PipeLR"):
                IsTop = false;
                IsBottom = false;
                IsLeft = true;
                IsRight = true;
                break;
            case ("PipeTR"):
                IsTop = true;
                IsBottom = false;
                IsLeft = false;
                IsRight = true;
                break;
            case ("PipeBR"):
                IsTop = false;
                IsBottom = true;
                IsLeft = false;
                IsRight = true;
                break;
            case ("PipeBL"):
                IsTop = false;
                IsBottom = true;
                IsLeft = true;
                IsRight = false;
                break;
            case ("PipeTL"):
                IsTop = true;
                IsBottom = false;
                IsLeft = true;
                IsRight = false;
                break;
        }
    }
}
