using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PoolContainer : MonoBehaviour
{
    [SerializeField]
    private GameObject prefab = null;

    public List<GameObject> poolList;

    [SerializeField, Tooltip("Toggle this if you want to reuse the first active objects when pool exceeds the size limit.")]
    private bool recycleObjects = false;

    [SerializeField, Tooltip("Will start reusing first active object when pool exceeds this size.")]
    private int recyclePoolLimit = 30;

    [SerializeField]
    private bool cacheOnStart = false;

    [SerializeField]
    private int cacheCount;

    public GameObject this[int index]// Indexer declaration  
    {
        get
        {
            if (index >= 0 && index < poolList.Count)
            {
                return poolList[index];
            }
            else
            {
                return null;
            }
        }
    }
    public GameObject Prefab => prefab;
    public GameObject GetActiveObject(int index)
    {
        if (index >= 0 && index < poolList.Count)
        {
            return poolList[index];
        }
        else return null;
    }

    public List<GameObject> ActiveObjects
    {
        get
        {
            if (poolList != null && poolList.Count > 0)
            {
                return poolList.FindAll(x => x.activeInHierarchy);
            }
            return null;
        }
    }

    public int GetActiveObjectCount()
    {
        return poolList.Count;
    }
    private void Awake()
    {
        poolList = new List<GameObject>();
    }


    private void ClearNullReferences()
    {
        for (int i = poolList.Count - 1; i >= 0; i--)
        {
            if (poolList[i] == null)
            {
                poolList.RemoveAt(i);
            }
        }
    }
    public GameObject Retrieve(Vector3 position, Quaternion rotation)
    {
        GameObject getObject = null;
        if (poolList != null)
        {
            if (poolList.Count > 0)
            {
                getObject = poolList.FirstOrDefault(x => x!=null && !x.gameObject.activeInHierarchy);
                if (getObject != null)
                {
                    getObject.transform.position = position;
                }
            }
        }
        if (getObject == null)
        {
            if (prefab != null)
            {
                getObject = Instantiate(prefab, position, Quaternion.identity);
                getObject.SetActive(false);
                if (poolList == null)
                {
                    poolList = new List<GameObject>();
                }
                getObject.transform.SetParent(transform);
                poolList.Add(getObject);
            }
        }
        return getObject;
    }

    public GameObject Retrieve(Transform targetSpawn)
    {
        return Retrieve(targetSpawn.position, targetSpawn.rotation);
    }
    public GameObject Retrieve(Vector3 position)
    {
        return Retrieve(position, Quaternion.identity);
    }
}
