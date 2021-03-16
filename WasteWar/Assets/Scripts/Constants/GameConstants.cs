using UnityEngine;

namespace Constants
{
    public class GameConstants : MonoBehaviour
    {
        public Vector3 DEFAULT_OBJECT_ROTATION;
        // Start is called before the first frame update
        //what's this here?
        public static GameConstants Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                //what's this here?
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }
}
