using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PipeLogic : MonoBehaviour
{
    public PrefabTemplates templates;
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

        if (CheckForCondition(rayUp, out hit, Vector3.forward))
            isUp = true;
        if (CheckForCondition(rayRight, out hit, Vector3.right))
            isRight = true;
        if (CheckForCondition(rayDown, out hit, Vector3.back))
            isDown = true;
        if (CheckForCondition(rayLeft, out hit, Vector3.left))
            isLeft = true;

        if (isUp && isDown)
            return templates.PipeTopBottom;
        else if (isLeft && isRight)
            return templates.PipeLeftRight;
        else if (isUp && isRight)
            return templates.PipeTopRight;
        else if (isDown && isRight)
            return templates.PipeBottomRight;
        else if (isDown && isLeft)
            return templates.PipeBottomLeft;
        else if (isUp && isLeft)
            return templates.PipeTopLeft;

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
        ActiveSides activeSides = structure.GetComponent<PipeState>().activeSides;
        Pipeline pipeline = new Pipeline();
        GameObject lastPipeBeforeBuilding = null;

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


        while (CheckStructureType(structure1))
            structure1 = NextPipeSegment(structure1, ref direction1, ref lastPipeBeforeBuilding);
        while (CheckStructureType(structure2))
            structure2 = NextPipeSegment(structure2, ref direction2, ref lastPipeBeforeBuilding);

        if (CheckIfStructuresSatisfyPipeline())
        {
            GeneratePipeline();

            //DEBUGOUTPUT
            foreach (var pipe in pipeline.pipes)
                pipe.GetComponent<MeshRenderer>().material.color = Color.yellow;
            foreach (var building in pipeline.buildings)
                building.GetComponent<MeshRenderer>().material.color = Color.yellow;

            StartCoroutine(StartResourceTransferFrom(pipeline));
        }

        void GeneratePipeline()
        {
            //TODO fix code duplciation
            if (structure1.CompareTag("Building"))
            {
                pipeline.buildings.Add(structure1);
                direction1 = -direction1;
                structure1 = lastPipeBeforeBuilding;
                pipeline.pipes.Add(lastPipeBeforeBuilding);
                while (CheckStructureType(structure1))
                {
                    structure1 = NextPipeSegment(structure1, ref direction1, ref lastPipeBeforeBuilding);
                    if (structure1.tag.Contains("Pipe"))
                        pipeline.pipes.Add(structure1);
                }
                pipeline.buildings.Add(structure1);
            }
            else if (structure2.CompareTag("Building"))
            {
                pipeline.buildings.Add(structure2);
                direction2 = -direction2;
                structure2 = lastPipeBeforeBuilding;
                pipeline.pipes.Add(lastPipeBeforeBuilding);
                while (CheckStructureType(structure2))
                {
                    structure2 = NextPipeSegment(structure2, ref direction2, ref lastPipeBeforeBuilding);
                    if (structure2.tag.Contains("Pipe"))
                        pipeline.pipes.Add(structure2);
                }
                pipeline.buildings.Add(structure2);
            }
            pipeline.pipes.Reverse();
            pipelines.Add(pipeline);
        }
        bool CheckStructureType(GameObject structure)
        {
            return structure != null && (!structure.CompareTag("Building") && !structure.CompareTag("Base"));
        }
        bool CheckIfStructuresSatisfyPipeline()
        {
            bool condition1 = structure1 != null && structure2 != null ? true : false;

            if (condition1)
                return (structure1.CompareTag("Building") && structure2.CompareTag("Base")) ||
                        (structure1.CompareTag("Base") && structure2.CompareTag("Building"));
            else
                return false;
        }
    }

    private IEnumerator StartResourceTransferFrom(Pipeline pipeline)
    {
        BuildingState giver;
        BaseData taker;

        giver = pipeline.buildings[0].GetComponent<BuildingState>();
        taker = pipeline.buildings[1].GetComponent<BaseData>();

        List<GameObject> pipes = pipeline.pipes;

        bool firstRun = true;

        while (giver.Storage > 0)
        {
            if (firstRun)
            {
                pipes[pipes.Count - 1].GetComponent<PipeState>().Full = true;
                pipes[pipes.Count - 1].GetComponent<MeshRenderer>().material.color = Color.grey;
                giver.Storage--;
                giver.CubeTextComponent.text = giver.Storage.ToString();

                firstRun = false;
            }

            else
            {

                if (pipes[0].GetComponent<PipeState>().Full == true)
                {
                    pipes[0].GetComponent<PipeState>().Full = false;
                    pipes[0].GetComponent<MeshRenderer>().material.color = Color.white;
                    taker.Storage++;
                    taker.CubeTextComponent.text = taker.Storage.ToString();
                }

                for (int i = 0; i < pipeline.pipes.Count - 1; i++)
                {
                    if (pipes[i + 1].GetComponent<PipeState>().Full == true)
                    {
                        pipes[i].GetComponent<PipeState>().Full = true;
                        pipes[i + 1].GetComponent<PipeState>().Full = false;

                        pipes[i].GetComponent<MeshRenderer>().material.color = Color.grey;
                        pipes[i + 1].GetComponent<MeshRenderer>().material.color = Color.white;
                    }
                }

                pipes[pipes.Count - 1].GetComponent<PipeState>().Full = true;
                pipes[pipes.Count - 1].GetComponent<MeshRenderer>().material.color = Color.grey;
                giver.Storage--;
                giver.CubeTextComponent.text = giver.Storage.ToString();
            }
            yield return new WaitForSeconds(giver.GetComponent<BuildingState>().YieldFrequency);
        }

        while (giver.Storage <=0 && pipes.Find(pipe => pipe.GetComponent<PipeState>().Full == true))
        {

            if (pipes[0].GetComponent<PipeState>().Full == true)
            {
                pipes[0].GetComponent<PipeState>().Full = false;
                pipes[0].GetComponent<MeshRenderer>().material.color = Color.white;
                taker.Storage++;
                taker.CubeTextComponent.text = taker.Storage.ToString();
            }

            for (int i = 0; i < pipeline.pipes.Count - 1; i++)
            {
                if (pipes[i + 1].GetComponent<PipeState>().Full == true)
                {
                    pipes[i].GetComponent<PipeState>().Full = true;
                    pipes[i + 1].GetComponent<PipeState>().Full = false;

                    pipes[i].GetComponent<MeshRenderer>().material.color = Color.grey;
                    pipes[i + 1].GetComponent<MeshRenderer>().material.color = Color.white;
                }
            }
            yield return new WaitForSeconds(giver.GetComponent<BuildingState>().YieldFrequency);
        }
    }

    private GameObject NextPipeSegment(GameObject pipe, ref Vector3 dir, ref GameObject lastPipeBeforeBuilding)
    {
        Vector3 pipeCenter = new Vector3(pipe.transform.position.x, pipe.transform.position.y, pipe.transform.position.z);
        RaycastHit hit;
        Ray ray = new Ray(pipeCenter, dir);
        ActiveSides activeSides;

        if (Physics.Raycast(ray, out hit, 1f, LayerMasks.Instance.ATTACKABLE))
        {
            if (hit.collider.gameObject.tag.Contains("Pipe"))
            {
                dir = GetNextDirection(dir);
                return hit.collider.gameObject;
            }
            else if (hit.collider.gameObject.CompareTag("Building") || (hit.collider.gameObject.CompareTag("Base")))
            {
                if (hit.collider.gameObject.CompareTag("Building"))
                    lastPipeBeforeBuilding = pipe;
                return hit.collider.gameObject;
            }
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