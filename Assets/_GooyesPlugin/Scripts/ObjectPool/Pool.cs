using System;
using System.Collections.Generic;
using UnityEngine;

namespace GooyesPlugin
{

    public class Pool : Singleton<Pool>
    {

        #region PoolObject Interface
        public interface IPoolObject
        {
            void Init();
            void Recycle();
        }
        #endregion

        #region Inner Class Definition

        [Serializable]
        public class PoolData
        {
            public enum ResizePolicy { LIMIT, REUSE, GROW }

            public string tag;
            public List<GameObject> prefabs;
            public int size;
            public ResizePolicy policy;

        }

        public class PoolGameObject
        {

            #region Fields
            private List<GameObject> _prefabs;
            private int _lastInstantiatedPrefabIndex;

            private string _tag;
            private Queue<PoolObject> _availableObjects;
            private PoolObject[] _objectsInUse;
            private PoolData.ResizePolicy _policy;

            private Transform _parent;
            private Transform _transform;
            #endregion

            public PoolGameObject(string tag, List<GameObject> prefabs, int size, PoolData.ResizePolicy policy, Transform parent)
            {
                if (prefabs != null && size > 0 && !string.IsNullOrEmpty(tag))
                {
                    _prefabs = prefabs;
                    _tag = tag;
                    _availableObjects = new Queue<PoolObject>(size);
                    _objectsInUse = new PoolObject[size];
                    _policy = policy;
                    _parent = parent;
                    _transform = new GameObject(tag).transform;
                    _transform.SetParent(_parent);

                    for (int i = 0; i < size; ++i)
                    {
                        InitNewObject(i);
                    }
                }
            }

            public int Count()
            {
                return _transform.childCount;
            }

            public GameObject Get(Vector3 position, Quaternion rotation, bool setPosAndRot)
            {
                if (!HasAvailableObjects())
                {
                    switch (_policy)
                    {
                        case PoolData.ResizePolicy.GROW:
                            {
                                int arraySize = _objectsInUse.Length;
                                InitNewObject(arraySize++);

                                //Debug.LogWarning($"Add NEW OBJECT: {Tag} POOL SIZE: {PoolTransformn.childCount}  TheObjectPoolSIZE: {Parent.childCount} ");

                                ResizeObjectsInGameArray(arraySize);
                            }
                            break;
                        case PoolData.ResizePolicy.LIMIT:
                            break;
                        case PoolData.ResizePolicy.REUSE:
                            {
                                ReuseTheOldestObject();
                            }
                            break;
                    }
                }
                return GetAvailableObject(position, rotation, setPosAndRot);
            }

            public void SetActive(PoolObject obj, bool active)
            {
                if (obj)
                {
                    if (obj.id >= 0 && obj.id < _objectsInUse.Length)
                    {
                        _objectsInUse[obj.id].gameObject?.SetActive(active);
                    }
                }
            }

            public void SetBack(PoolObject obj)
            {
                if (obj)
                {
                    PoolObject inUse = RemoveObjectFromUse(obj.id);
                    if (inUse)
                    {
                        inUse.gameObject.GetComponent<IPoolObject>()?.Recycle();
                        inUse.transform.SetParent(_transform, false);
                        inUse.gameObject.SetActive(false);
                        _availableObjects.Enqueue(inUse);
                    }
                }
            }

            public void Clear()
            {
                if (_availableObjects != null)
                {
                    foreach (PoolObject obj in _availableObjects)
                    {
                        if (obj && obj.gameObject)
                        {
                            Destroy(obj.gameObject);
                        }
                    }
                }
                if (_objectsInUse != null)
                {
                    foreach (PoolObject obj in _objectsInUse)
                    {
                        if (obj && obj.gameObject)
                        {
                            Destroy(obj.gameObject);
                        }
                    }
                }
            }

            #region Helpers
            private bool HasAvailableObjects()
            {
                return _availableObjects != null && _availableObjects.Count > 0;
            }

            private GameObject GetAvailableObject(Vector3 position, Quaternion rotation, bool setPosAndRot)
            {
                if (_availableObjects.Count > 0)
                {
                    PoolObject obj = _availableObjects.Dequeue();
                    if (setPosAndRot)
                    {
                        obj.transform.SetPositionAndRotation(position, rotation);
                    }
                    obj.gameObject.SetActive(true);
                    _objectsInUse[obj.id] = obj;
                    obj.gameObject.GetComponent<IPoolObject>()?.Init();
                    return obj.gameObject;
                }
                return null;
            }

            private PoolObject RemoveObjectFromUse(int id)
            {
                if (id >= 0 || id < _objectsInUse.Length)
                {
                    PoolObject obj = _objectsInUse[id];
                    if (obj)
                    {
                        _objectsInUse[id] = null;
                        return obj;
                    }
                }
                else
                {
                    Debug.LogError($"[ERROR] ObjectsInUse Capacity is nit proper! PoolID: {id} Capacity: {_objectsInUse.Length}");
                }
                return null;
            }

