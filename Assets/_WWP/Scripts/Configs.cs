using UnityEngine;
using WWP.Game;

namespace WWP
{
    public class Configs : MonoBehaviour
    {
        public static Configs instance;
        private void Awake()
        {
            if (instance != null) return;
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }
}
