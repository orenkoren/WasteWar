using System.Collections.Generic;
using UnityEngine;

public class PipeLogic : MonoBehaviour
{
    public Prefabs pipes;
    public GameObject pipelinesPrefab;
    private List<Pipeline> pipelines;

    private void Start()
    {
        pipelines = pipelinesPrefab.GetComponent<PipelineStorage>().Pipelines;
        GameEvents.PipePlacedListeners += TraversePipeSegments;
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

        bool CheckForCondition(Ray rayDirection, out RaycastHit hit, Vector3 directedUnitVector)
        {
            return
            Physics.Raycast(rayDirection, out hit, 1f, LayerMasks.Instance.ATTACKABLE)
            && hit.collider.gameObject.tag.Contains("Pipe")
            && CheckIfPipeAligns(directedUnitVector, hit.collider.gameObject.GetComponent<PipeState>().activeSides)
            ? true : false;
        }
    }

    //TODO Make this work incase the building is placed last, and not a pipe
    private void TraversePipeSegments(object sender, GameObject structure)
    {
        Dictionary<int, GameObject> visitedPipes = new Dictionary<int,GameObject>();
        visitedPipes.Add(0, structure);
        ActiveSides activeSides = structure.GetComponent<PipeState>().activeSides;
        Pipeline pipeline = new Pipeline();

        Vector3 direction1 = Vector3.zero;
        Vector3 direction2 = Vector3.zero;
        GameObject structure1 = structure;
        GameObject structure2 = structure;

        if (activeSides.IsTop == true && direction1 == Vector3.zero)
            direction1 = Vector3.forward;
        else if (activeSides.IsTop == true)
            direction2 = Vector3.forward;

        if (activeSides.IsRight == true && direction1 == Vector3.zero)
            direction1 = Vector3.right;
        else if (activeSides.IsRight == true)
            direction2 = Vector3.right;

        if (activeSides.IsBottom == true && direction1 == Vector3.zero)
            direction1 = Vector3.back;
        else if (activeSides.IsBottom == true)
            direction2 = Vector3.back;

        if (activeSides.IsLeft == true && direction1 == Vector3.zero)
            direction1 = Vector3.left;
        else if (activeSides.IsLeft == true)
            direction2 = Vector3.left;

        int i = 0;

        while (structure1 != null && !structure1.CompareTag("Building")){
            structure1 = NextPipeSegment(structure1, ref direction1);
            if (structure1 != null)
                visitedPipes.Add(--i, structure1);
        }
        int n = i;
        i = 0;

        while (structure2 != null && !structure2.CompareTag("Building")){
            structure2 = NextPipeSegment(structure2, ref direction2);
            if (structure2 != null)
                visitedPipes.Add(++i, structure2);
        }

        if ((structure1!=null && structure2!=null) && (structure1.CompareTag("Building") && structure2.CompareTag("Building")))
        {
            pipeline.buildings.Add(visitedPipes[i--]);
            while (i > n)
            {
                pipeline.pipes.Add(visitedPipes[i--]);
            }
            pipeline.buildings.Add(visitedPipes[i]);
            pipelines.Add(pipeline);
            foreach (var pipe in pipelines[0].pipes)
                pipe.GetComponent<MeshRenderer>().material.SetColor("_Color", Color.yellow);
            foreach (var building in pipelines[0].buildings)
                building.GetComponent<MeshRenderer>().material.SetColor("_Color", Color.yellow);
        }
    }

    private GameObject NextPipeSegment(GameObject pipe, ref Vector3 dir)
    {
        Vector3 pipeCenter = new Vector3(pipe.transform.position.x, pipe.transform.position.y, pipe.transform.position.z);
        RaycastHit hit;
        Ray ray = new Ray(pipeCenter, dir);
        ActiveSides activeSides;

        if (Physics.Raycast(ray, out hit, 1f, LayerMasks.Instance.ATTACKABLE))
        {
            if (hit.collider.gameObject.tag.Contains("Pipe"))
            {
                dir=GetNextDirection(dir);
                return hit.collider.gameObject;
            }
            else if (hit.collider.gameObject.CompareTag("Building"))
                return hit.collider.gameObject;

            return null;
        }
        return null;

        Vector3 GetNextDirection(Vector3 dir)
        {
            activeSides = hit.collider.gameObject.GetComponent<PipeState>().activeSides;
            if (activeSides.IsTop && dir != Vector3.back)
                dir = Vector3.forward;
            else if (activeSides.IsRight && dir != Vector3.left)
                dir = Vector3.right;
            else if (activeSides.IsBottom && dir != Vector3.forward)
                dir = Vector3.back;
            else if (activeSides.IsLeft && dir != Vector3.right)
                dir = Vector3.left;

            return dir;
        }
    }

    private void OnDestroy()
    {
        GameEvents.PipePlacedListeners -= TraversePipeSegments;
    }
}
