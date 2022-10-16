using System;
using UnityEngine;
using UnityEngine.Pool;

public class MwObjectPool {

    public MwObjectPool(GameObject prefab, Func<GameObject> createFunc, Action<GameObject> actionOnGet = null, Action<GameObject> actionOnRelease = null, Action<GameObject> actionOnDestroy = null, bool collectionCheck = true, int defaultCapacity = 10, int maxSize = 10000) {
        this.objectPrefab = prefab;
        _pool = new ObjectPool<GameObject>(createFunc,actionOnGet,actionOnRelease,actionOnDestroy,collectionCheck,defaultCapacity,maxSize);
    }

    public  GameObject objectPrefab;
    private ObjectPool<GameObject> _pool;
    public GameObject Get() => _pool.Get();
    public void Release(GameObject instance) =>_pool.Release(instance);
    public void Clear() => _pool.Clear();

}