            private void InitNewObject(int id)
            {
                _lastInstantiatedPrefabIndex %= _prefabs.Count;
                GameObject currentPrefab = _prefabs[_lastInstantiatedPrefabIndex++];
                GameObject obj = Instantiate(currentPrefab);
                obj.name = currentPrefab.name;
                if (obj)
                {
                    PoolObject poolObj = obj.GetComponent<PoolObject>();
                    if (poolObj)
                    {
                        poolObj.Init(_tag, id);
                        obj.transform.SetParent(_transform);
                        obj.SetActive(false);

                        _availableObjects.Enqueue(poolObj);
                    }
                }
                else
                {
                    Debug.LogError($"[ERROR] Error while adding objects to the pool!");
                }

            }

            private void ResizeObjectsInGameArray(int newSize)
            {
                if (newSize > _objectsInUse.Length)
                {
                    PoolObject[] newArray = new PoolObject[newSize];
                    for (int i = 0; i < _objectsInUse.Length; ++i)
                    {
                        newArray[i] = _objectsInUse[i];
                    }
                    _objectsInUse = newArray;
                }
                else
                {
                    Debug.LogError($"[ERROR] Unable to shrink Array! You may implement it if you want!");
                }
            }

            private void ReuseTheOldestObject()
            {
                if (_objectsInUse != null)
                {
                    float oldestTime = -1.0f;
                    int index = -1;
                    for (int i = 0; i < _objectsInUse.Length; ++i)
                    {
                        PoolObject obj = _objectsInUse[i];
                        if (obj)
                        {
                            if (oldestTime < 0)
                            {
                                oldestTime = obj.ActivationTime;
                                index = i;
                            }
                            else if (oldestTime < obj.ActivationTime)
                            {
                                oldestTime = obj.ActivationTime;
                                index = i;
                            }
                        }
                    }
                    ForceRemoveObjectFromUse(index);
                }
            }

            private void ForceRemoveObjectFromUse(int index)
            {
                if (index >= 0 && index < _objectsInUse.Length)
                {
                    PoolObject obj = _objectsInUse[index];
                    if (obj)
                    {
                        obj.GetComponent<IPoolObject>()?.Recycle();
                        _objectsInUse[index] = null;
                        obj.gameObject.SetActive(false);
                        _availableObjects.Enqueue(obj);
                        return;
                    }
                }
                Debug.LogError($"[ERROR] RemoveObjectFromUse:( {_tag} {index}");
            }
            #endregion

        }

        #endregion

        #region Fields
        [SerializeField]
        protected Camera _camera;

        [SerializeField]
        private List<PoolData> _pools;
        private Dictionary<string, PoolGameObject> _poolMap;

        [SerializeField]
        protected List<GameObject> _sceneObjects;
        private Dictionary<string, GameObject> _sceneObjectsMap;

        [SerializeField]
        protected List<Material> _materials;
        #endregion

        #region Overrides
        protected override void Init()
        {
            if (_sceneObjects != null)
            {
                _sceneObjectsMap = new Dictionary<string, GameObject>();
                foreach (GameObject obj in _sceneObjects)
                {
                    if (!_sceneObjectsMap.ContainsKey(obj.name))
                    {
                        _sceneObjectsMap.Add(obj.name, obj);
                    }
                }
            }

            if (_pools != null)
            {
                foreach (PoolData pool in _pools)
                {
                    InitSinglePool(pool);
                }
            }


            _camera = Camera.main;
            if (!_camera)
            {
                Debug.LogError($"Unable to find main Camera on the scene!", gameObject);
            }
        }

        #endregion

        #region Public
        public static GameObject PlayVFX(string id, Vector3 pos, Quaternion rot)
        {
            if (Exists)
            {
                if (_instance._camera ? _instance._camera.IsOnScreen(pos) : false)
                {
                    return _instance.InternalGet(id, pos, rot);
                }
            }
            return null;
        }

        public static Material GetMaterial(int index)
        {
            if (Exists)
            {
                if (Instance._materials != null)
                {
                    if (index >= 0 && index < Instance._materials.Count)
                    {
                        return Instance._materials[index];
                    }
                }
            }
            return null;
        }

        public static GameObject GetSceneObject(string tag)
        {
            return Exists ? _instance.InternalGetSceneObject(tag) : null;
        }

        public static Transform GetTransform()
        {
            return Exists ? _instance.transform : null;
        }

        public static GameObject Get(string tag, bool setPosAndRot = true)
        {
            return Get(tag, Vector3.zero, Quaternion.identity, setPosAndRot);
        }

        public static bool Has(string tag)
        {
            return Exists && _instance.InternalHas(tag);
        }

        public static GameObject Get(string tag, Vector3 position, Quaternion rotation, bool setPosAndRot = true)
        {
            return Exists ? _instance.InternalGet(tag, position, rotation, setPosAndRot) : null;
        }

        public static void SetActive(string tag, PoolObject obj, bool active)
        {
            if (Exists) { _instance.InternatSetActive(tag, obj, active); }
        }

