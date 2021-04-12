using UnityEngine;

namespace Constants
{
    public class GameKeys : MonoBehaviour
    {
        public KeyCode[] StructureKeybinds { get; private set; } = new KeyCode[] 
        {
            KeyCode.B,
            KeyCode.V, 
            KeyCode.C,
            KeyCode.X,
            KeyCode.Z, 
            KeyCode.Escape, 
        };
        public KeyCode[] StructureManipulationKeybinds { get; private set; } = new KeyCode[]
        {
            KeyCode.R,
        };
        //KeyCode[] MenuKeybinds = new KeyCode[] { KeyCode.Escape };
        public static GameKeys Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}

