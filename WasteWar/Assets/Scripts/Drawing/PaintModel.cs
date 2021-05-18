using UnityEngine;

public class PaintModel : MonoBehaviour
{
    private MeshRenderer[] renderers;


    private void Awake()
    {
        renderers = GetComponentsInChildren<MeshRenderer>(); 
    }

    public void Paint(string mode)
    {
        if (mode.Equals("placement"))
            Placement();
        else if (mode.Equals("resourceTransmission"))
            ResourceTransmission();
    }
    public void Paint(string mode,bool isSuccessful)
    {
        if (mode.Equals("placement"))
            Placement(isSuccessful);
    }

    private void Placement()
    {
    }

    private void Placement(bool isSuccessful)
    {
        foreach (var ren in renderers)
        {
            if (isSuccessful)
                ren.material.SetColor("_Color", Color.green);
            else
                ren.material.SetColor("_Color", Color.red);
        }
    }
    private void ResourceTransmission()
    {
        foreach (var ren in renderers)
        {
            if (GetComponent<PipeState>().Full)
                ren.material.SetColor("_Color", Color.grey);
            else
                ren.material.SetColor("_Color", Color.white);
        }
    }
}
