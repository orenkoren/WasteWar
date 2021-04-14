using UnityEngine;
using System;

public class PipeState : MonoBehaviour
{
    public bool Full { get; private set; }
    private PipeDirection pipeDirection;

    // Start is called before the first frame update
    void Start()
    {
        Full = false;
        pipeDirection = new PipeDirection();

    }
   
    public void AdjustDirectionsThroughRotation()
    {
        pipeDirection.From = (pipeDirection.From + 2) % 4;
        pipeDirection.To = (pipeDirection.To + 2) % 4;
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
        Ray rayUp = new Ray(templateCenter, new Vector3(0,0,1f));
        Ray rayRight = new Ray(templateCenter, new Vector3(1f,0,0));
        Ray rayDown = new Ray(templateCenter, new Vector3(0,0,-1f));
        Ray rayLeft = new Ray(templateCenter, new Vector3(-1f,0,0));

        if (Physics.Raycast(rayUp, out hit, 1f, LayerMasks.Instance.ATTACKABLE)
        && hit.collider.gameObject.tag == "Pipes")
        {
            Debug.Log("kekw");
            isUp = true;
        }
        if (Physics.Raycast(rayRight, out hit, 1f, LayerMasks.Instance.ATTACKABLE)
        && hit.collider.gameObject.tag == "Pipes")
        {
            Debug.Log("kekw");
            isRight = true;
        }
        if (Physics.Raycast(rayDown, out hit, 1f, LayerMasks.Instance.ATTACKABLE)
        && hit.collider.gameObject.tag == "Pipes")
        {
            isDown = true;
            Debug.Log("kekw");
        }
        if (Physics.Raycast(rayLeft, out hit, 1f, LayerMasks.Instance.ATTACKABLE)
        && hit.collider.gameObject.tag == "Pipes")
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
public class PipeDirection
{
    enum FromTo
    {
        TOP=1,
        RIGHT=2,
        BOTTOM=3,
        LEFT=4
    }
    public int From { get; set; }
    public int To { get; set; }

    public PipeDirection()
    {
        From = (int)FromTo.LEFT;
        To = (int)FromTo.RIGHT;
    }
}