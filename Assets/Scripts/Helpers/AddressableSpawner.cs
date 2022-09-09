using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

public class AddressableSpawner : MonoBehaviour
{
    [SerializeField] private AssetReference _assetReference;

    private AsyncOperationHandle<GameObject> _asyncOperation;

    public void Spawn()
    {
        if(!_assetReference.RuntimeKeyIsValid())
        {
            Debug.LogWarning($"The addressable key {_assetReference.RuntimeKey} is not valid");
            return;
        }
        
        //This would load the asset and instantiate it, not sure if it is a good practice
        //_assetReference.InstantiateAsync(GetRandomPosition(), Quaternion.identity).Completed += (operation) => 
        //{
        //    operation.Result.AddComponent<NotifyOnDestroy>().Destroyed += OnInstanceDestroyed;
        //};

        if (!_asyncOperation.IsValid())
        {
            LoadAndSpawn();
            return;
        }

        SpawnFromLoaded();
    }

    //You can use a couroutine and wait for loading instead of using the Completed event
    //private IEnumerator SpawnCoroutine()
    //{
    //    _asyncOperation = Addressables.LoadAssetAsync<GameObject>(_assetReference);

    //    yield return _asyncOperation;

    //    if(_asyncOperation.Status == AsyncOperationStatus.Succeeded)
    //        SpawnFromLoaded();
    //}

    private void LoadAndSpawn()
    {
        Debug.Log("LoadAndSpawn");

        //If using Addressables.LoadAssetAsync, the assetReference.Asset would be null even after loaded and the op.Result must be used
        //_asyncOperation = Addressables.LoadAssetAsync<GameObject>(_assetReference);

        _asyncOperation = _assetReference.LoadAssetAsync<GameObject>();
        _asyncOperation.Completed += OnLoadAssetAsyncCompleted;
    }

    private void OnLoadAssetAsyncCompleted(AsyncOperationHandle<GameObject> op)
    {
        if(op.Status != AsyncOperationStatus.Succeeded)
        {
            Debug.LogWarning($"Load asset async did not succeeded ({op.Status})");
            return;
        }

        //The operation.Result contains the asset loaded and could also be used
        //Instantiate(_asyncOperation.Result, GetRandomPosition(), Quaternion.identity);

        SpawnFromLoaded();
    }

    private void SpawnFromLoaded()
    {
        GameObject go = Instantiate(_assetReference.Asset, GetRandomPosition(), Quaternion.identity) as GameObject;

        var notify = go.AddComponent<NotifyOnDestroy>();
        notify.Destroyed += OnInstanceDestroyed;
    }

    private void OnInstanceDestroyed(GameObject gameObject)
    {
        //If AssetReference.InstanciateAsync used, instance must also be released
        //Addressables.ReleaseInstance(gameObject);

        _assetReference.ReleaseAsset();

        //Other way of releasing an addressable
        //Addressables.Release(_asyncOperation);
    }

    private Vector3 GetRandomPosition()
    {
        return Random.insideUnitSphere * 2f;
    }
}