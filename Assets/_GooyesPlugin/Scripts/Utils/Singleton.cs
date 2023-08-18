using UnityEngine;

namespace GooyesPlugin
{
    public class Singleton<T> : MonoBehaviour where T : Singleton<T>
    {
        public delegate void InitComplete();
        public static event InitComplete EventInitComplete;

        #region Fields
        protected bool _inited = false;
        protected bool _dontDestroyOnLoad = false;
        #endregion

        #region Static
        protected static T _instance;

        public static bool Exists
        {
            get { return _instance != null; }
        }

        public static T Instance
        {
            get
            {
                if (_instance == null)
                    Create();
                return _instance;
            }
        }

        protected static void Create()
        {
            GameObject go = new GameObject();
            go.name = typeof(T).Name;
            _instance = go.AddComponent(typeof(T)) as T;
        }

        public static void Destroy()
        {
            if (_instance != null && _instance.gameObject != null)
            {
                Destroy(_instance.gameObject);
            }
            _instance = null;
        }
        #endregion

        #region Unity Event Functions
        void Awake()
        {
            if (_instance == null)
            {
                if (!_inited)
                    InternalInit();
                _instance = this as T;
            }
            else
            {
                if (_instance != this)
                {
                    Destroy(gameObject);
                    Debug.LogError($"Two singletons at a time!", _instance.gameObject);
                }
            }
        }
        #endregion

        #region Virtual Functions
        protected void InternalInit()
        {
            if (_dontDestroyOnLoad)
            {
                DontDestroyOnLoad(this.gameObject);
            }
            Init();
            _inited = true;
            EventInitComplete?.Invoke();
        }

        protected virtual void Init()
        {
            InternalInit();
        }

        virtual public void StartUp()
        {
            // Use for creating with no parameters in proper order while game is starting up.
        }
        #endregion
    }
}