        public static void PutBack(PoolObject obj)
        {
            if (Exists)
            {
                _instance.InternalPutBack(obj);
            }
            else
            {
                Debug.LogError($"[ERROR] Trying to Put Back object to Pool -> But there is no TheObjectPool exists.");
            }
        }

        // USE ONLY IN EDITOR (not in runtime)
        public void Add(GameObject prefab, int amount, PoolData.ResizePolicy policy)
        {
            if (prefab)
            {
                if (_pools == null)
                {
                    _pools = new List<PoolData>();
                }
                string tag = prefab.name;

                bool isInPool = false;
                foreach (PoolData pool in _pools)
                {
                    if (pool.tag.Equals(tag))
                    {
                        isInPool = true;
                        break;
                    }
                }
                if (!isInPool)
                {
                    _pools.Add(new PoolData()
                    {
                        prefabs = new List<GameObject>() { prefab },
                        size = amount,
                        tag = tag,
                        policy = policy
                    });
                }
            }
        }

        public void RuntimeAdd(GameObject prefab, int amount, PoolData.ResizePolicy policy)
        {
            if (prefab)
            {
                PoolData pool = new PoolData()
                {
                    prefabs = new List<GameObject>() { prefab },
                    size = amount,
                    tag = prefab.name,
                    policy = policy
                };
                InitSinglePool(pool);
            }
        }
        #endregion

        #region Helpers
        private void InitSinglePool(PoolData obj)
        {
            if (obj != null)
            {
                if (_poolMap == null) { _poolMap = new Dictionary<string, PoolGameObject>(); }
                if (obj.prefabs[0].GetComponent<PoolObject>())
                {
                    string tag = obj.tag;
                    if (!_poolMap.ContainsKey(tag))
                    {
                        PoolGameObject pool = new PoolGameObject(tag, obj.prefabs, obj.size, obj.policy, transform);
                        _poolMap.Add(tag, pool);
                    }
                }
                else
                {
                    Debug.LogError($"[ERROR] Missing PoolObject component! {obj.tag}");
                }
            }
        }

        private GameObject InternalGetSceneObject(string tag)
        {
            if (!string.IsNullOrEmpty(tag))
            {
                if (_sceneObjectsMap != null)
                {
                    if (_sceneObjectsMap.ContainsKey(tag))
                    {
                        return _sceneObjectsMap[tag];
                    }
                }
            }
            return null;
        }

        private bool InternalHas(string tag)
        {
            if (string.IsNullOrEmpty(tag))
            {
                return false;
            }
            return _poolMap.ContainsKey(tag);
        }

        private GameObject InternalGet(string tag, Vector3 position, Quaternion rotation, bool setPosAndRot = true)
        {
            if (string.IsNullOrEmpty(tag))
            {
                return null;
            }

            if (_poolMap != null && _poolMap.ContainsKey(tag))
            {
                return _poolMap[tag].Get(position, rotation, setPosAndRot);
            }
            else
            {
                Debug.LogError($"{gameObject} [ERROR] Unable to Find Pool with TAG: {tag}");
                return null;
            }
        }

        private void InternatSetActive(string tag, PoolObject obj, bool active)
        {
            if (string.IsNullOrEmpty(tag) || obj == null || !_poolMap.ContainsKey(tag))
            {
                return;
            }
            _poolMap[tag].SetActive(obj, active);
        }

        private void InternalPutBack(PoolObject obj)
        {
            if (obj)
            {
                string tag = obj.poolTag;
                if (_poolMap.ContainsKey(tag))
                {
                    _poolMap[tag].SetBack(obj);
                }
                else
                {
                    Debug.LogError($"{gameObject} [ERROR] Unable to Find Pool with TAG: {obj.poolTag}");
                }
            }
            else
            {
                Debug.LogError($"[ERROR] Trying to return NULL Object to the pool!");
            }
        }
        #endregion
    }

    public static class PoolExtensionMethods
    {

        #region TheObjectPool Extensions
        public static GameObject InstantiatePool(this GameObject gameObj, string tag, Vector3 position, Quaternion rotation)
        {
            return Pool.Get(tag, position, rotation);
        }

        public static void SetActivePool(this GameObject gameObj, bool active)
        {
            if (gameObj)
            {
                PoolObject obj = gameObj.GetComponent<PoolObject>();
                if (obj)
                {
                    Pool.SetActive(obj.poolTag, obj, active);
                }
                else
                {
                    gameObj.SetActive(active);
                }
            }
            else
            {
                Debug.LogError($"[ERROR] Custom Destroy on Destroed Game Object");
            }
        }

        public static void Destroy_Pool(this GameObject gameObj)
        {
            if (gameObj)
            {
                PoolObject obj = gameObj.GetComponent<PoolObject>();
                if (obj)
                {
                    Pool.PutBack(obj);
                }
                else
                {
                    Debug.LogWarning($"No pool object comp on game object {obj}", obj);
                    GameObject.Destroy(gameObj);
                }
            }
            else
            {
                Debug.LogError($"[ERROR] Custom Destroy on Destroed Game Object {gameObj}");
            }
        }
        #endregion
    }
}
