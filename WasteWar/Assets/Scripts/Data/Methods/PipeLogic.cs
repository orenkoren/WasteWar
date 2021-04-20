using UnityEngine;

public class PipeLogic : MonoBehaviour
{
    [SerializeField]
    public Prefabs pipes;

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
        Vector3 templateSize = template.GetComponent<Renderer>().bounds.size;

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

        if (Physics.Raycast(rayUp, out hit, 1f, LayerMasks.Instance.ATTACKABLE)
        && hit.collider.gameObject.tag.Contains("Pipe")
        && CheckIfPipeAligns(Vector3.forward, hit.collider.gameObject.GetComponent<PipeState>().activeSides))
            isUp = true;
        if (Physics.Raycast(rayRight, out hit, 1f, LayerMasks.Instance.ATTACKABLE)
        && hit.collider.gameObject.tag.Contains("Pipe")
        && CheckIfPipeAligns(Vector3.right, hit.collider.gameObject.GetComponent<PipeState>().activeSides))
            isRight = true;
        if (Physics.Raycast(rayDown, out hit, 1f, LayerMasks.Instance.ATTACKABLE)
        && hit.collider.gameObject.tag.Contains("Pipe")
        && CheckIfPipeAligns(Vector3.back, hit.collider.gameObject.GetComponent<PipeState>().activeSides))
            isDown = true;
        if (Physics.Raycast(rayLeft, out hit, 1f, LayerMasks.Instance.ATTACKABLE)
        && hit.collider.gameObject.tag.Contains("Pipe")
        && CheckIfPipeAligns(Vector3.left, hit.collider.gameObject.GetComponent<PipeState>().activeSides))
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
}
