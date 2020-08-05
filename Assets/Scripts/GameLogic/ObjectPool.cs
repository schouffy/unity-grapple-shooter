using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour
{
    [System.Serializable]
    public class Pool
    {
        public Constants.PoolTag Tag;
        public GameObject Prefab;
        public int Size;
    }

    public List<Pool> Pools;
    public Dictionary<Constants.PoolTag, Queue<GameObject>> PoolDictionary;

    [HideInInspector]
    public static ObjectPool Instance;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        PoolDictionary = new Dictionary<Constants.PoolTag, Queue<GameObject>>();

        foreach (var pool in Pools)
        {
            var objectPool = new Queue<GameObject>();

            for (int i = 0; i < pool.Size; ++i)
            {
                GameObject obj = Instantiate(pool.Prefab, this.transform);
                obj.SetActive(false);
                objectPool.Enqueue(obj);
            }

            PoolDictionary.Add(pool.Tag, objectPool);
        }
    }

    public GameObject SpawnFromPool(Constants.PoolTag tag, Vector3 position, Quaternion rotation)
    {
        if (!PoolDictionary.ContainsKey(tag))
        {
            Debug.LogWarning($"Pool with tag '{tag}' doesn't exist");
            return null;
        }

        GameObject spawned = PoolDictionary[tag].Dequeue();
        spawned.SetActive(true);
        spawned.transform.position = position;
        spawned.transform.rotation = rotation;

        var poolable = spawned.GetComponent<IPoolable>();
        if (poolable != null)
            poolable.OnSpawn();

        PoolDictionary[tag].Enqueue(spawned);

        return spawned;
    }

}
