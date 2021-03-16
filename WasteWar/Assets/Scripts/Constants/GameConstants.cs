using UnityEngine;

namespace Constants
{
    public class GameConstants : MonoBehaviour
    {
        public Vector3 DEFAULT_OBJECT_ROTATION;

        public static GameConstants Instance { get; private set; }

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
