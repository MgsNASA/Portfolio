using UnityEngine;

namespace GooyesPlugin
{

    public class PoolObject : MonoBehaviour
    {
        #region Fields
        [HideInInspector] public string poolTag;
        [HideInInspector] public int id;
        [HideInInspector] public float ActivationTime;

        [SerializeField] public float lifetime = -1.0f;

        private float _timer;
        #endregion

        #region Unity Event Functions
        private void OnEnable()
        {
            ActivationTime = Time.timeSinceLevelLoad;
            _timer = 0.0f;
        }

        private void Update()
        {
            if (lifetime > 0.0f)
            {
                _timer += Time.deltaTime;
                if (_timer >= lifetime)
                {
                    gameObject.Destroy_Pool();
                    _timer = 0.0f;
                }
            }
        }

        #endregion

        #region Public
        public void Init(string TagToSet, int IDToSet)
        {
            poolTag = TagToSet;
            id = IDToSet;
        }
        #endregion

        public override string ToString()
        {
            return ($"{poolTag} {id} Time: {ActivationTime}");
        }
    }
}
