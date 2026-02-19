using System.Collections.Generic;
using UnityEngine;

public abstract class GenericPooler<T> : MonoBehaviour where T : Component
{
    [SerializeField] protected T prefab;
    [SerializeField] protected int initialSize = 10;

    protected List<T> pool = new List<T>();

    protected virtual void Awake()
    {
        for (int i = 0; i < initialSize; i++)
        {
            AddObjectToPool();
        }
    }

    protected T AddObjectToPool()
    {
        T newObj = Instantiate(prefab, transform);
        newObj.gameObject.SetActive(false);
        pool.Add(newObj);
        return newObj;
    }

    public virtual T Get(Transform parent = null)
    {
        foreach (var item in pool)
        {
            if (!item.gameObject.activeInHierarchy)
            {
                PrepareObject(item, parent);
                return item;
            }
        }

        T expandedObj = AddObjectToPool();
        PrepareObject(expandedObj, parent);
        return expandedObj;
    }

    private void PrepareObject(T item, Transform parent)
    {
        if (parent != null) item.transform.SetParent(parent);
        item.transform.localScale = Vector3.one;
        item.gameObject.SetActive(true);
    }

    public void DeactivateAll()
    {
        foreach (var item in pool)
        {
            item.gameObject.SetActive(false);
        }
    }
}
