using UnityEngine;

namespace Constants
{
    public class GameKeys : MonoBehaviour
    {
        public KeyCode[] StructureKeybinds { get; private set; } = new KeyCode[] { KeyCode.B, KeyCode.C,KeyCode.X, KeyCode.Escape };
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

