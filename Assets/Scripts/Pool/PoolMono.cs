using System.Collections.Generic;
using UnityEngine;

public class PoolMono<T> where T : MonoBehaviour
{
    public T prefab { get; }
    public Transform container { get; }
    public List<T> pool;
    public PoolMono(T prefab, int count)
    {
        this.prefab = prefab;
        this.container = null;
        this.CreatePool(count);
    }
    public PoolMono(T prefab, int count, Transform container)
    {
        this.prefab = prefab;
        this.container = container;
        this.CreatePool(count);
    }
    void CreatePool(int count)
    {
        this.pool = new List<T>();
        for (int i = 0; i < count; i++)
            this.CreateObject();
    }
    public T CreateObject(bool IsActiveByDefolt = false)
    {
        var createdObject = UnityEngine.Object.Instantiate(this.prefab, this.container);
        createdObject.gameObject.SetActive(IsActiveByDefolt);
        this.pool.Add(createdObject);
        return createdObject;
    }
    public bool TryToGetFreeElement(out T element, bool IsActiveByDefolt = true)
    {
        foreach (var mono in pool)
        {
            if (!mono.gameObject.activeInHierarchy)
            {
                element = mono;
                mono.gameObject.SetActive(IsActiveByDefolt);
                return true;
            }
        }
        element = null;
        return false;
    }
    public T GetElement(bool IsActiveByDefolt = true)
    {
        if (this.TryToGetFreeElement(out var element, IsActiveByDefolt))
            return element;
        return this.CreateObject();
    }
}


