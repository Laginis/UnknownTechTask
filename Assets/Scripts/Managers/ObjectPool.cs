using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectPool : MonoBehaviour //TODO: add async asset loading with Addressables??
{
    public static ObjectPool Instance { get; private set; }

    [Serializable]
    public struct PoolEntryData
    {
        public Component PrefabComponent;
        public int InitAmount;
    }
    [SerializeField] private PoolEntryData[] poolData;

    private readonly Dictionary<Type, Component> poolEntries = new();
    private readonly Dictionary<Type, Stack<Component>> availableEntries = new();


    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        StartCoroutine(InitializePool());
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }

    public T Request<T>(Transform parent = null) where T : Component
    {
        T request;
        Type type = typeof(T);
        if (availableEntries.ContainsKey(type) && availableEntries[type].Count > 0)
        {
            Component component = availableEntries[type].Pop();
            GameObject obj = component.gameObject;
            obj.transform.SetParent(parent, false);
            obj.SetActive(true);
            request = component as T;
        }
        else
        {
            request = CreateNewEntry(type, parent) as T;
        }

        return request;
    }

    private Component CreateNewEntry(Type type, Transform parent, bool isActive = true)
    {
        if (poolEntries.ContainsKey(type))
        {
            GameObject obj = Instantiate(poolEntries[type].gameObject, parent, false);
            obj.SetActive(isActive);
            return obj.GetComponent(type);
        }

        return null;
    }

    public void Return<T>(T obj) where T : Component
    {
        Type type = typeof(T);
        if (!availableEntries.ContainsKey(type)) return;

        obj.transform.SetParent(transform, false);
        obj.gameObject.SetActive(false);

        availableEntries[type].Push(obj);
    }

    private IEnumerator InitializePool()
    {
        int objectsPerFrame = 50;
        int counter = 0;
        foreach (var poolEntry in poolData)
        {
            var type = poolEntry.PrefabComponent.GetType();

            if (poolEntries.ContainsKey(type)) continue;

            poolEntries.Add(type, poolEntry.PrefabComponent);
            availableEntries.Add(type, new());

            for (int i = 0; i < poolEntry.InitAmount; i++)
            {
                availableEntries[type].Push(CreateNewEntry(type, transform, false));

                if (counter >= objectsPerFrame)
                {
                    counter = 0;
                    yield return null;
                }
            }
        }
    }

}
