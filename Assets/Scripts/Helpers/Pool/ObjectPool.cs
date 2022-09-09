using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class ObjectPool : MonoBehaviour
{
    #region Static Variables
    private static Dictionary<AssetReference , ObjectPool> _pools = new Dictionary<AssetReference , ObjectPool>();
    private static GameObject _mainContainer;
    #endregion

    public delegate void PoolRequestCallback(GameObject gameObject);

    #region Instance Variables
    public string PoolName => _assetReference.RuntimeKey.ToString();

    private AssetReference _assetReference;
    private List<GameObject> _objects;
    private List<GameObject> _availableObjects;
    private bool _autoResize;
    private int _size;
    private bool _loadingStarted;
    private Queue<PoolRequest> _pendingRequests;
    #endregion

    #region Static Methods
    public static ObjectPool CreatePool(AssetReference assetReference, int size, bool autoResize, bool instantiateImmediate = false)
    {
        if(_mainContainer == null)
        {
            _mainContainer = new GameObject("Object Pools");
            _pools.Clear();
        }

        if (_pools.ContainsKey(assetReference))
            return _pools[assetReference];

        GameObject container = new GameObject(assetReference.RuntimeKey.ToString());
        ObjectPool pool = container.AddComponent<ObjectPool>();
        pool.SetProperties(assetReference, size, autoResize);
        _pools.Add(assetReference, pool);

        if (instantiateImmediate)
            pool.InstantiatePool();

        container.transform.parent = _mainContainer.transform;

        return pool;
    }
    #endregion

    #region Pool Methods
    private void SetProperties(AssetReference assetReference, int size, bool autoResize)
    {
        _assetReference = assetReference;
        _size = size;
        _autoResize = autoResize;
        _objects = new List<GameObject>(size);
        _availableObjects = new List<GameObject>(size);
        _pendingRequests = new Queue<PoolRequest>();
    }

    public void InstantiatePool()
    {
        if (_objects.Count != 0)
            return;

        if (_loadingStarted)
            return;

        _loadingStarted = true;
        _assetReference.LoadAssetAsync<GameObject>().Completed += OnAssetLoaded;
    }

    private void OnAssetLoaded(AsyncOperationHandle<GameObject> operation)
    {
        for (int i = 0; i < _size; i++)
        {
            GameObject instance = NewObjectInstance();
            instance.SetActive(false);
        }

        while(_pendingRequests.Count > 0)
        {
            PoolRequest request = _pendingRequests.Dequeue();

            if (RetriveFromPool(request.Position, request.Rotation, out GameObject go))
                request.Callback?.Invoke(go);
        }
    }

    public bool TryGetObject(Vector3 position, Quaternion rotation, out GameObject retrieved, PoolRequestCallback callback)
    {
        if(!_loadingStarted)
        {
            Debug.LogWarning("Trying to get an object from a pool that has not been instantiated yet");
            retrieved = null;
            return false;
        }

        if(!_assetReference.IsDone)
        {
            _pendingRequests.Enqueue(new PoolRequest(position, rotation, callback));
            retrieved = null;
            return false;
        }

        return RetriveFromPool(position, rotation, out retrieved);
    }

    private bool RetriveFromPool(Vector3 position, Quaternion rotation, out GameObject retrieved)
    {
        int lastIndex = _availableObjects.Count - 1;

        if (_availableObjects.Count > 0)
        {
            if (_availableObjects[lastIndex] == null)
            {
                Debug.LogError($"Missing object from {PoolName}. Have you accidentally destroyed it?");
                retrieved = null;
                return false;
            }

            _availableObjects[lastIndex].transform.position = position;
            _availableObjects[lastIndex].transform.rotation = rotation;
            _availableObjects[lastIndex].SetActive(true);
            retrieved = _availableObjects[lastIndex];
            _availableObjects.RemoveAt(lastIndex);
            return true;
        }

        if (_autoResize)
        {
            GameObject g = NewObjectInstance();
            g.transform.position = position;
            g.transform.rotation = rotation;
            retrieved = g;
            return true;
        }

        retrieved = null;
        return false;
    }

    private GameObject NewObjectInstance()
    {
        GameObject instance = Instantiate(_assetReference.Asset) as GameObject;
        instance.transform.SetParent(transform, false);
        _objects.Add(instance);

        PooledObject pooledObject = instance.GetComponent<PooledObject>();

        if (pooledObject != null)
            pooledObject.Pool = this;
        else
            instance.AddComponent<PooledObject>().Pool = this;

        return instance;
    }

    public void ClearPool()
    {
        foreach (var instance in _objects)
        {
            if (instance == null)
                continue;

            Addressables.ReleaseInstance(instance);
            Destroy(instance);
        }

        _objects.Clear();
        _availableObjects.Clear();
    }

    public void DeletePool()
    {
        ClearPool();

        _assetReference.ReleaseAsset();
        _loadingStarted = false;
    }

    public void AddToAvailableObjects(GameObject objectInstance)
    {
        _availableObjects.Add(objectInstance);
    }
    #endregion

    private struct PoolRequest
    {
        public Vector3 Position;
        public Quaternion Rotation;
        public PoolRequestCallback Callback;

        public PoolRequest(Vector3 position, Quaternion rotation, PoolRequestCallback callback)
        {
            Position = position;
            Rotation = rotation;
            Callback = callback;
        }
    }
}
