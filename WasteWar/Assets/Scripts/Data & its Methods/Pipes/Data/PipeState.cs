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
